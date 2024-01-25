using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using EmojiPad.Data;
using EmojiPad.Services;
using EmojiPad.Utils;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SQLite;

namespace EmojiPad.Services
{
    public class FileService
    {
        private readonly EmojiPadConfiguration _config;
        private EventService _event;
        private SQLiteConnection _context;
        private SemaphoreSlim _syncSemaphore;

        public FileService(EventService events, EmojiPadConfiguration config, SQLiteConnection ctx)
        {
            _event = events;
            _config = config;
            _context = ctx;
            _syncSemaphore = new SemaphoreSlim(1);

            if (!Directory.Exists(_config.EmojiFolderPath))
            {
                Directory.CreateDirectory(_config.EmojiFolderPath);
            }
        }

        public void Sync()
        {

            Task.Run(async () =>
            {
                if (_syncSemaphore.CurrentCount != 0)
                {
                    try
                    {
                        await _syncSemaphore.WaitAsync();
                        if (SyncEmojiCache())
                        {
                            _event.SetBusy(1000);
                            _event.InvokeRefreshEmojis();
                        }
                    }
                    finally
                    {
                        _syncSemaphore.Release();
                    }
                }
            });
        }

        private bool SyncEmojiCache()
        {
            bool changed = false;

            var dir = new DirectoryInfo(_config.EmojiFolderPath);
            var files = new Dictionary<string, string>();
            
            foreach (var enumerateFile in dir.EnumerateFiles())
            {
                _event.SetBusy(500);
                if (IsValidImage(enumerateFile))
                {
                    files[enumerateFile.Name] = HashFile(enumerateFile.Name);
                }
            }

            // Remove deleted files
            var deletedEmojis = new List<Emoji>();
            foreach (var k in _context.Table<Emoji>())
            {
                _event.SetBusy(500);
                if (!files.ContainsKey(k.EmojiName) || !files.ContainsValue(k.FileHash))
                {
                    changed = true;
                    deletedEmojis.Add(k);
                    Console.WriteLine($"Deleting Emoji {k.EmojiName} from index, the file was removed from disk!");
                }
                else
                {
                    files.Remove(k.EmojiName);
                }
            }
            _context.RunInTransaction(() =>
            {
                foreach (var k in deletedEmojis)
                {
                    _context.Delete(k);
                }
            });

            Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 3 }, (file, token) =>
            {
                _event.SetBusy(500);
                changed = true;
                Console.WriteLine($"Adding Emoji {file} to index, the file was created!");
                var emoji = new Emoji()
                {
                    EmojiName = file.Key,
                    UsedFrequency = 0,
                    FileHash = file.Value
                };
                CacheEmoji(emoji);
                _context.Insert(emoji);
            });

            return changed;
        }

        public string HashFile(string emojiName)
        {
            var fi = new FileInfo(Path.Join(_config.EmojiFolderPath, emojiName));
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fi.FullName))
                {
                    return Convert.ToHexString(md5.ComputeHash(stream));
                }
            }
        }

        public void CacheEmoji(Emoji emoji)
        {
            var cacheDir = Path.Join(Path.GetTempPath(), "emojipad");

            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }
            if((!File.Exists(Utilities.GetCachedPath(emoji)) || emoji.PasteSize != _config.EmojiPasteSize) && emoji.FileHash is not null)
            {
                Utilities.GetService<EventService>().SetBusy();
                if (emoji.PasteSize != _config.EmojiPasteSize)
                {
                    emoji.PasteSize = _config.EmojiPasteSize;
                    emoji.EmojiCacheId = "";
                    _context.Update(emoji);
                }
                var fi = new FileInfo(Path.Join(_config.EmojiFolderPath, emoji.EmojiName));
                Image img = Image.Load(fi.FullName, out var format);
                img.Mutate(x =>
                {
                    x.Resize(new ResizeOptions()
                    {
                        Size = Utilities.GetImageBounds(img.Size(), _config.EmojiPasteSize),
                        Sampler = KnownResamplers.Lanczos3
                    });
                });
                using var fs = File.Create(Utilities.GetCachedPath(emoji));
                img.Save(fs, format);
                fs.Close();
            }
        }

        public void UncacheEmoji(Emoji emoji)
        {
            if (File.Exists(Utilities.GetCachedPath(emoji)))
            {
                File.Delete(Utilities.GetCachedPath(emoji));
            }
        }

        public bool IsValidImage(FileInfo file)
        {
            return file.Extension.ToLower() == ".jpeg"
                   || file.Extension.ToLower() == ".png"
                || file.Extension.ToLower() == ".jpg";
        }
    }
}
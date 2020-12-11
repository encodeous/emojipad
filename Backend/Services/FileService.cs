using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using emojipad.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace emojipad.Services
{
    public class FileService
    {
        private readonly IConfiguration _configuration;
        private readonly EmojiContext _context;
        private readonly StatusService _status;
        private EventService _event;

        public FileService(IConfiguration configuration, EmojiContext context, StatusService status, EventService events)
        {
            _configuration = configuration;
            _context = context;
            _status = status;
            _event = events;
            context.Database.Migrate();

            if (!Directory.Exists(Utilities.GetEmojiFolderPath()))
            {
                Directory.CreateDirectory(Utilities.GetEmojiFolderPath());
            }
            
            Task.Run(() =>
            {
                FileSystemWatcher fsw = new FileSystemWatcher();
                fsw.Path = Utilities.GetEmojiFolderPath();
                fsw.Filter = "*.*";
                fsw.Renamed += FswOnRenamed;
                fsw.EnableRaisingEvents = true;
                while (Program.Running)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        if (SyncEmojiCache())
                        {
                            context.SaveChanges();
                            Thread.Sleep(1000);
                            _event.InvokeRefreshEmojis();
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });
        }
        private void FswOnRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                _status.SetBusy();
                var fi = new FileInfo(e.FullPath);

                if (fi.Directory.FullName != Utilities.GetEmojiFolderPath() && IsValidImage(fi))
                {
                    return;
                }

                Console.WriteLine($"Emoji {e.OldName} has been Renamed to {e.Name}! Updating Database...");
                var sel = from k in _context.Emojis
                    where k.FileName == e.OldName
                    select k;
                if (sel.Count() != 0)
                {
                    var f = sel.First();
                    f.FileName = fi.Name;
                }
                else
                {
                    var f = new Emoji()
                    {
                        FileName = fi.Name,
                        UsedFrequency = 0
                    };
                    f.FileName = fi.Name;
                    _context.Add(f);
                }
            }
            catch
            {
                // ignored
            }
        }
        

        private bool SyncEmojiCache()
        {
            bool changed = false;
            lock (_context)
            {
                var dir = new DirectoryInfo(Utilities.GetEmojiFolderPath());
                var files = new HashSet<string>();
                foreach (var enumerateFile in dir.EnumerateFiles())
                {
                    if (IsValidImage(enumerateFile))
                    {
                        files.Add(enumerateFile.Name);
                    }
                }

                // Remove deleted files
                var deletedEmojis = new List<Emoji>();
                foreach (var k in _context.Emojis)
                {
                    if (!files.Contains(k.FileName))
                    {
                        changed = true;
                        deletedEmojis.Add(k);
                        Console.WriteLine($"Deleting Emoji {k.FileName} from index, the file was removed from disk!");
                    }
                    else
                    {
                        files.Remove(k.FileName);
                    }
                }
                _context.RemoveRange(deletedEmojis);

                foreach (var file in files)
                {
                    changed = true;
                    Console.WriteLine($"Adding Emoji {file} to index, the file was created!");
                    _context.Add(new Emoji()
                    {
                        FileName = file,
                        UsedFrequency = 0
                    });
                }
                
                _context.SaveChanges();
            }

            return changed;
        }

        public bool IsValidImage(FileInfo file)
        {
            return file.Extension.ToLower() == ".jpeg"
                   || file.Extension.ToLower() == ".png"
                   || file.Extension.ToLower() == ".tga"
                   || file.Extension.ToLower() == ".bmp";
        }
    }
}
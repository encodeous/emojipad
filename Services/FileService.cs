using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using emojipad.Shared;
using Microsoft.Extensions.Configuration;

namespace emojipad.Services
{
    public class FileService
    {
        private readonly IConfiguration _configuration;
        private readonly EmojiContext _context;

        public FileService(IConfiguration configuration, EmojiContext context)
        {
            _configuration = configuration;
            _context = context;

            if (!Directory.Exists(Utilities.GetEmojiFolderPath()))
            {
                Directory.CreateDirectory(Utilities.GetEmojiFolderPath());
            }

            BuildEmojiCache();
            FileSystemWatcher fsw = new FileSystemWatcher();
            fsw.Path = Utilities.GetEmojiFolderPath();
            fsw.Filter = "*.*";
            fsw.Renamed += FswOnRenamed;
            fsw.Created += FswOnCreated;
            fsw.Deleted += FswOnDeleted;
            fsw.EnableRaisingEvents = true;
        }

        private void FswOnDeleted(object sender, FileSystemEventArgs e)
        {
            var fi = new FileInfo(e.FullPath);

            if (fi.Directory.FullName != Utilities.GetEmojiFolderPath() &&
                fi.Directory.Parent.FullName != Utilities.GetEmojiFolderPath())
            {
                return;
            }
            Console.WriteLine($"Emoji Deleted! Updating Database...");
            var sel = from k in _context.Emojis
                where k.RelativeFilePath == Path.GetRelativePath(Utilities.GetEmojiFolderPath(), e.FullPath)
                select k;
            if (sel.Count() != 0)
            {
                var f = sel.First();
                _context.Remove(f);
                _context.SaveChanges();
            }
            _context.SaveChanges();
        }

        private void FswOnCreated(object sender, FileSystemEventArgs e)
        {
            var fi = new FileInfo(e.FullPath);

            if (fi.Directory.FullName != Utilities.GetEmojiFolderPath() &&
                fi.Directory.Parent.FullName != Utilities.GetEmojiFolderPath())
            {
                return;
            }
            Console.WriteLine($"Emoji Found! Updating Database...");
            var f = new Emoji()
            {
                FileName = fi.Name,
                FolderName = fi.DirectoryName,
                RelativeFilePath = Path.GetRelativePath(Utilities.GetEmojiFolderPath(), fi.FullName),
                UsedFrequency = 0
            };
            f.FolderName = fi.DirectoryName;
            f.FileName = fi.Name;
            _context.Add(f);
            _context.SaveChanges();
        }

        private void FswOnRenamed(object sender, RenamedEventArgs e)
        {
            var fi = new FileInfo(e.FullPath);

            if (fi.Directory.FullName != Utilities.GetEmojiFolderPath() &&
                fi.Directory.Parent.FullName != Utilities.GetEmojiFolderPath())
            {
                return;
            }
            Console.WriteLine($"Emoji Renamed! Updating Database...");
            var sel = from k in _context.Emojis
                where k.RelativeFilePath == Path.GetRelativePath(Utilities.GetEmojiFolderPath(), e.OldFullPath)
                select k;
            if (sel.Count() != 0)
            {
                var f = sel.First();
                f.FolderName = fi.DirectoryName;
                f.FileName = fi.Name;
                f.RelativeFilePath = Path.GetRelativePath(Utilities.GetEmojiFolderPath(), fi.FullName);
            }
            else
            {
                var f = new Emoji()
                {
                    FileName = fi.Name,
                    FolderName = fi.DirectoryName,
                    RelativeFilePath = Path.GetRelativePath(Utilities.GetEmojiFolderPath(), fi.FullName),
                    UsedFrequency = 0
                };
                f.FolderName = fi.DirectoryName;
                f.FileName = fi.Name;
                _context.Add(f);
            }

            _context.SaveChanges();
        }
        

        private void BuildEmojiCache()
        {
            Console.WriteLine("Building Emoji Cache...");
            var dir = new DirectoryInfo(Utilities.GetEmojiFolderPath());
            var files = new HashSet<(string dir, string name)>();
            foreach (var enumerateFile in dir.EnumerateFiles())
            {
                files.Add(("", enumerateFile.Name));
            }
            foreach (var sdirs in dir.EnumerateDirectories())
            {
                foreach (var enumerateFile in sdirs.EnumerateFiles())
                {
                    files.Add((sdirs.Name, enumerateFile.Name));
                }
            }
            
            // Remove deleted files
            var deletedEmojis = new List<Emoji>();
            foreach (var k in _context.Emojis)
            {
                var tup = (k.FolderName, k.FileName);
                if (!files.Contains(tup))
                {
                    deletedEmojis.Add(k);
                }
                else
                {
                    files.Remove(tup);
                }
            }
            _context.RemoveRange(deletedEmojis);

            foreach (var file in files)
            {
                _context.Add(new Emoji()
                {
                    FileName = file.name,
                    FolderName = file.dir,
                    RelativeFilePath = Path.Join(file.dir, file.name),
                    UsedFrequency = 0
                });
            }

            _context.SaveChanges();
        }
    }
}
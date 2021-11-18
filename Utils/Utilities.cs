using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using EmojiPad.Data;
using EmojiPad.Services;
using Microsoft.Extensions.DependencyInjection;
using SQLite;
using static EmojiPad.Utils.KeyboardHook;
using Size = SixLabors.ImageSharp.Size;

namespace EmojiPad.Utils
{
    public static class Utilities
    {
        public static string GetCurDir()
        {
            return Environment.CurrentDirectory;
        }
        public static T GetService<T>()
        {
            return Program.Services.GetService<T>();
        }
        public static string GetEmojiFolderPath()
        {
            return Path.Join(GetCurDir(), "emojis");
        }
        public static string GetEmojiDatabase()
        {
            return Path.Join(GetCurDir(), "data");
        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetCachedPath(Emoji emoji)
        {
            if (emoji.FileHash == null) return null;
            if (string.IsNullOrEmpty(emoji.EmojiCacheId))
            {
                emoji.EmojiCacheId = RandomString(10);
                GetService<SQLiteConnection>().Update(emoji);
            }
            return Path.Join(Path.GetTempPath(), "emojipad", emoji.EmojiCacheId + emoji.EmojiName);
        }

        public static string GetActivationKeyDisplay(IList<VKeys> keys)
        {
            if(keys == null || keys.Count == 0) return "None";
            return string.Join(" + ", keys.Select(x=>x.ToString()));
        }
        public static void CopyFile(string path)
        {
            StringCollection paths = new StringCollection();
            paths.Add(path);
            Clipboard.SetFileDropList(paths);
        }
        public static Size GetImageBounds(Size imgSize, int maxBounds)
        {
            int maxDim = Math.Max(imgSize.Height, imgSize.Width);
            decimal val = (decimal)maxBounds / maxDim;
            return new Size((int)Math.Round(imgSize.Width * val), (int)Math.Round(imgSize.Height * val));
        }
    }
}
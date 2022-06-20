using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
        public static string GetDataDir()
        {
            return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "emojipad");
        }
        public static T GetService<T>()
        {
            return Program.Services.GetService<T>();
        }
        public static string GetEmojiFolderPath()
        {
            return Path.Join(GetDataDir(), "emojis");
        }
        public static string GetEmojiDatabase()
        {
            return Path.Join(GetDataDir(), "data");
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
        [DllImport("User32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_RESTORE = 9;
        public static bool AlreadyRunning()
        {
            bool running = false;
            try
            {
                // Getting collection of process  
                Process currentProcess = Process.GetCurrentProcess();

                // Check with other process already running   
                foreach (var p in Process.GetProcesses())
                {
                    if (p.Id != currentProcess.Id) // Check running process   
                    {
                        if (p.MainModule is not null && currentProcess.MainModule is not null
                            && p.MainModule.FileName == currentProcess.MainModule.FileName)
                        {
                            running = true;
                            IntPtr hFound = p.MainWindowHandle;
                            if (IsIconic(hFound))
                                ShowWindow(hFound, SW_RESTORE);
                            SetForegroundWindow(hFound);
                            break;
                        }
                    }
                }
            }
            catch { }
            return running;
        }
    }
}
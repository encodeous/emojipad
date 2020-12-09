using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace emojipad
{
    public class Utilities
    {
        public static string GetExecutingFile()
        {
            return Assembly.GetExecutingAssembly().Location;
        }
        public static string GetEmojiFolderPath()
        {
            return Path.Join(new FileInfo(GetExecutingFile()).Directory.FullName, "emojis");
        }
    }
}
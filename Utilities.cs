using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SixLabors.ImageSharp;

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
        public static string GetEmojiDatabase()
        {
            return Path.Join(new FileInfo(GetExecutingFile()).Directory.FullName, "data");
        }
        public static Size GetImageBounds(Size imgSize, int maxBounds)
        {
            int maxDim = Math.Max(imgSize.Height, imgSize.Width);
            decimal val = (decimal)maxBounds / maxDim;
            return new Size((int)Math.Round(imgSize.Width * val), (int)Math.Round(imgSize.Height * val));
        }
    }
}
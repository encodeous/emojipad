using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EmojiPad.Models;

namespace EmojiPad.Models.Config
{
    public class EmojiIndex
    {
        public Dictionary<string, Emoji> Emojis = new();

        public static EmojiIndex LoadFromFile(FileInfo path)
        {
            if (!path.Exists)
            {
                return new()
                {
                    Emojis = new Dictionary<string, Emoji>()
                };
            }
            return new()
            {
                Emojis = JsonSerializer.Deserialize<Dictionary<string, Emoji>>(File.ReadAllBytes(path.FullName))
            };
        }

        public void Save(FileInfo path)
        {
            lock (Emojis)
            {
                File.WriteAllBytes(path.FullName, JsonSerializer.SerializeToUtf8Bytes(Emojis));
            }
        }
    }
}

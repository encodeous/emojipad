using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiPad
{
    class Settings
    {
        public string EmojiPath = Path.Combine(Environment.CurrentDirectory, "emojis/");
        public Dictionary<string, int> EmojiFrequency = new Dictionary<string, int>();
    }
}

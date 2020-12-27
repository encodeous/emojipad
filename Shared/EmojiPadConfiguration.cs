using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json;

namespace emojipad.Shared
{
    public class EmojiPadConfiguration
    {
        private static object obj = new object();
        [Range(0, 1000, ErrorMessage = "Must be from 0 to 1000")]
        public bool ShowOnStart { get; set; }
        public bool RunOnStart { get; set; }
        public bool HideAfterCopy { get; set; }
        public bool HideAfterLostFocus { get; set; }
        public bool RegexSearch { get; set; }
        public bool AlwaysOnTop { get; set; }
        public string Keybind { get; set; }
        public string EmojiFolderPath { get; set; }
        public string EmojiDatabasePath { get; set; }
        [Range(1, 10000, ErrorMessage = "Must be from 1 to 10000")]
        public int EmojiPasteSize { get; set; }
        public int FrequentEmojiCount { get; set; }
        
        public static EmojiPadConfiguration CreateDefault()
        {
            var k = new EmojiPadConfiguration()
            {
                ShowOnStart = true,
                RunOnStart = false,
                Keybind = "Alt+E",
                EmojiPasteSize = 48,
                HideAfterCopy = false,
                FrequentEmojiCount = 32,
                RegexSearch = false,
                EmojiFolderPath = Utilities.GetEmojiFolderPath(),
                AlwaysOnTop = true,
                HideAfterLostFocus = false,
                EmojiDatabasePath = Utilities.GetEmojiDatabase()
            };
            if (!File.Exists("emojipad-conf.json"))
            {
                k.Save();
            }

            return k;
        }
        
        public static EmojiPadConfiguration Load()
        {
            lock (obj)
            {
                return JsonConvert.DeserializeObject<EmojiPadConfiguration>(File.ReadAllText("emojipad-conf.json"));
            }
        }

        public void Save()
        {
            lock (obj)
            {
                File.WriteAllText("emojipad-conf.json", JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        public EmojiPadConfiguration Clone()
        {
            return new EmojiPadConfiguration()
            {
                ShowOnStart = ShowOnStart,
                RunOnStart = RunOnStart,
                HideAfterCopy = HideAfterCopy,
                Keybind = Keybind+"",
                EmojiPasteSize = EmojiPasteSize,
                FrequentEmojiCount = FrequentEmojiCount,
                RegexSearch = RegexSearch,
                EmojiFolderPath = EmojiFolderPath,
                AlwaysOnTop = AlwaysOnTop,
                HideAfterLostFocus = HideAfterLostFocus,
                EmojiDatabasePath = EmojiDatabasePath
            };
        }
    }
}
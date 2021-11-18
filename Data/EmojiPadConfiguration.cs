using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;
using EmojiPad.Annotations;
using EmojiPad.Utils;
using Newtonsoft.Json;
using static EmojiPad.Utils.KeyboardHook;

namespace EmojiPad.Data
{
    public class EmojiPadConfiguration : INotifyPropertyChanged
    {
        private static object obj = new object();
        private bool _alwaysOnTop;

        public bool ShowOnStart { get; set; }
        public bool HideAfterCopy { get; set; }
        public bool CloseToTray { get; set; }
        public bool HideAfterLostFocus { get; set; }
        public bool RegexSearch { get; set; }

        public bool AlwaysOnTop
        {
            get => _alwaysOnTop;
            set
            {
                if (value == _alwaysOnTop) return;
                _alwaysOnTop = value;
                OnPropertyChanged();
            }
        }

        public List<VKeys> Keybind { get; set; }
        public string EmojiFolderPath { get; set; }
        public string EmojiDatabasePath { get; set; }
        [Range(1,1000)]
        public int EmojiPasteSize { get; set; }
        [Range(0, 1000)]
        public int FrequentEmojiCount { get; set; }
        
        public static EmojiPadConfiguration CreateDefault()
        {
            var k = new EmojiPadConfiguration()
            {
                ShowOnStart = true,
                Keybind = new List<VKeys>() { VKeys.LMENU, VKeys.KEY_E },
                EmojiPasteSize = 48,
                HideAfterCopy = false,
                FrequentEmojiCount = 24,
                RegexSearch = false,
                EmojiFolderPath = Utilities.GetEmojiFolderPath(),
                AlwaysOnTop = true,
                CloseToTray = true,
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
                HideAfterCopy = HideAfterCopy,
                Keybind = new List<VKeys>(Keybind),
                EmojiPasteSize = EmojiPasteSize,
                FrequentEmojiCount = FrequentEmojiCount,
                RegexSearch = RegexSearch,
                CloseToTray = CloseToTray,
                EmojiFolderPath = EmojiFolderPath,
                AlwaysOnTop = AlwaysOnTop,
                HideAfterLostFocus = HideAfterLostFocus,
                EmojiDatabasePath = EmojiDatabasePath
            };
        }

        public void Replace(EmojiPadConfiguration conf)
        {
            ShowOnStart = conf.ShowOnStart;
            HideAfterCopy = conf.HideAfterCopy;
            Keybind = conf.Keybind;
            EmojiPasteSize = conf.EmojiPasteSize;
            FrequentEmojiCount = conf.FrequentEmojiCount;
            RegexSearch = conf.RegexSearch;
            EmojiFolderPath = conf.EmojiFolderPath;
            CloseToTray = conf.CloseToTray;
            AlwaysOnTop = conf.AlwaysOnTop;
            HideAfterLostFocus = conf.HideAfterLostFocus;
            EmojiDatabasePath = conf.EmojiDatabasePath;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
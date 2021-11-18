using EmojiPad.Services;
using EmojiPad.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmojiPad.Data
{
    public class EmojiPadModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value,
    [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private string searchQuery;
        public string SearchQuery
        {
            get { return searchQuery; }
            set {
                if(searchQuery == value) return;
                SetField(ref searchQuery, value);
                Utilities.GetService<EventService>().ChangeQuery(value);
            }
        }

        private Emoji selectedEmoji = new Emoji()
        {
            EmojiName = "Hover over an emoji to show more information.",
            UsedFrequency = 0
        };
        public Emoji SelectedEmoji
        {
            get => selectedEmoji;
            set => SetField(ref selectedEmoji, value);
        }

        public bool IsHotKeyActive
        {
            get => _isHotKeyActive;
            set => SetField(ref _isHotKeyActive, value);
        }

        public string HotKeySelectorText
        {
            get => _hotKeySelectorText;
            set => SetField(ref _hotKeySelectorText, value);
        }

        public Brush StatusColor
        {
            get => _brushColor;
            set => SetField(ref _brushColor, value);
        }

        public ObservableCollection<Emoji> ShownEmojis { get; set; } = new ObservableCollection<Emoji>();

        public EmojiPadConfiguration Config { get; set; } = null;
        
        public Action HotkeyCallbackFunc;
        private bool _isHotKeyActive = true;
        private string _hotKeySelectorText;
        private Brush _brushColor = new SolidColorBrush(Colors.LimeGreen);
    }
}

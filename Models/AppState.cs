using EmojiPad.Models;
using PropertyChanged.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emojipad.Models
{
    public partial class AppState
    {
        [Notify]
        private string _query = "";
        public ObservableCollection<Emoji> DisplayedEmojis { get; set; }
    }
}

using EmojiPad.Models;
using PropertyChanged.SourceGenerator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmojiPad.Models.Config;

namespace EmojiPad.Models
{
    public partial class AppState
    {
        [Notify] private string _query = "";
        public ObservableCollection<Emoji> DisplayedEmojis { get; set; } = new ();
        [Notify] private bool _requiresSetup = true;
        [DependsOn(nameof(RequiresSetup))]
        public string SetupVisibility => RequiresSetup ? "Visible" : "Collapsed";
        [Notify] private EmojiPadConfig? _tempConfig;
        public MainWindow MainWindow { get; set; }
        [Notify] private bool _isNotConfiguringHotkey = true;
        public EmojiIndex Index = new (){ Emojis = new()};
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using emojipad.Models;

namespace emojipad.Services
{
    public class AppService
    {
        public static AppService Instance = new ();
        public AppState State { get; } = new ();
        public EmojiService Emoji { get; } = new();
        private AppService(){ }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmojiPad.Data
{
    internal class OptionsDataContext
    {
        public EmojiPadConfiguration Config { get; set; }
        public EmojiPadModel Model { get; set; }

        public OptionsDataContext(EmojiPadConfiguration conf, EmojiPadModel model)
        {
            Config = conf;
            Model = model;
        }
    }
}

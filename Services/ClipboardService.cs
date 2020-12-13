using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using emojipad.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;

namespace emojipad.Services
{
    public class ClipboardService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly EmojiPadConfiguration _config;
        public ClipboardService(IJSRuntime js, EmojiPadConfiguration configuration)
        {
            _jsRuntime = js;
            _config = configuration;
        }

        public void CopyEmoji(Emoji emoji, bool originalSize)
        {
            if (originalSize)
            {
                _jsRuntime.InvokeVoidAsync("copyImageFile", Path.Join(_config.EmojiFolderPath, emoji.FileName));
            }
            else
            {
                
                using var img = Image.Load(Path.Join(_config.EmojiFolderPath, emoji.FileName));
                var sz = Utilities.GetImageBounds(img.Size(), _config.EmojiPasteSize);
                _jsRuntime.InvokeVoidAsync("copyImageFileRescaled", 
                    Path.Join(_config.EmojiFolderPath, emoji.FileName), sz.Height, sz.Width);
            }
        }
    }
}

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
        private readonly IJSRuntime jsRuntime;
        private readonly IConfiguration config;
        public ClipboardService(IJSRuntime js, IConfiguration configuration)
        {
            jsRuntime = js;
            config = configuration;
        }

        public void CopyEmoji(Emoji emoji, bool originalSize)
        {
            if (originalSize)
            {
                jsRuntime.InvokeVoidAsync("copyImageFile", Path.Join(Utilities.GetEmojiFolderPath(), emoji.FileName));
            }
            else
            {
                
                using var img = Image.Load(Path.Join(Utilities.GetEmojiFolderPath(), emoji.FileName));
                var sz = Utilities.GetImageBounds(img.Size(), int.Parse(config["ems:emoji-paste-size"]));
                jsRuntime.InvokeVoidAsync("copyImageFileRescaled", 
                    Path.Join(Utilities.GetEmojiFolderPath(), emoji.FileName), sz.Height, sz.Width);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace emojipad.Services
{
    public class ClipboardService
    {
        private readonly IJSRuntime js;
        public ClipboardService(IJSRuntime js)
        {
            this.js = js;
        }

        public ValueTask CopyClipboard(string filePath)
        {
            return js.InvokeVoidAsync("copyImageFile", filePath);
        }
    }
}

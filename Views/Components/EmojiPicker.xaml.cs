using emojipad.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using emojipad.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace emojipad.Views.Components
{
    public sealed partial class EmojiPicker : UserControl
    {
        //public EmojiProvider Emojis { get; set; }
        public EmojiPicker()
        {
            this.InitializeComponent();
            DataContext = AppService.Instance.State;
            Loaded += (sender, args) =>
            {

                Search.Focus(FocusState.Keyboard);
            };
        }

        private void Search_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            args.TrySetNewFocusedElement(Search);
        }
    }
}

using EmojiPad.Models;
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
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using EmojiPad.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EmojiPad.Views.Components
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
                FocusSearch();
            };
        }

        public void FocusSearch()
        {
            Search.Focus(FocusState.Keyboard);
        }

        private void Search_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            if(AppService.Instance.State.TempConfig is null)
                args.TrySetNewFocusedElement(Search);
        }

        private void Search_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            Hint.Text = "";
            AppService.Instance.Emoji.StateChangedSameThread();
            ItemsView.ScrollView.ScrollTo(0, 0);

            UpdateHint();
        }

        public void UpdateHint()
        {
            if (Search.Text == "")
            {
                Hint.Text = "Search for Emojis";
            }
            else
            {
                if (AppService.Instance.State.DisplayedEmojis.Any())
                {
                    SetHint(AppService.Instance.State.DisplayedEmojis.First().EmojiName);
                }
                else Hint.Text = "";
            }
        }

        public void SetHint(string emojiName)
        {
            if (emojiName.ToLower().StartsWith(Search.Text.ToLower()))
            {
                Hint.Text = Search.Text + emojiName.Substring(Search.Text.Length);
            }
        }

        private void UIElement_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                SetHint((string)img.Tag);
            }
        }

        private void UIElement_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            UpdateHint();
        }

        private void UIElement_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // copy emoji
            AppService.Instance.Emoji.CopyEmoji((string)(sender as Image).Tag);
        }

        private void Search_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (AppService.Instance.State.DisplayedEmojis.Any())
            {
                var emojiName = AppService.Instance.State.DisplayedEmojis.First().EmojiName;
                if (emojiName.ToLower().StartsWith(Search.Text.ToLower()))
                {
                    AppService.Instance.Emoji.CopyEmoji(emojiName);
                }
            }
        }
    }
}

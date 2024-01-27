using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using EmojiPad.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NClone;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage;
using EmojiPad.Utils;
using SharpHook.Native;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EmojiPad.Views.Components
{
    public sealed partial class Settings : UserControl
    {
        public Settings()
        {
            this.InitializeComponent();
            DataContext = AppService.Instance.State;
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // Create a folder picker
            FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();

            // See the sample code below for how to make the window accessible from the App class.
            var window = AppService.Instance.State.MainWindow;

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your folder picker
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a folder
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                AppService.Instance.State.TempConfig.EmojiFolder = folder.Path;
                FolderText.Text = folder.Path;
            }
        }

        private async void HotkeySetClicked(object sender, RoutedEventArgs e)
        {
            var state = AppService.Instance.State;
            state.IsNotConfiguringHotkey = false;

            HashSet<KeyCode> keys = new ();
            HashSet<KeyCode> pressed = new();
            state.TempConfig.Hotkeys = keys;

            void OnHookOnKeyDown(object? sender, SharpHook.KeyboardHookEventArgs e)
            {
                keys.Add(e.Data.KeyCode);
                pressed.Add(e.Data.KeyCode);
                DispatcherQueue.TryEnqueue(UpdateText);
                e.SuppressEvent = true;
            }

            AppService.Instance.Hook.KeyPressed += OnHookOnKeyDown;
            var sslim = new SemaphoreSlim(0);

            void OnHookOnKeyUp(object? sender, SharpHook.KeyboardHookEventArgs e)
            {
                pressed.Remove(e.Data.KeyCode);
                if (!pressed.Any())
                {
                    sslim.Release();
                }
                e.SuppressEvent = true;
            }

            AppService.Instance.Hook.KeyReleased += OnHookOnKeyUp;
            await sslim.WaitAsync();
            AppService.Instance.Hook.KeyPressed -= OnHookOnKeyDown;
            AppService.Instance.Hook.KeyReleased -= OnHookOnKeyUp;

            state.IsNotConfiguringHotkey = true;
        }

        private void UpdateText()
        {
            Hotkey.Text = AppService.Instance.State.TempConfig.UiHotKey;
        }

        private void Config_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = AppService.ConfigPath.FullName
            });
        }
    }
}

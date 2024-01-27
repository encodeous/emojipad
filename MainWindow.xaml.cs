using EmojiPad.Utils;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.WindowManagement;
using AppUIBasics.Helper;
using WinRT.Interop;
using WinUIEx.Messaging;
using WinUIGallery.DesktopWap.Helper;
using System.Windows.Input;
using Windows.UI.Popups;
using EmojiPad.Services;
using EmojiPad.Views;
using EmojiPad.Views.Components;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using WinUIEx;
using WindowExtensions = WinUIEx.WindowExtensions;
using NClone;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EmojiPad
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {
        private bool _isSettingsOpen = false;
        public MainWindow()
        {
            AppService.Instance.State.MainWindow = this;
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            var monitor = new WindowMessageMonitor(this);
            monitor.WindowMessageReceived += Monitor_WindowMessageReceived;
            var root = Content as FrameworkElement;
            root.Loaded += Root_Loaded;
        }

        public void SetPickerActive(bool visible)
        {
            if (visible)
            {
                AppService.Instance.Emoji.StateChangedSameThread();
                this.Show(disableEfficiencyMode: true);
                WindowUtilities.SetForegroundWindow(this.GetWindowHandle());
                this.SetIsAlwaysOnTop(true);
                Activate();
            }
            else
            {
                AppService.Instance.State.DisplayedEmojis.Clear();
                this.Hide(enableEfficiencyMode: false);
                if (TrayIcon is not null)
                    TrayIcon.ForceCreate(false);
            }
        }

        public ICommand ShowHideWindowCommand
        {
            get
            {
                var command = new XamlUICommand();
                command.ExecuteRequested += (sender, e) =>
                {
                    var window = this;

                    SetPickerActive(!window.Visible);
                };

                return command;
            }
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                var command = new XamlUICommand();
                command.ExecuteRequested += (sender, e) =>
                {
                    AppService.Instance.Shutdown();
                    Close();
                };

                return command;
            }
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    await AppService.Instance.StartAsync();
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        SetPickerActive(!AppService.Instance.Config.StartMinimized);
                    });
                    Activated += (sender, args) =>
                    {
                        if (_isSettingsOpen) return;
                        if (args.WindowActivationState == WindowActivationState.Deactivated)
                        {
                            if (AppService.Instance.Config.HideOnLoseFocus)
                            {
                                HidePicker();
                            }
                        }
                    };
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Ex: {e} {e.Message}");
                }
            });
            TitleBarHelper.SetClickThrough(this, SettingsButton);
        }

        public void HidePicker()
        {
            SetPickerActive(false);
        }

        private void Monitor_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;
            const int WM_NCLBUTTONDBLCLK = 0x00A3; //double click on a title bar a.k.a. non-client area of the form

            switch (e.Message.MessageId)
            {
                case WM_SYSCOMMAND:             //preventing the form from being moved by the mouse.
                    int command = (int)(e.Message.WParam & 0xfff0);
                    if (command == SC_MOVE)
                        return;
                    break;
            }

            if (e.Message.MessageId == WM_NCLBUTTONDBLCLK)       //preventing the form being resized by the mouse double click on the title bar.
            {
                e.Handled = true;
                return;
            }
        }


        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            //myButton.Content = "Clicked";
        }

        private async void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            AppService.Instance.State.TempConfig = Clone.ObjectGraph(AppService.Instance.Config);
            var settings = new Settings();
            _isSettingsOpen = true;

            var saveCommand = new XamlUICommand();
            saveCommand.ExecuteRequested += async (sender, e) =>
            {
                var cfg = AppService.Instance.State.TempConfig!;
                if (Validator.TryValidateObject(cfg,
                        new ValidationContext(cfg), new List<ValidationResult>()))
                {
                    AppService.Instance.Config = cfg;
                    AppService.Instance.SaveConfig();
                    AppService.Instance.State.TempConfig = null;
                    await AppService.Instance.LoadConfigAsync();
                }
            };

            var dlg = new ContentDialog()
            {
                Title = "Edit Settings",
                Content = settings,
                CloseButtonText = "Cancel",
                XamlRoot = Content.XamlRoot,
                PrimaryButtonText = "Save",
                PrimaryButtonCommand = saveCommand
            };
            
            await dlg.ShowAsync();
            _isSettingsOpen = false;

            // return focus to the search bar
            Picker.FocusSearch();
        }
    }
}

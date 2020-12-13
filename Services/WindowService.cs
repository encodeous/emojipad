using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using emojipad.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Microsoft.Win32;

namespace emojipad.Services
{
    public class WindowService
    {
        private BrowserWindow _mainWindow;
        private readonly EmojiPadConfiguration _config;
        public WindowService(EmojiPadConfiguration config)
        {
            _config = config;
        }

        public void OnLoad()
        {
            Task.Run(async () =>
            {
                _mainWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
                {
                    Width = 420,
                    Height = 400,
                    Resizable = false,
                    AlwaysOnTop = _config.AlwaysOnTop,
                    Title = "EmojiPad",
                    Show = _config.ShowOnStart,
                    SkipTaskbar = _config.AlwaysOnTop,
                    Frame = false,
                    WebPreferences = new WebPreferences()
                    {
                        DevTools = false,
                        ZoomFactor = 1
                    }
                });
                Electron.GlobalShortcut.Register(_config.Keybind, () =>
                {
                    _mainWindow.Show();
                    _mainWindow.Focus();
                });
                _mainWindow.OnBlur += () =>
                {
                    if (_config.HideAfterLostFocus)
                    {
                        _mainWindow.Hide();
                    }
                };
                // try
                // {
                //     if (_config.RunOnStart)
                //     {
                //         using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                //         {
                //             string path = Path.Join(Utilities.GetExecutingFile(), "../../EmojiPad.exe");
                //             key.SetValue("EmojiPad", "\"" + path + "\"");
                //         }
                //     }
                //     else
                //     {
                //         using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                //         {
                //             key.DeleteValue("EmojiPad", false);
                //         }
                //     }
                // }
                // catch
                // {
                //     
                // }

                Electron.App.SetLoginItemSettings(new LoginSettings()
                {
                    OpenAtLogin = _config.RunOnStart
                });
            });
        }

        public void Hide()
        {
            _mainWindow.Hide();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectronNET.API;
using ElectronNET.API.Entities;
using emojipad.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Microsoft.Win32;
using Image = SixLabors.ImageSharp.Image;
using Size = System.Drawing.Size;

namespace emojipad.Services
{
    public class WindowService
    {
        private bool Started = false;
        private BrowserWindow _mainWindow;
        private readonly EmojiPadConfiguration _config;
        public WindowService(EmojiPadConfiguration config)
        {
            _config = config;
        }

        public async Task CreateWindow()
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
        }

        public void OnLoad()
        {
            Task.Run(async () =>
            {
                Started = true;
                await CreateWindow();
                Electron.WindowManager.IsQuitOnWindowAllClosed = false;
                Electron.GlobalShortcut.Register(_config.Keybind, Show);
                _mainWindow.OnBlur += () =>
                {
                    if (_config.HideAfterLostFocus)
                    {
                        _mainWindow.Hide();
                    }
                };
                
                Electron.App.SetLoginItemSettings(new LoginSettings()
                {
                    OpenAtLogin = _config.RunOnStart
                });
            });
            Task.Run(() =>
            {
                NotifyIcon icon = new NotifyIcon();
                icon.Icon = new Icon("icon.ico");
                icon.Text = "Show EmojiPad";
                var strip = new ContextMenuStrip();
                strip.SuspendLayout();
                strip.Size = new Size(152, 44);
                icon.DoubleClick += (sender, args) =>
                {
                    Show();
                };
                var item1 = new ToolStripMenuItem("Show EmojiPad");
                item1.Size = new Size(152, 22);
                item1.Click += (sender, args) =>
                {
                    Show();
                };
                var item2 = new ToolStripMenuItem("Exit EmojiPad");
                item2.Size = new Size(152, 22);
                item2.Click += (sender, args) =>
                {
                    try
                    {
                        Electron.App.Exit();
                    }
                    catch
                    {
                        
                    }
                };
                strip.Items.Add(item1);
                strip.Items.Add(item2);
                strip.ResumeLayout();
                icon.ContextMenuStrip = strip;
                icon.Visible = true;
                
                Application.Run();
            });
        }

        public void Show()
        {
            if (Started)
            {
                if (_mainWindow == null || Electron.WindowManager.BrowserWindows.Count == 0)
                {
                    CreateWindow().GetAwaiter().GetResult();
                }
                _mainWindow?.Show();
                _mainWindow?.Focus();
            }
        }

        public void Hide()
        {
            _mainWindow?.Hide();
        }
    }
}

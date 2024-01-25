using EmojiPad.Data;
using EmojiPad.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using static EmojiPad.Utils.KeyboardHook;

namespace EmojiPad.Services
{
    internal class ServiceHost : IHostedService
    {
        public static bool IsRunning = true;
        public static ServiceHost Instance { get; private set; }

        public static HashSet<VKeys> KeysDown = new HashSet<VKeys>();
        private static BackoffManager _queryDispatcher = new BackoffManager(TimeSpan.FromMilliseconds(200));
        private static BackoffManager _busyDispatcher = new BackoffManager(TimeSpan.FromMilliseconds(200));
        private static readonly Brush _busyBrush = new SolidColorBrush(Color.FromRgb(255, 163, 0));
        private static readonly Brush _readyBrush = new SolidColorBrush(Colors.LimeGreen);

        public void Stop()
        {
            IsRunning = false;
            Utilities.GetService<SQLiteConnection>().Close();
            Utilities.GetService<KeyboardHook>().Uninstall();
            Environment.Exit(0);
        }

        public void AddEmojis()
        {
            _queryDispatcher.ExecuteTask(() =>
            {
                MainWindow.Instance?.Dispatcher.Invoke(() =>
                {
                    Utilities.GetService<EventService>().SetBusy();
                    var model = Utilities.GetService<EmojiPadModel>();
                    if (string.IsNullOrEmpty(model.SearchQuery))
                    {
                        var file = Utilities.GetService<FileService>();
                        model.ShownEmojis.Clear();
                        var search = Utilities.GetService<SearchService>();
                        var _context = Utilities.GetService<SQLiteConnection>().Table<Emoji>();
                        if (_context.Count() >= 100)
                        {
                            foreach (var k in search.GetFavouriteEmojis())
                            {
                                model.ShownEmojis.Add(k);
                            }
                        }
                        foreach (var k in _context)
                        {
                            file.CacheEmoji(k);
                            model.ShownEmojis.Add(k);
                        }
                    }
                    else
                    {
                        var file = Utilities.GetService<FileService>();
                        model.ShownEmojis.Clear();
                        var search = Utilities.GetService<SearchService>();
                        foreach (var k in search.GetEmojis(model.SearchQuery))
                        {
                            file.CacheEmoji(k);
                            model.ShownEmojis.Add(k);
                        }
                    }
                });
                return ValueTask.CompletedTask;
            });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Instance = this;
            // "Warm Up" Services
            var kbh = Utilities.GetService<KeyboardHook>();
            kbh.Install();
            kbh.KeyDown += (k) =>
            {
                Debug.WriteLine($"DOWN {k.ToString()}");
                KeysDown.Add(k);
                return KeysUpdate();
            };
            kbh.KeyUp += (k) =>
            {
                Debug.WriteLine($"UP {k.ToString()}");
                KeysDown.Remove(k);
                return KeysUpdate();
            };
            Utilities.GetService<SearchService>();
            Utilities.GetService<FileService>();
            Utilities.GetService<EmojiPadModel>().Config = Utilities.GetService<EmojiPadConfiguration>();
            var wnd = Utilities.GetService<WindowService>();
            wnd.Initialize();
            var evt = Utilities.GetService<EventService>();
            evt.RefreshEmojis += AddEmojis;
            evt.QueryChangedEvent += x =>
            AddEmojis();
            evt.Busy += x =>
            {
                _busyDispatcher.ExecuteTask(async () =>
                {
                    await MainWindow.Instance.Dispatcher.Invoke(async () =>
                    {
                        var mdl = Utilities.GetService<EmojiPadModel>();
                        mdl.StatusColor = _busyBrush;
                        await Task.Delay(x);
                        mdl.StatusColor = _readyBrush;
                    });
                });
            };
            evt.EmojiCopiedEvent += emoji =>
            {
                var conf = Utilities.GetService<EmojiPadConfiguration>();
                Utilities.GetService<EventService>().SetBusy();
                var _context = Utilities.GetService<SQLiteConnection>();
                Utilities.CopyFile(Utilities.GetCachedPath(emoji));
                emoji.UsedFrequency++;
                _context.Update(emoji);
                AddEmojis();
                if (conf.HideAfterCopy)
                {
                    Utilities.GetService<WindowService>().HideWindow();
                }
            };
            return Task.CompletedTask;
        }

        private bool KeysUpdate()
        {
            var model = Utilities.GetService<EmojiPadModel>();
            if (!model.IsHotKeyActive && model.HotkeyCallbackFunc != null)
            {
                Task.Run(model.HotkeyCallbackFunc);
            }
            else
            {
                var conf = Utilities.GetService<EmojiPadConfiguration>();
                foreach (VKeys key in conf.Keybind)
                {
                    if (!KeysDown.Contains(key))
                    {
                        return false;
                    }  
                }
                
                foreach (VKeys key in KeysDown)
                {
                    if (!conf.Keybind.Contains(key))
                    {
                        return false;
                    }
                }
                
                KeysDown.Clear();

                Task.Run(() =>
                {
                    Utilities.GetService<WindowService>().ShowWindow();
                });
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // not needed
            throw new NotImplementedException();
        }
    }
}

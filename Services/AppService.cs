using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Popups;
using EmojiPad.Models;
using EmojiPad.Models.Config;
using EmojiPad.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharpHook;
using SharpHook.Native;

namespace EmojiPad.Services
{
    public class AppService
    {
        public static FileInfo ConfigPath = new(AppDataPaths.GetDefault().ProgramData + "\\config.json");
        public static AppService? Instance = new ();
        public AppState State { get; } = new ();
        public EmojiService Emoji { get; } = new();
        public EmojiPadConfig Config { get; set; }
        public HashSet<KeyCode> Pressed = new ();
        public TaskPoolGlobalHook Hook { get; set; }
        public FileSystemWatcher Watcher { get; set; }
        private AppService(){ }

        public void Shutdown()
        {
            Hook.Dispose();
            if (Config.EmojiFolder is not null)
            {
                var indexPath = Config.EmojiFolder + "/index.json";
                if (Directory.Exists(Config.EmojiFolder))
                {
                    State.Index.Save(new FileInfo(indexPath));
                }
            }

            Instance = null;
        }

        private DateTime _clearPressed = DateTime.Now;

        public async Task StartAsync()
        {
            await LoadConfigAsync();
            Hook = new TaskPoolGlobalHook();
            new Thread(() =>
            {
                try
                {
                    Hook.Run();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Ex while handling keyboard input: {e}");
                }
            }).Start();
            Hook.KeyPressed += (sender, args) =>
            {
                _clearPressed = DateTime.Now + TimeSpan.FromSeconds(10);
                try
                {
                    Pressed.Add(args.Data.KeyCode);
                    //Debug.WriteLine($"P {args.Data.KeyCode}");
                    if (Pressed.SetEquals(Config.Hotkeys))
                    {
                        Pressed.Clear();
                        HotkeyTrigger();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Ex while handling keyboard input: {e}");
                }
                
            };
            Hook.KeyReleased += (sender, args) =>
            {
                _clearPressed = DateTime.Now + TimeSpan.FromSeconds(10);
                try
                {
                    //Debug.WriteLine($"R {args.Data.KeyCode}");
                    Pressed.Remove(args.Data.KeyCode);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Ex while handling keyboard input: {e}");
                }
                
            };
            Task.Run(async () =>
            {
                while (Instance is not null)
                {
                    if (DateTime.Now > _clearPressed)
                    {
                        Pressed.Clear();
                    }

                    await Task.Delay(1000);
                }
            });
        }

        public async Task LoadConfigAsync()
        {
            ConfigPath = new(AppDataPaths.GetDefault().ProgramData + "\\config.json");
            StartupTask startupTask = await StartupTask.GetAsync("EmojiPadStartupId");
            Config = EmojiPadConfig.LoadFromFile(ConfigPath);
            if (Config.Autostart)
            {
                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                        // Task is disabled but can be enabled.
                        StartupTaskState newState = await startupTask.RequestEnableAsync();
                        Debug.WriteLine("Request to enable startup, result = {0}", newState);
                        break;
                    case StartupTaskState.Enabled:
                        Debug.WriteLine("Startup is enabled.");
                        break;
                }
            }
            else
            {
                startupTask.Disable();
            }


            var reqSetup = Config.EmojiFolder == null || !Directory.Exists(Config.EmojiFolder);
            if (!reqSetup)
            {
                IndexFiles();
                if(Watcher is not null)
                    Watcher.Dispose();
                Watcher = new FileSystemWatcher(Config.EmojiFolder);
                Watcher.Filters.Add("*.png");
                Watcher.Filters.Add("*.jpg");
                Watcher.Filters.Add("*.jpeg");
                Watcher.Filters.Add("*.gif");
                Watcher.Created += (sender, args) =>
                {
                    State.Index.Emojis[args.Name] = new Emoji()
                    {
                        EmojiName = args.Name,
                        Path = args.FullPath
                    };
                    Emoji.StateChanged();
                };
                Watcher.Deleted += (sender, args) =>
                {
                    State.Index.Emojis.Remove(args.Name);
                    Emoji.StateChanged();
                };
            }

            State.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                State.RequiresSetup = reqSetup;
            });
        }

        public void HotkeyTrigger()
        {
            State.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                State.MainWindow.SetPickerActive(!State.MainWindow.Visible);
            });
        }

        public void SaveConfig()
        {
            Config.Save(ConfigPath);
        }

        public void IndexFiles()
        {
            var index = new EmojiIndex();
            var indexPath = Config.EmojiFolder + "\\index.json";
            if (File.Exists(indexPath))
            {
                index = EmojiIndex.LoadFromFile(new FileInfo(indexPath));
            }
            foreach(var file in new DirectoryInfo(Config.EmojiFolder).EnumerateFiles())
            {
                if (file.Extension is ".png" or ".jpg" or ".jpeg" or ".gif")
                {
                    if (!index.Emojis.ContainsKey(file.Name))
                    {
                        index.Emojis[file.Name] = new Emoji()
                        {
                            EmojiName = file.Name,
                            Path = Config.EmojiFolder + "/" + file.Name
                        };
                    }
                    else
                    {
                        index.Emojis[file.Name].Path = Config.EmojiFolder + "/" + file.Name;
                    }
                }
            }

            List<string> removals = new ();
            foreach (var file in index.Emojis.Keys)
            {
                if(!File.Exists(Config.EmojiFolder + "/" + file))
                    removals.Add(file);
            }
            foreach (var removal in removals)
            {
                index.Emojis.Remove(removal);
            }

            State.Index = index;
            Emoji.StateChanged();
            index.Save(new FileInfo(indexPath));
        }
    }
}

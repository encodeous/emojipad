using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmojiPad.Utils;
using FuzzySharp;
using GregsStack.InputSimulatorStandard;
using Newtonsoft.Json;
using Directory = System.IO.Directory;

namespace EmojiPad
{
    class Controller
    {
        public static InputSimulator InputSimulatorS = new InputSimulator();
        public static bool Active = false;
        public static KeyboardHook EmojiKeyboardHook = new KeyboardHook();
        public static EmojiEvent OnButtonMouseDown;
        public static EmojiEvent OnButtonMouseHover;
        public static FileSystemWatcher FsSystemWatcher;

        public static Dictionary<string, Image> ImageMap = new Dictionary<string, Image>();
        public static Dictionary<string, string> ImagePath = new Dictionary<string, string>();
        public static Settings EmojiPadSettings;

        public delegate void EmojiEvent(string key);
        public static Button CreateButtonFromImage(string key)
        {
            var button = new LazyButton()
            {
                id = key,
                TabStop = false,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 37, 38),
                ForeColor = Color.FromArgb(37, 37, 38),
                Width = 36,
                Height = 36
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(79, 84, 92);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(79, 84, 92);
            button.MouseHover += (sender, args) => OnButtonMouseHover?.Invoke(key);
            button.MouseDown += (sender, args) => OnButtonMouseDown?.Invoke(key);
            button.Name = key;
            return button;
        }
        public static Image LoadPreview(string key)
        {
            return Extension.GetImageScaled(ImageMap[key], 40);
        }
        public static void Start()
        {
            EmojiKeyboardHook = new KeyboardHook();
            EmojiKeyboardHook.KeyDown += EmojiKeyboardHookOnKeyDown;
            EmojiKeyboardHook.KeyUp += EmojiKeyboardHookOnKeyUp;
            EmojiKeyboardHook.Install();

            if (File.Exists("settings.json"))
                EmojiPadSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
            EmojiPadSettings ??= new Settings();
            if (!Directory.Exists(EmojiPadSettings.EmojiPath))
            {
                Directory.CreateDirectory(EmojiPadSettings.EmojiPath);
            }

            SyncFiles();

            FsSystemWatcher = new FileSystemWatcher(EmojiPadSettings.EmojiPath);
            FsSystemWatcher.EnableRaisingEvents = true;
            FsSystemWatcher.Created += (sender, args) =>
            {
                SyncFiles();

                EmojiPicker.RunActionSafe(EmojiPicker.instance.RefreshEmojis);
            };

            FsSystemWatcher.Deleted += (sender, args) =>
            {
                SyncFiles();

                EmojiPicker.RunActionSafe(EmojiPicker.instance.RefreshEmojis);
            };

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    Save();
                }
            });

        }

        static HashSet<KeyboardHook.VKeys> keysDown = new HashSet<KeyboardHook.VKeys>();
        private static bool EmojiKeyboardHookOnKeyUp(KeyboardHook.VKeys key)
        {
            keysDown.Remove(key);
            return false;
        }

        static SemaphoreSlim sslim = new SemaphoreSlim(1);

        public static void Save()
        {
            if (sslim.CurrentCount == 0) return;
            sslim.Wait();

            File.WriteAllText("settings.json", JsonConvert.SerializeObject(EmojiPadSettings));

            sslim.Release();
        }

        private static bool EmojiKeyboardHookOnKeyDown(KeyboardHook.VKeys key)
        {
            if (Active)
            {
                if (key == KeyboardHook.VKeys.RETURN && !string.IsNullOrEmpty(EmojiPicker.instance.Selected))
                {
                    
                    EmojiPicker.RunActionSafe(() =>
                    {
                        EmojiPicker.instance.SelectEmoji(EmojiPicker.instance.Selected);
                    });
                    return true;
                }
                if (key == KeyboardHook.VKeys.ESCAPE)
                {
                    Active = false;
                    EmojiPicker.RunActionSafe(() =>
                    {
                        EmojiPicker.instance.Hide();
                    });
                    return true;
                }
            }

            if ((keysDown.Contains(KeyboardHook.VKeys.LMENU)|| keysDown.Contains(KeyboardHook.VKeys.RMENU))
                && keysDown.Contains(KeyboardHook.VKeys.KEY_E) && keysDown.Contains(KeyboardHook.VKeys.LWIN))
            {
                Active = true;
                EmojiPicker.RunActionSafe(() =>
                {
                    EmojiPicker.instance.UpdateLocation();
                    EmojiPicker.instance.Show();
                    EmojiPicker.instance.BringToFront();
                    EmojiPicker.instance.searchBar.Focus();
                });
                return true;
            }

            return false;
        }

        public static string[] GetImagesSorted()
        {
            var list = new List<(string, int)>();
            foreach (var k in ImageMap.Keys)
            {
                if (!EmojiPadSettings.EmojiFrequency.ContainsKey(k))
                {
                    EmojiPadSettings.EmojiFrequency[k] = 0;
                }
                list.Add((k, EmojiPadSettings.EmojiFrequency[k]));
            }
            list.Sort((o1, o2) =>
            {
                if (o1.Item2 < o2.Item2)
                {
                    return 1;
                }
                if (o1.Item2 > o2.Item2)
                {
                    return -1;
                }
                return String.Compare(o1.Item1, o2.Item1, StringComparison.Ordinal);
            });
            var imgList = new List<string>();
            foreach (var k in list)
            {
                imgList.Add(k.Item1);
            }

            return imgList.ToArray();
        }

        public static string[] FuzzyImageSearch(string query)
        {
            var list = new List<(string, int)>();
            foreach (var k in ImageMap.Keys)
            {
                list.Add((k, Fuzz.WeightedRatio(query, k)));
            }
            list.Sort((o1, o2) =>
            {
                if (o1.Item2 < o2.Item2)
                {
                    return 1;
                }
                if (o1.Item2 > o2.Item2)
                {
                    return -1;
                }
                return String.Compare(o1.Item1, o2.Item1, StringComparison.Ordinal);
            });
            var imgList = new List<string>();
            foreach (var k in list)
            {
                imgList.Add(k.Item1);
            }

            return imgList.ToArray();
        }

        public static void SyncFiles()
        {
            var files = Directory.GetFiles(EmojiPadSettings.EmojiPath);
            foreach (var file in files)
            {
                try
                {
                    FileInfo fi = new FileInfo(file);
                    ImageMap[fi.Name] = Image.FromFile(file);
                    ImagePath[fi.Name] = file;
                }
                catch
                {
                    Console.WriteLine("Failed to load "+ file+"!");
                }
            }

        }

        public static void CopyImg(string id)
        {
            ClipboardUtils.CopyImage(Extension.GetImageScaled(ImageMap[id], 90));
        }
    }
}

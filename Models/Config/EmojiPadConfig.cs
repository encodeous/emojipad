using EmojiPad.Models;
using EmojiPad.Utils;
using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Popups;

namespace EmojiPad.Models.Config
{
    public class EmojiPadConfig
    {
        private static readonly EmojiPadConfig defaultConf = new ()
        {
            Autostart = true,
            CropSize = 48,
            Hotkeys = [KeyCode.VcLeftAlt, KeyCode.VcQ]
        };
        public bool HideOnLoseFocus { get; set; }
        public bool Autostart { get; set; }
        public bool StartMinimized { get; set; }
        public bool HideAfterCopy { get; set; }
        [Range(-1, 1000)]
        public int CropSize { get; set; }
        public string? EmojiFolder { get; set; }
        public HashSet<KeyCode> Hotkeys { get; set; } = new();
        public string UiHotKey => string.Join(" + ", Hotkeys);

        public static EmojiPadConfig LoadFromFile(FileInfo path)
        {
            if (!path.Exists)
            {
                defaultConf.Save(path);
                return defaultConf;
            }

            try
            {
                var file = JsonSerializer.Deserialize<EmojiPadConfig>(
                    File.ReadAllBytes(path.FullName)
                );
                if (file is null)
                {
                    return defaultConf;
                }

                return file;
            }
            catch (Exception e)
            {
                var diag = new MessageDialog(
                    $"An exception occurred while loading EmojiPad configuration files: {e}. The default configuration has been loaded.");
                diag.ShowAsync();
                return defaultConf;
            }
        }

        public void Save(FileInfo path)
        {
            if (!new DirectoryInfo(AppDataPaths.GetDefault().ProgramData).Exists)
                new DirectoryInfo(AppDataPaths.GetDefault().ProgramData).Create();
            File.WriteAllBytes(path.FullName, JsonSerializer.SerializeToUtf8Bytes(this));
        }
    }
}

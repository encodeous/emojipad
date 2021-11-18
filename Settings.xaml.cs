using EmojiPad.Data;
using EmojiPad.Services;
using EmojiPad.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static EmojiPad.Utils.KeyboardHook;

namespace EmojiPad
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private EmojiPadModel model;
        private EmojiPadConfiguration curConf;
        private HashSet<VKeys> keys = null;

        public Settings()
        {
            model = Utilities.GetService<EmojiPadModel>();
            var conf = Utilities.GetService<EmojiPadConfiguration>();
            curConf = conf.Clone();
            model.HotKeySelectorText = $"[{Utilities.GetActivationKeyDisplay(conf.Keybind)}] Click to set activation key";
            DataContext = new OptionsDataContext(curConf, model);
            InitializeComponent();
        }

        private void Activation_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsHotKeyActive)
            {
                model.IsHotKeyActive = false;
                keys = new HashSet<VKeys>();
                model.HotKeySelectorText = GetKeyDisplay();
                model.HotkeyCallbackFunc += () =>
                {
                    foreach(var k in ServiceHost.KeysDown)
                    {
                        keys.Add(k);
                    }
                    model.HotKeySelectorText = $"{Utilities.GetActivationKeyDisplay(keys.ToList())} (Click to save)";
                };
            }
            else
            {
                model.IsHotKeyActive = true;
                curConf.Keybind = keys.ToList();
                model.HotKeySelectorText = $"[{Utilities.GetActivationKeyDisplay(curConf.Keybind)}] Click to set activation key";
            }
        }

        private string GetKeyDisplay()
        {
            return $"{Utilities.GetActivationKeyDisplay(curConf.Keybind)} (Click to save)";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
            Close();
            var conf = Utilities.GetService<EmojiPadConfiguration>();
            conf.Replace(curConf);
            conf.Save();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void Shutdown()
        {
            model.IsHotKeyActive = true;
            var conf = Utilities.GetService<EmojiPadConfiguration>();
            model.HotKeySelectorText = $"[{Utilities.GetActivationKeyDisplay(conf.Keybind)}] Click to set activation key";
        }
    }
}

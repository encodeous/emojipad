using EmojiPad.Data;
using EmojiPad.Services;
using EmojiPad.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmojiPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public MainWindow()
        {
            var model = Utilities.GetService<EmojiPadModel>();
            DataContext = model;
            Instance = this;
            InitializeComponent();
            var cfg = Utilities.GetService<EmojiPadConfiguration>();
            if (!cfg.ShowOnStart)
            {
                Hide();
            }
            Topmost = cfg.AlwaysOnTop;
            SearchBox.Focus();
            ServiceHost.Instance.AddEmojis();
        }

        private void OnSettingsClick(object sender, MouseButtonEventArgs e)
        {
            var wnd = new Settings();
            wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            wnd.Show();
        }

        private void OnFolderClick(object sender, MouseButtonEventArgs e)
        {
            var cfg = Utilities.GetService<EmojiPadConfiguration>();
            Process.Start(new ProcessStartInfo()
            {
                FileName = cfg.EmojiFolderPath + System.IO.Path.DirectorySeparatorChar,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        
        private void OnRefreshClick(object sender, MouseButtonEventArgs e)
        {
            var cfg = Utilities.GetService<FileService>();
            cfg.Sync();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var cfg = Utilities.GetService<EmojiPadConfiguration>();
            if (cfg.CloseToTray)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            var cfg = Utilities.GetService<EmojiPadConfiguration>();
            if (cfg.HideAfterLostFocus)
            {
                Hide();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Utilities.GetService<EmojiPadModel>().SearchQuery = "";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
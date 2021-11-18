using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using Size = System.Drawing.Size;

namespace EmojiPad.Services
{
    public class WindowService
    {
        public void ShowWindow()
        {
            MainWindow.Instance.Dispatcher.BeginInvoke(() =>
            {
                MainWindow.Instance.Show();
                MainWindow.Instance.Activate();
            });
        }

        public void HideWindow()
        {
            MainWindow.Instance.Dispatcher.BeginInvoke(() =>
            {
                MainWindow.Instance.Hide();
            });
        }

        public void Initialize()
        {
            Task.Run(() =>
            {
                NotifyIcon icon = new NotifyIcon();
                var info = System.Windows.Application.GetResourceStream(new Uri("/wpficon.ico", UriKind.Relative));
                icon.Icon = new Icon(info.Stream);
                icon.Text = "Show EmojiPad";
                var strip = new ContextMenuStrip();
                strip.SuspendLayout();
                strip.Size = new Size(152, 44);
                icon.DoubleClick += (sender, args) =>
                {
                    ShowWindow();
                };
                var item1 = new ToolStripMenuItem("Show EmojiPad");
                item1.Size = new Size(152, 22);
                item1.Click += (sender, args) =>
                {
                    ShowWindow();
                };
                var item2 = new ToolStripMenuItem("Exit EmojiPad");
                item2.Size = new Size(152, 22);
                item2.Click += (sender, args) =>
                {
                    try
                    {
                        Environment.Exit(0);
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
    }
}

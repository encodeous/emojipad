using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmojiPad.Utils;
using GregsStack.InputSimulatorStandard.Native;

namespace EmojiPad
{
    public partial class EmojiPicker : Form
    {
        public static EmojiPicker instance;
        public string Selected = "";
        public EmojiPicker()
        {
            instance = this;
            Controller.OnButtonMouseHover += OnButtonMouseHover;
            Controller.OnButtonMouseDown += OnButtonMouseDown;
            InitializeComponent();
        }

        private void OnButtonMouseDown(string key)
        {
            SelectEmoji(key);
        }

        private void OnButtonMouseHover(string key)
        {
            Selected = key;
            emojiCaption.Text = key;
            emojiPreview.Image = Controller.LoadPreview(key);
        }

        private void EmojiPicker_Load(object sender, EventArgs e)
        {
            RefreshEmojis();
        }
        protected override void OnLoad(EventArgs e)
        {
            UpdateLocation();
            Visible = false;

            base.OnLoad(e);
        }
        private CancellationTokenSource _refreshCancelSource = new CancellationTokenSource();

        public void RefreshEmojis()
        {
            _refreshCancelSource.Cancel();
            _refreshCancelSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
                var ct = _refreshCancelSource.Token;
                await Task.Delay(200, ct);
                if (ct.IsCancellationRequested) return;
                RunActionSafe(() =>
                {
                    emojiBox.SuspendLayout();
                    emojiBox.Controls.Clear();
                    if (searchBar.Text != "")
                    {
                        var images = Controller.FuzzyImageSearch(searchBar.Text);

                        if (images.Length != 0)
                        {
                            Selected = images[0];
                            emojiCaption.Text = images[0];
                            emojiPreview.Image = Controller.LoadPreview(images[0]);
                        }

                        foreach (var img in images)
                        {
                            emojiBox.Controls.Add(Controller.CreateButtonFromImage(img));
                        }
                    }
                    else
                    {
                        var images = Controller.GetImagesSorted();

                        if (images.Length != 0)
                        {
                            Selected = images[0];
                            emojiCaption.Text = images[0];
                            emojiPreview.Image = Controller.LoadPreview(images[0]);
                        }

                        foreach (var img in images)
                        {
                            emojiBox.Controls.Add(Controller.CreateButtonFromImage(img));
                        }
                    }
                    emojiBox.ResumeLayout();
                });
            });
        }

        private delegate void SafeCallDelegate(Action text);

        public static void RunActionSafe(Action act)
        {
            if (instance.InvokeRequired)
            {
                var d = new SafeCallDelegate(RunActionSafe);
                instance.Invoke(d, act);
            }
            else
            {
                act.Invoke();
            }
        }

        private bool _dragging = false;
        private Point _position;

        private void EmojiPicker_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;

            _position = Cursor.Position;
        }

        private void EmojiPicker_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        public void UpdateLocation()
        {
            var pos = Cursor.Position;
            if (pos.X + Width > Screen.GetBounds(pos).Width)
            {
                pos.X = Screen.GetBounds(pos).Width - Width;
            }
            if (pos.Y + Height > Screen.GetBounds(pos).Height)
            {
                pos.Y = Screen.GetBounds(pos).Height - Height;
            }

            Location = pos;
        }

        private void EmojiPicker_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging)
            {
                return;
            }
            var position = Cursor.Position;

            Top += position.Y - _position.Y;
            Left += position.X - _position.X;

            _position = position;
        }

        private void searchBar_TextChanged(object sender, EventArgs e)
        {
            RefreshEmojis();
        }

        public void SelectEmoji(string key)
        {
            if (!Controller.EmojiPadSettings.EmojiFrequency.ContainsKey(EmojiPicker.instance.Selected))
            {
                Controller.EmojiPadSettings.EmojiFrequency[EmojiPicker.instance.Selected] = 0;
            }

            Controller.EmojiPadSettings.EmojiFrequency[EmojiPicker.instance.Selected]++;
            Controller.CopyImg(key);
            Hide();
            Controller.Active = false;
            Controller.InputSimulatorS.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
            Controller.InputSimulatorS.Keyboard.KeyDown(VirtualKeyCode.VK_V);
            Controller.InputSimulatorS.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            Controller.InputSimulatorS.Keyboard.KeyUp(VirtualKeyCode.VK_V);
        }
        private void EmojiPicker_Deactivate(object sender, EventArgs e)
        {
            Hide();
            Controller.Active = false;
        }
    }
}

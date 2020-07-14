using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EmojiPad.Utils
{
    class LazyButton : Button
    {
        public string id = "";
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Image = Extension.GetImageScaled(Controller.ImageMap[id], 32);
            base.OnPaint(pe);
        }
    }
}

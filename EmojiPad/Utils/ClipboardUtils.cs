using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace EmojiPad.Utils
{
    static class ClipboardUtils
    {
        public static bool CopyImage(Image image)
        {
            bool result;

            // http://csharphelper.com/blog/2014/09/copy-an-irregular-area-from-one-picture-to-another-in-c/

            try
            {
                IDataObject data;
                Bitmap opaqueBitmap;
                Bitmap transparentBitmap;
                MemoryStream transparentBitmapStream;

                data = new DataObject();
                opaqueBitmap = null;
                transparentBitmap = null;
                transparentBitmapStream = null;

                try
                {
                    opaqueBitmap = image.Copy(Color.FromArgb(54, 57, 63));
                    transparentBitmap = image.Copy(Color.Transparent);

                    transparentBitmapStream = new MemoryStream();
                    transparentBitmap.Save(transparentBitmapStream, ImageFormat.Png);

                    data.SetData(DataFormats.Bitmap, opaqueBitmap);
                    data.SetData("PNG", false, transparentBitmapStream);

                    Clipboard.Clear();
                    Clipboard.SetDataObject(data, true);
                }
                finally
                {
                    opaqueBitmap?.Dispose();
                    transparentBitmapStream?.Dispose();
                    transparentBitmap?.Dispose();
                }

                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy image. {ex.GetBaseException().Message}", "Copy Image", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                result = false;
            }

            return result;
        }
        public static Bitmap Copy(this Image image, Color transparentColor)
        {
            Bitmap copy;

            copy = new Bitmap(image.Size.Width, image.Size.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(copy))
            {
                g.Clear(transparentColor);
                g.PageUnit = GraphicsUnit.Pixel;
                g.DrawImage(image, new Rectangle(Point.Empty, image.Size));
            }

            return copy;
        }
    }
}

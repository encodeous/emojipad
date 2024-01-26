using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppUIBasics.Helper;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using WinRT;
using System.Runtime.InteropServices;
using Windows.Foundation;
using AppUIBasics;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Media;

namespace WinUIGallery.DesktopWap.Helper
{

    internal class TitleBarHelper
    {
        public static void SetCaptionButtonColors(Window window, Windows.UI.Color color)
        {
            var res = Application.Current.Resources;
            res["WindowCaptionForeground"] = color;
            window.AppWindow.TitleBar.ButtonForegroundColor = color;
        }

        public static void SetCaptionButtonBackgroundColors(Window window, Windows.UI.Color? color)
        {
            var titleBar = window.AppWindow.TitleBar;
            titleBar.ButtonBackgroundColor = color;
        }

        public static void SetForegroundColor(Window window, Windows.UI.Color? color)
        {
            var titleBar = window.AppWindow.TitleBar;
            titleBar.ForegroundColor = color;
        }

        public static void SetBackgroundColor(Window window, Windows.UI.Color? color)
        {
            var titleBar = window.AppWindow.TitleBar;
            titleBar.BackgroundColor = color;
        }

        public static void SetClickThrough(Window wnd, FrameworkElement element)
        {
            var window = WindowHelper.GetAppWindow(wnd);
            var nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(window.Id);
            GeneralTransform transformTxtBox = element.TransformToVisual(null);
            Rect bounds = transformTxtBox.TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));

            // Windows.Graphics.RectInt32[] rects defines the area which allows click throughs in custom titlebar
            // it is non dpi-aware client coordinates. Hence, we convert dpi aware coordinates to non-dpi coordinates
            var scale = wnd.Content.XamlRoot.RasterizationScale;
            var transparentRect = new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scale),
                _Y: (int)Math.Round(bounds.Y * scale),
                _Width: (int)Math.Round(bounds.Width * scale),
                _Height: (int)Math.Round(bounds.Height * scale)
            );
            var rects = new Windows.Graphics.RectInt32[] { transparentRect };

            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rects);
        }
    }
}

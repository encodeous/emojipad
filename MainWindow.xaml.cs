using emojipad.Utils;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.WindowManagement;
using AppUIBasics.Helper;
using WinRT.Interop;
using WinUIEx;
using WinUIEx.Messaging;
using WinUIGallery.DesktopWap.Helper;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace emojipad
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            var monitor = new WindowMessageMonitor(this);
            monitor.WindowMessageReceived += Monitor_WindowMessageReceived;
            var root = Content as FrameworkElement;
            root.Loaded += Root_Loaded;
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            Activate();
            TitleBarHelper.SetClickThrough(this, SettingsButton);
        }

        private void Monitor_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;
            const int WM_NCLBUTTONDBLCLK = 0x00A3; //double click on a title bar a.k.a. non-client area of the form

            switch (e.Message.MessageId)
            {
                case WM_SYSCOMMAND:             //preventing the form from being moved by the mouse.
                    int command = (int)(e.Message.WParam & 0xfff0);
                    if (command == SC_MOVE)
                        return;
                    break;
            }

            if (e.Message.MessageId == WM_NCLBUTTONDBLCLK)       //preventing the form being resized by the mouse double click on the title bar.
            {
                e.Handled = true;
                return;
            }
        }


        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            //myButton.Content = "Clicked";
        }
    }
}

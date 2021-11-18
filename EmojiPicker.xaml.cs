using EmojiPad.Data;
using EmojiPad.Services;
using EmojiPad.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmojiPad
{
    /// <summary>
    /// Interaction logic for EmojiPicker.xaml
    /// </summary>
    public partial class EmojiPicker : UserControl
    {
        public EmojiPicker()
        {
            var model = Utilities.GetService<EmojiPadModel>();
            DataContext = model;
            InitializeComponent();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            var model = Utilities.GetService<EmojiPadModel>();
            model.SelectedEmoji = (Emoji)((Image)sender).Tag;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var img = (Emoji)((Image)sender).Tag;
            Utilities.GetService<EventService>().CopyEmoji(img);
        }
    }

}

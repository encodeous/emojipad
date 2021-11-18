using EmojiPad.Services;
using EmojiPad.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace EmojiPad.Data
{
    [ValueConversion(typeof(Emoji), typeof(string))]
    public class EmojiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Emoji) return "";
            Emoji emoji = (Emoji)value;
            if (emoji == null) return "";
            var file = Utilities.GetService<FileService>();
            file.CacheEmoji(emoji);
            return Utilities.GetCachedPath(emoji);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}

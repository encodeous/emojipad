using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EmojiPad.Utils
{
    internal class NumericValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object val, CultureInfo cultureInfo)
        {
            if(val is not string) return new ValidationResult(false, "");
            if(int.TryParse(val.ToString(), out var i))
            {
                if(i < 200 && i > 0) return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "");
        }
    }
}

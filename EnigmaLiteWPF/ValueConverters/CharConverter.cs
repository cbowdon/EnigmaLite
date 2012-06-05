using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace EnigmaLiteWPF.ValueConverters
{
    /// <summary>
    /// Convert from chars to strings suitable for displaying in the key.
    /// TODO: unit tests for this
    /// </summary>
    [ValueConversion(typeof(char), typeof(string))]
    public class CharConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (char)value;
            var i = (int)c;
            if (i < 31)
            {
                return string.Format("0x{0}", i.ToString("X"));
            }
            else
            {
                return string.Format("{0}", c);
            }            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (string)value;
            var ca = s.ToCharArray();
            char c;
            if (ca[0] == '0' && ca[1] == 'x')
            {
                c = (char)int.Parse(s.Remove(0,2).ToLower(), System.Globalization.NumberStyles.HexNumber);
            } else 
            {
                c = ca[0];
            }                
            return c;
        }
    }
}

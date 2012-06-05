using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace EnigmaLiteWPF.ValueConverters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class ScoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var score = (double)value;
            return String.Format("Solution Score: {0:0}%", 100*score);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // not required
            return DependencyProperty.UnsetValue;
        }
    }
}

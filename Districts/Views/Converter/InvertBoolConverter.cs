using System;
using System.Globalization;
using System.Windows.Data;

namespace Districts.Views.Converter
{
    internal class InvertBoolConverter : IValueConverter
    {
        public bool AsserNull { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool param) return !param;

            if (AsserNull)
                return null;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
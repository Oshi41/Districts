using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Districts.Views.Converter
{
    class InverseBoolToVisConverter : IValueConverter
    {
        public bool IsHidden { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                return val ? GetFalseValue() : Visibility.Visible;
            }

            return GetFalseValue();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Visibility GetFalseValue()
        {
            return IsHidden ? Visibility.Hidden : Visibility.Collapsed;
        }
    }
}

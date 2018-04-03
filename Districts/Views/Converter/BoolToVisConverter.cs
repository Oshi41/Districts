using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Districts.Views.Converter
{
    internal class BoolToVisConverter : IValueConverter
    {
        public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true.Equals(value)
                ? Visibility.Visible
                : FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Visible.Equals(value);
        }
    }
}
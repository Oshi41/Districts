using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Districts.Views.Converter
{
    class MathConverter : IValueConverter
    {
        public double Divide { get; set; } = 1;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(), out var result))
            {
                return result / Divide;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

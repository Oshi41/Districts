using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Districts.Views.Controls
{
    internal class EnumToDescrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var fi = value.GetType().GetField(value.ToString());
                if (fi != null)
                {
                    var attributes =
                        (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    return attributes.Length > 0 && !string.IsNullOrEmpty(attributes[0].Description)
                        ? attributes[0].Description
                        : value.ToString();
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
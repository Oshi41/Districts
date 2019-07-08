using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DistrictsLib.Implementation.Printing.WPF
{
    internal class ListStringConverter : IValueConverter
    {
        private const string _delimeter = ",\n";

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<string> result)
            {
                return string.Join(_delimeter, result);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string raw)
            {
                var aplitted = raw.Split(new[] { _delimeter }, StringSplitOptions.RemoveEmptyEntries);
                return aplitted.ToList();
            }

            return Binding.DoNothing;
        }

        #endregion
    }
}

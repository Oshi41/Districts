using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DistrictsNew.WpfConverters
{
    class MultiBoolConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.All(x => Equals(x, true));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

using System;
using System.Globalization;
using System.Windows.Data;
using Districts.JsonClasses;

namespace Districts.Views.Converter
{
    class DoorStrComparer : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Door door)
            {
                return $"{door.Street, -25} " +
                       $"{door.HouseNumber, -6} " +
                       $"{door.Number, -3} " +
                       $"{string.Join(", ", door.Codes)}";
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

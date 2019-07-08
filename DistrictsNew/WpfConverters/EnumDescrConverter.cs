using System;
using System.Globalization;
using System.Windows.Data;
using DistrictsLib.Legacy.JsonClasses.Manage;
using DistrictsNew.Extensions;

namespace DistrictsNew.WpfConverters
{
    class EnumDescrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ActionTypes type)
            {
                return type.Description();
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

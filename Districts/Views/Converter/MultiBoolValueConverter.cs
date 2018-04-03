using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Districts.Views.Converter
{
    public class MultiBoolValueConverter : IMultiValueConverter
    {
        #region Nested

        public enum ConverterTypes
        {
            AllTrue,
            AllFalse,
            AnyTrue,
            AnyFalse
        }

        #endregion

        public ConverterTypes ConverterType { get; set; } = ConverterTypes.AllTrue;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var boolVals = values.OfType<bool>();
            switch (ConverterType)
            {
                case ConverterTypes.AllTrue:
                {
                    var allTrue = boolVals.All(x => x);
                    if (allTrue) return true;

                    var allFalse = boolVals.All(x => !x);
                    return allFalse
                        ? false
                        : (object) null;
                }

                case ConverterTypes.AllFalse:
                {
                    var allFalse = boolVals.All(x => !x);
                    if (allFalse) return true;

                    var allTrue = boolVals.All(x => x);
                    return allTrue
                        ? false
                        : (object) null;
                }

                case ConverterTypes.AnyTrue:
                    return boolVals.Any(x => x);

                case ConverterTypes.AnyFalse:
                    return boolVals.Any(x => !x);

                default:
                    return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                var toReturn = new object[targetTypes.Length];
                for (var i = 0; i < toReturn.Length; i++) toReturn[i] = val;

                return toReturn;
            }

            return null;
        }
    }
}
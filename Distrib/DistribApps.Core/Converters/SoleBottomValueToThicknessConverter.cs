using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DistribApps.Core.Converters
{
    /// <summary>
    /// Converts a numerical value to a thickness where only the bottom component is set to the value
    /// </summary>
    public sealed class SoleBottomValueToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(0, 0, 0, System.Convert.ToDouble(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

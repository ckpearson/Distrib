using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProcessRunner.Converters
{
    public sealed class BoolToBusyProgressStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var b = (bool)value;
            if (b)
            {
                return Elysium.Controls.ProgressState.Indeterminate;
            }
            else
            {
                return Elysium.Controls.ProgressState.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

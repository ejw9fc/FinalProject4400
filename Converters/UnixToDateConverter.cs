using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FinalProject4400.Converters
{
    public class UnixToDateConverter : IValueConverter
    {
        /// <summary>
        /// // Converts a UNIX timestamp (in seconds) to a local DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long unix)
                // Create DateTime from UNIX seconds and convert to local time
                return DateTimeOffset
                       .FromUnixTimeSeconds(unix)
                       .DateTime
                       .ToLocalTime();
            // If the value isn't a long, do nothing
            return Binding.DoNothing;
        }
        // Not implemented for one-way bindings
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp36.Infrastructure
{
    public class TomorrowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
                return date.Date == DateTime.Now.Date.AddDays(1);
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp36.Infrastructure
{
    public class OverdueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
                return date.Date < DateTime.Now.Date;
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
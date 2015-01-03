using System;
using System.Windows;
using System.Windows.Data;

namespace MoveFilesUpOneLevel.ServiceClasses
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result;
            try
            {
                result = (bool)value;
            }
            catch
            {
                result = false;
            }
            if (result)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

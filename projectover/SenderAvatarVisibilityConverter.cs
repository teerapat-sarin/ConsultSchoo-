using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace projectover
{
    public class SenderAvatarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // แสดง avatar เฉพาะฝั่งซ้าย (Align = Left)
            if (value is string align && align.Equals("Left", StringComparison.OrdinalIgnoreCase))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace projectover
{
    public class AlignToColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ถ้า Align เป็น "Right" -> ใช้ Column 1, ถ้าเป็น "Left" -> Column 0
            if (value is string align && align.Equals("Right", StringComparison.OrdinalIgnoreCase))
                return 1;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

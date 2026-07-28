using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RunTrackerOverlay.Converters
{
    public class OpacityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double opacity)
            {
                byte alpha = (byte)(opacity * 255);
                if (alpha == 0) alpha = 1;
                return new SolidColorBrush(Color.FromArgb(alpha, 0, 0, 0));
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

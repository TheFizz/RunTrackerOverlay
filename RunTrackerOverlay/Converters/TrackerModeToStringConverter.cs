using System;
using System.Globalization;
using System.Windows.Data;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Converters
{
    public class TrackerModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TrackerMode mode)
            {
                return mode switch
                {
                    TrackerMode.Continuous => "Continuous",
                    TrackerMode.StartStop => "Start/Stop Mode",
                    TrackerMode.D2RAuto => "D2R Auto",
                    _ => mode.ToString()
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

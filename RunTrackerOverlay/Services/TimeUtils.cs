using System;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Services
{
    public static class TimeUtils
    {
        public static string FormatTime(TimeSpan ts, string format)
        {
            if (ts.TotalDays >= 1)
            {
                return format == Constants.TimeFormatNoMs ? Constants.TimeLimitTextNoMs : Constants.TimeLimitTextStandard;
            }

            try
            {
                return ts.ToString(format);
            }
            catch
            {
                // Fallback to standard format if the provided format is invalid
                return ts.ToString(Constants.TimeFormatStandard);
            }
        }

        public static bool IsValidFormat(string format)
        {
            try
            {
                TimeSpan.Zero.ToString(format);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

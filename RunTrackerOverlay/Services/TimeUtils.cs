using System;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Services
{
    public static class TimeUtils
    {
        public static string FormatTime(TimeSpan ts, bool hideMilliseconds)
        {
            if (ts == TimeSpan.Zero || ts.TotalDays >= 1)
            {
                if (ts.TotalDays >= 1)
                {
                    return hideMilliseconds ? Constants.TimeLimitTextNoMs : Constants.TimeLimitTextStandard;
                }
                return hideMilliseconds ? Constants.ZeroTimeTextNoMs : Constants.ZeroTimeTextStandard;
            }

            return hideMilliseconds 
                ? ts.ToString(Constants.TimeFormatNoMs) 
                : ts.ToString(Constants.TimeFormatStandard);
        }
    }
}

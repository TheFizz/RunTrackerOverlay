using System;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Services
{
    public interface IDisplayService
    {
        string GetTimerText(TimeSpan elapsed, string format);
        string GetRunStatusSymbol(bool isRunning, bool isPaused, int runCount);
        string GetRunLabelText(bool isRunning, int runCount, bool showRunCount);
        string GetRunStatusForeground(bool isRunning, bool isPaused, int runCount);
        string GetTimerForeground(bool isRunning, bool isPaused, int runCount);
        string GetBackgroundBrush(double windowOpacity);
        string GetTooltipText(AppSettings settings);
    }

    public class DisplayService : IDisplayService
    {
        public string GetTimerText(TimeSpan elapsed, string format)
        {
            return TimeUtils.FormatTime(elapsed, format);
        }

        public string GetRunStatusSymbol(bool isRunning, bool isPaused, int runCount)
        {
            if (isRunning)
            {
                return isPaused ? "⧖" : "▶";
            }
            return runCount == 0 ? "■" : "✔︎";
        }

        public string GetRunLabelText(bool isRunning, int runCount, bool showRunCount)
        {
            string runNum;
            if (isRunning)
            {
                runNum = showRunCount ? runCount.ToString() : "???";
            }
            else
            {
                runNum = showRunCount ? (runCount == 0 ? "1" : runCount.ToString()) : "???";
            }
            return $" Run #{runNum}";
        }

        public string GetRunStatusForeground(bool isRunning, bool isPaused, int runCount)
        {
            if (isRunning)
            {
                return isPaused ? "Orange" : "Lime";
            }
            return runCount == 0 ? "LightSteelBlue" : "Green";
        }

        public string GetTimerForeground(bool isRunning, bool isPaused, int runCount)
        {
            if (isRunning)
            {
                return isPaused ? "Orange" : "Lime";
            }
            return runCount == 0 ? "White" : "Green";
        }

        public string GetBackgroundBrush(double windowOpacity)
        {
            byte alpha = (byte)(windowOpacity * 255);
            if (alpha == 0) alpha = 1;
            return string.Format("#{0:X2}000000", alpha);
        }

        public string GetTooltipText(AppSettings settings)
        {
            if (settings.Mode == TrackerMode.Continuous)
            {
                return $"Start/Next: {settings.StartStopNextKey}\nStop (Cont.): {settings.StopContKey}\nPause/Resume: {settings.PauseResumeKey}\nRecord Loot: {settings.LootKey}\nFocus: {settings.FocusKey}";
            }
            else if (settings.Mode == TrackerMode.D2RAuto)
            {
                return $"Start/Stop Auto: {settings.StartStopNextKey}\nPause/Resume: {settings.PauseResumeKey}\nRecord Loot: {settings.LootKey}\nFocus: {settings.FocusKey}";
            }
            else
            {
                return $"Start/Stop: {settings.StartStopNextKey}\nPause/Resume: {settings.PauseResumeKey}\nRecord Loot: {settings.LootKey}\nFocus: {settings.FocusKey}";
            }
        }
    }
}

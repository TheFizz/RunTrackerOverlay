using System;
using System.Windows.Input;

namespace RunTrackerOverlay.Models
{
    public class AppSettings
    {
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;
        public Key StartStopNextKey { get; set; } = Key.F5;
        public Key StopContKey { get; set; } = Key.F6;
        public Key PauseResumeKey { get; set; } = Key.F7;
        public Key FocusKey { get; set; } = Key.F8;
        public Key LootKey { get; set; } = Key.F9;
        public bool ShowKeysTooltip { get; set; } = true;
        public bool IsSnappingEnabled { get; set; } = true;
        public TrackerMode Mode { get; set; } = TrackerMode.Continuous;
        public double WindowOpacity { get; set; } = 0.5;
        public double TextOpacity { get; set; } = 1.0;
        public bool ShowBest { get; set; } = true;
        public bool ShowLast { get; set; } = true;
        public bool ShowWorst { get; set; } = true;
        public bool ShowAvg { get; set; } = true;
        public bool ShowTotal { get; set; } = true;
        public bool ShowLoot { get; set; } = true;
        public string TimerFormat { get; set; } = Constants.TimeFormatStandard;
        public bool ApplyFormatToStats { get; set; } = false;
        public bool ShowSessionName { get; set; } = true;
        public bool ShowRunCount { get; set; } = true;

        public void CopyFrom(AppSettings other)
        {
            this.WindowLeft = other.WindowLeft;
            this.WindowTop = other.WindowTop;
            this.StartStopNextKey = other.StartStopNextKey;
            this.FocusKey = other.FocusKey;
            this.StopContKey = other.StopContKey;
            this.PauseResumeKey = other.PauseResumeKey;
            this.LootKey = other.LootKey;
            this.ShowKeysTooltip = other.ShowKeysTooltip;
            this.IsSnappingEnabled = other.IsSnappingEnabled;
            this.Mode = other.Mode;
            this.WindowOpacity = other.WindowOpacity;
            this.TextOpacity = other.TextOpacity;
            this.ShowBest = other.ShowBest;
            this.ShowLast = other.ShowLast;
            this.ShowWorst = other.ShowWorst;
            this.ShowAvg = other.ShowAvg;
            this.ShowTotal = other.ShowTotal;
            this.ShowLoot = other.ShowLoot;
            this.TimerFormat = other.TimerFormat;
            this.ApplyFormatToStats = other.ApplyFormatToStats;
            this.ShowSessionName = other.ShowSessionName;
            this.ShowRunCount = other.ShowRunCount;
        }
    }
}

using System;
using System.Windows.Input;

namespace RunTrackerOverlay.Models
{
    public class AppSettings
    {
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;
        public Key ActivationKey { get; set; } = Key.Pause;
        public Key FocusKey { get; set; } = Key.F8;
        public Key PauseKey { get; set; } = Key.F9;
        public Key LootKey { get; set; } = Key.PageUp;
        public bool ShowKeysTooltip { get; set; } = true;
        public bool IsSnappingEnabled { get; set; } = true;
        public bool IsContinuousMode { get; set; } = false;
        public double WindowOpacity { get; set; } = 0.5;
        public double TextOpacity { get; set; } = 1.0;
        public bool ShowBest { get; set; } = true;
        public bool ShowLast { get; set; } = true;
        public bool ShowWorst { get; set; } = true;
        public bool ShowAvg { get; set; } = true;
        public bool ShowTotal { get; set; } = true;
        public bool HideMilliseconds { get; set; } = false;
        public string SessionName { get; set; } = "Session 1";
        public bool ShowSessionName { get; set; } = true;

        public void CopyFrom(AppSettings other)
        {
            this.WindowLeft = other.WindowLeft;
            this.WindowTop = other.WindowTop;
            this.ActivationKey = other.ActivationKey;
            this.FocusKey = other.FocusKey;
            this.PauseKey = other.PauseKey;
            this.LootKey = other.LootKey;
            this.ShowKeysTooltip = other.ShowKeysTooltip;
            this.IsSnappingEnabled = other.IsSnappingEnabled;
            this.IsContinuousMode = other.IsContinuousMode;
            this.WindowOpacity = other.WindowOpacity;
            this.TextOpacity = other.TextOpacity;
            this.ShowBest = other.ShowBest;
            this.ShowLast = other.ShowLast;
            this.ShowWorst = other.ShowWorst;
            this.ShowAvg = other.ShowAvg;
            this.ShowTotal = other.ShowTotal;
            this.HideMilliseconds = other.HideMilliseconds;
            this.SessionName = other.SessionName;
            this.ShowSessionName = other.ShowSessionName;
        }
    }
}

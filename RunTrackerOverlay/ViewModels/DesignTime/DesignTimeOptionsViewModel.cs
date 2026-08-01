using System;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.ViewModels.DesignTime
{
    public class DesignTimeOptionsViewModel
    {
        public DesignTimeOptionsViewModel()
        {
            IsSnappingEnabled = true;
            ShowKeysTooltip = true;
            ShowSessionName = true;
            SessionName = "My Awesome Session";
            Mode = TrackerMode.Continuous;
            TimerFormat = Constants.TimeFormatStandard;
            ApplyFormatToStats = false;
            ShowBest = true;
            ShowLast = true;
            ShowWorst = true;
            ShowAvg = true;
            ShowTotal = true;
            ShowLoot = true;
            StartStopNextKeyText = "F5";
            StopContKeyText = "F6";
            PauseResumeKeyText = "F7";
            FocusKeyText = "F8";
            LootKeyText = "F9";
            WindowOpacity = 0.5;
            TextOpacity = 1.0;
        }

        public bool IsSnappingEnabled { get; set; }
        public bool ShowKeysTooltip { get; set; }
        public bool ShowSessionName { get; set; }
        public string SessionName { get; set; }
        public TrackerMode Mode { get; set; }
        public TrackerMode[] AllModes => (TrackerMode[])Enum.GetValues(typeof(TrackerMode));
        public string TimerFormat { get; set; }
        public bool IsTimerFormatInvalid { get; set; }
        public bool ApplyFormatToStats { get; set; }
        
        public bool ShowBest { get; set; }
        public bool ShowLast { get; set; }
        public bool ShowWorst { get; set; }
        public bool ShowAvg { get; set; }
        public bool ShowTotal { get; set; }
        public bool ShowLoot { get; set; }

        public string StartStopNextKeyText { get; set; }
        public string StopContKeyText { get; set; }
        public string PauseResumeKeyText { get; set; }
        public string FocusKeyText { get; set; }
        public string LootKeyText { get; set; }
        
        public double WindowOpacity { get; set; }
        public double TextOpacity { get; set; }
    }
}

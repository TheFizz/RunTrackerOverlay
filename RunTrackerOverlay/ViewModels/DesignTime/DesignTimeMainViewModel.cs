using System.Collections.Generic;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.ViewModels.DesignTime
{
    public class DesignTimeMainViewModel
    {
        public DesignTimeMainViewModel()
        {
            TimerText = "00:12.34";
            RunStatusSymbol = "⧖";
            RunLabelText = "Run #5";
            BestTimeText = "00:10.50";
            WorstTimeText = "00:25.00";
            LastRunTimeText = "00:15.20";
            AverageTimeText = "00:14.00";
            TotalTimeText = "01:10.00";
            LootCountText = "3";
            SessionName = "Session 1";
            ShowSessionName = true;
            TooltipText = "F8: Focus | F9: Stop | PgUp: Loot";
            TimerForeground = System.Windows.Media.Brushes.White;
            RunStatusForeground = System.Windows.Media.Brushes.LimeGreen;
            BackgroundBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(128, 0, 0, 0));
            TextOpacity = 1.0;
            ShowTooltip = true;
            ShowBest = true;
            ShowLast = true;
            ShowWorst = true;
            ShowAvg = true;
            ShowTotal = true;
            ShowLoot = true;
            IsActive = true;
            WindowLeft = 100;
            WindowTop = 100;
        }

        public string TimerText { get; set; }
        public string RunStatusSymbol { get; set; }
        public string RunLabelText { get; set; }
        public string BestTimeText { get; set; }
        public string WorstTimeText { get; set; }
        public string LastRunTimeText { get; set; }
        public string AverageTimeText { get; set; }
        public string TotalTimeText { get; set; }
        public string LootCountText { get; set; }
        public string SessionName { get; set; }
        public bool ShowSessionName { get; set; }
        public bool ShowSessionNameVisible => true;
        public string TooltipText { get; set; }
        public object TimerForeground { get; set; }
        public object RunStatusForeground { get; set; }
        public object BackgroundBrush { get; set; }
        public double TextOpacity { get; set; }
        public bool ShowTooltip { get; set; }
        public bool ShowBest { get; set; }
        public bool ShowLast { get; set; }
        public bool ShowWorst { get; set; }
        public bool ShowAvg { get; set; }
        public bool ShowTotal { get; set; }
        public bool ShowLoot { get; set; }
        public bool ShowStats => true;
        public bool IsActive { get; set; }
        public double WindowLeft { get; set; }
        public double WindowTop { get; set; }

        public System.Windows.Input.ICommand SaveCommand => null!;
        public System.Windows.Input.ICommand ResetCommand => null!;
        public System.Windows.Input.ICommand OptionsCommand => null!;
        public System.Windows.Input.ICommand CloseCommand => null!;
    }
}

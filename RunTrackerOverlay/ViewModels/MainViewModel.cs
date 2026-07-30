using System;
using System.Windows.Input;

using RunTrackerOverlay.Models;
using RunTrackerOverlay.Services;

namespace RunTrackerOverlay.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly TimerEngine _timerEngine;
        private readonly StatisticsTracker _statsTracker = new StatisticsTracker();
        private readonly AppSettings _settings;
        private readonly ISessionLogger _sessionLogger;
        private readonly IReportExporter _reportExporter;
        private string _timerText = "00:00.00";
        private string _runStatusSymbol = "■";
        private string _runLabelText = " Run #1";
        private string _bestTimeText = "00:00.00";
        private string _worstTimeText = "00:00.00";
        private string _lastRunTimeText = "00:00.00";
        private string _averageTimeText = "00:00.00";
        private string _totalTimeText = "00:00.00";
        private string _sessionName = "Session 1";
        private string _tooltipText = "";
        private object _timerForeground = "White";
        private object _runStatusForeground = "LightSteelBlue";
        private object _backgroundBrush = null!;
        private double _textOpacity = 1.0;
        private bool _showTooltip;
        private bool _showBest = true;
        private bool _showLast = true;
        private bool _showWorst = true;
        private bool _showAvg = true;
        private bool _showTotal = true;
        private bool _showSessionName = true;
        private bool _isActive;
        private bool _hideMilliseconds;
        private bool _isDirty;
        private bool _isDialogOpen;

        public Func<string, (bool? result, bool? includeEmpty)>? RequestSaveDialog { get; set; }
        public Func<string, string, bool?>? RequestConfirmation { get; set; }
        public Func<bool?>? RequestOptionsDialog { get; set; }
        public Action<string, string>? ShowMessage { get; set; }
        public Action<string, string>? ShowError { get; set; }
        public Action? RequestClose { get; set; }

        public ICommand SaveCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand OptionsCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        
        public MainViewModel(TimerEngine timerEngine, AppSettings settings, ISessionLogger sessionLogger, IReportExporter reportExporter)
        {
            _timerEngine = timerEngine;
            _settings = settings;
            _sessionLogger = sessionLogger;
            _reportExporter = reportExporter;
            
            _timerEngine.StateChanged += () => UpdateDisplay();
            _timerEngine.RunCompleted += (count, duration, loot) =>
            {
                _statsTracker.AddRun(duration);
                _sessionLogger.AppendRun(count, duration, loot);
                IsDirty = true;
                UpdateDisplay();
            };
            _timerEngine.LootAddedToLastRun += (loot) =>
            {
                _sessionLogger.UpdateLastRunLoot(loot);
                IsDirty = true;
                UpdateDisplay();
            };

            SaveCommand = new RelayCommand(_ => ExecuteSave());
            ResetCommand = new RelayCommand(_ => ExecuteReset());
            OptionsCommand = new RelayCommand(_ => ExecuteOptions());
            CloseCommand = new RelayCommand(_ => RequestClose?.Invoke());

            UpdateDisplay();
            UpdateTooltip();
            UpdateVisuals();
        }

        public string TimerText { get => _timerText; set => SetProperty(ref _timerText, value); }
        public string RunStatusSymbol { get => _runStatusSymbol; set => SetProperty(ref _runStatusSymbol, value); }
        public string RunLabelText { get => _runLabelText; set => SetProperty(ref _runLabelText, value); }
        public string BestTimeText { get => _bestTimeText; set => SetProperty(ref _bestTimeText, value); }
        public string WorstTimeText { get => _worstTimeText; set => SetProperty(ref _worstTimeText, value); }
        public string LastRunTimeText { get => _lastRunTimeText; set => SetProperty(ref _lastRunTimeText, value); }
        public string AverageTimeText { get => _averageTimeText; set => SetProperty(ref _averageTimeText, value); }
        public string TotalTimeText { get => _totalTimeText; set => SetProperty(ref _totalTimeText, value); }
        public string SessionName 
        { 
            get => _sessionName; 
            set 
            {
                if (SetProperty(ref _sessionName, value))
                {
                    OnPropertyChanged(nameof(ShowSessionNameVisible));
                }
            } 
        }

        public bool ShowSessionName
        {
            get => _showSessionName;
            set
            {
                if (SetProperty(ref _showSessionName, value))
                {
                    OnPropertyChanged(nameof(ShowSessionNameVisible));
                }
            }
        }

        public bool ShowSessionNameVisible => ShowSessionName && !string.IsNullOrWhiteSpace(SessionName);
        public string TooltipText { get => _tooltipText; set => SetProperty(ref _tooltipText, value); }
        public object TimerForeground { get => _timerForeground; set => SetProperty(ref _timerForeground, value); }
        public object RunStatusForeground { get => _runStatusForeground; set => SetProperty(ref _runStatusForeground, value); }
        public object BackgroundBrush { get => _backgroundBrush; set => SetProperty(ref _backgroundBrush, value); }
        public double TextOpacity { get => _textOpacity; set => SetProperty(ref _textOpacity, value); }
        public bool ShowTooltip { get => _showTooltip; set => SetProperty(ref _showTooltip, value); }
        public bool ShowBest 
        { 
            get => _showBest; 
            set 
            { 
                if (SetProperty(ref _showBest, value))
                    OnPropertyChanged(nameof(ShowStats));
            } 
        }
        public bool ShowLast 
        { 
            get => _showLast; 
            set 
            { 
                if (SetProperty(ref _showLast, value))
                    OnPropertyChanged(nameof(ShowStats));
            } 
        }
        public bool ShowWorst 
        { 
            get => _showWorst; 
            set 
            { 
                if (SetProperty(ref _showWorst, value))
                    OnPropertyChanged(nameof(ShowStats));
            } 
        }
        public bool ShowAvg 
        { 
            get => _showAvg; 
            set 
            { 
                if (SetProperty(ref _showAvg, value))
                    OnPropertyChanged(nameof(ShowStats));
            } 
        }
        public bool ShowTotal 
        { 
            get => _showTotal; 
            set 
            { 
                if (SetProperty(ref _showTotal, value))
                    OnPropertyChanged(nameof(ShowStats));
            } 
        }

        public bool ShowStats => ShowBest || ShowLast || ShowWorst || ShowAvg || ShowTotal;

        public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
        public bool IsDirty { get => _isDirty; set => SetProperty(ref _isDirty, value); }
        public bool IsDialogOpen { get => _isDialogOpen; set => SetProperty(ref _isDialogOpen, value); }
        public bool HideMilliseconds
        {
            get => _hideMilliseconds;
            set
            {
                if (SetProperty(ref _hideMilliseconds, value))
                {
                    UpdateDisplay();
                    UpdateTooltip();
                }
            }
        }

        // WindowWidth property removed

        private void TimerEngine_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentLoot" || e.PropertyName == "RunCount" || e.PropertyName == "TotalTime" || e.PropertyName == "LastRunLoot")
            {
                IsDirty = true;
            }
            UpdateDisplay();
        }

        public void UpdateVisuals()
        {
            byte alpha = (byte)(_settings.WindowOpacity * 255);
            if (alpha == 0) alpha = 1;
            BackgroundBrush = string.Format("#{0:X2}000000", alpha);
            TextOpacity = _settings.TextOpacity;
            ShowBest = _settings.ShowBest;
            ShowLast = _settings.ShowLast;
            ShowWorst = _settings.ShowWorst;
            ShowAvg = _settings.ShowAvg;
            ShowTotal = _settings.ShowTotal;
            ShowSessionName = _settings.ShowSessionName;
            HideMilliseconds = _settings.HideMilliseconds;
            SessionName = _settings.SessionName;
        }

        public void UpdateTooltip()
        {
            if (_settings.IsContinuousMode)
            {
                TooltipText = $"Run: {_settings.ActivationKey}\nStop: {_settings.PauseKey}\nLoot: {_settings.LootKey}\nFocus: {_settings.FocusKey}";
            }
            else
            {
                TooltipText = $"Run/Stop: {_settings.ActivationKey}\nLoot: {_settings.LootKey}\nFocus: {_settings.FocusKey}";
            }
            ShowTooltip = _settings.ShowKeysTooltip;
        }

        public void UpdateDisplay()
        {
            TimeSpan elapsed = _timerEngine.CurrentElapsed;
            TimerText = TimeUtils.FormatTime(elapsed, HideMilliseconds);

            if (_timerEngine.IsRunning)
            {
                RunStatusSymbol = "▶";
                RunLabelText = $" Run #{_timerEngine.RunCount}";
                RunStatusForeground = "Lime";
                TimerForeground = "Lime";
            }
            else
            {
                if (_timerEngine.RunCount == 0)
                {
                    RunStatusSymbol = "■";
                    RunLabelText = " Run #1";
                    RunStatusForeground = "LightSteelBlue";
                    TimerForeground = "White";
                }
                else
                {
                    RunStatusSymbol = "✔︎";
                    RunLabelText = $" Run #{_timerEngine.RunCount}";
                    RunStatusForeground = "Green";
                    TimerForeground = "Green";
                }
            }

            BestTimeText = TimeUtils.FormatTime(_statsTracker.BestTime, HideMilliseconds);
            WorstTimeText = TimeUtils.FormatTime(_statsTracker.WorstTime, HideMilliseconds);
            LastRunTimeText = TimeUtils.FormatTime(_timerEngine.LastRunTime, HideMilliseconds);
            
            AverageTimeText = TimeUtils.FormatTime(_statsTracker.RunCount > 0 
                ? TimeSpan.FromTicks(_statsTracker.TotalTime.Ticks / _statsTracker.RunCount) 
                : TimeSpan.Zero, HideMilliseconds);
            TotalTimeText = TimeUtils.FormatTime(_statsTracker.TotalTime, HideMilliseconds);
        }

        private void ExecuteReset()
        {
            IsDialogOpen = true;
            try
            {
                if (RequestConfirmation?.Invoke("Are you sure you want to reset the current session? This will clear all loot and times.", "Reset Session") == true)
                {
                    _timerEngine.Reset();
                    _statsTracker.Reset();
                    _sessionLogger.InitializeSession(SessionName, 0);
                    IsDirty = false;
                }
            }
            finally
            {
                IsDialogOpen = false;
            }
        }

        public Action? SettingsChanged;
        public Action? OptionsRequested;
        public Action? OptionsClosed;

        private void ExecuteOptions()
        {
            IsDialogOpen = true;
            try
            {
                OptionsRequested?.Invoke();
                if (RequestOptionsDialog?.Invoke() == true)
                {
                    _timerEngine.UpdateSettings(_settings.IsContinuousMode);
                    _sessionLogger.InitializeSession(_settings.SessionName, _timerEngine.RunCount);
                    UpdateVisuals();
                    UpdateTooltip();
                    UpdateDisplay();
                    SettingsChanged?.Invoke();
                }
                OptionsClosed?.Invoke();
            }
            finally
            {
                IsDialogOpen = false;
            }
        }

        private void ExecuteSave()
        {
            IsDialogOpen = true;
            try
            {
                string filename = $"{SessionName}.txt";
                if (string.IsNullOrWhiteSpace(SessionName)) filename = "Session.txt";

                var dialogResult = RequestSaveDialog?.Invoke(filename);
                if (dialogResult?.result == true)
                {
                    if (System.IO.File.Exists(filename))
                    {
                        if (RequestConfirmation?.Invoke($"File '{filename}' already exists. Overwrite?", "Overwrite Confirmation") != true)
                        {
                            return;
                        }
                    }

                    bool includeRunsWithoutLoot = dialogResult?.includeEmpty ?? true;

                    string header = $"Session: {SessionName}{Environment.NewLine}" +
                                     $"Runs: {_timerEngine.RunCount}{Environment.NewLine}" +
                                     $"Best: {BestTimeText}{Environment.NewLine}" +
                                     $"Last: {LastRunTimeText}{Environment.NewLine}" +
                                     $"Worst: {WorstTimeText}{Environment.NewLine}" +
                                     $"Avg: {AverageTimeText}{Environment.NewLine}" +
                                     $"Total: {TotalTimeText}{Environment.NewLine}" +
                                     $"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                    _reportExporter.SaveSessionToFile(filename, header, includeRunsWithoutLoot, _sessionLogger);
                    IsDirty = false;
                    ShowMessage?.Invoke($"Stats saved to {filename}", "Success");
                }
            }
            catch (Exception ex)
            {
                ShowError?.Invoke($"Error saving stats: {ex.Message}", "Error");
            }
            finally
            {
                IsDialogOpen = false;
            }
        }

    }
}

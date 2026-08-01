using System;
using System.Linq;
using System.Windows.Input;

using RunTrackerOverlay.Models;
using RunTrackerOverlay.Services;

namespace RunTrackerOverlay.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly TimerEngine _timerEngine;
        public TimerEngine TimerEngine => _timerEngine;
        private readonly StatisticsTracker _statsTracker = new StatisticsTracker();
        private readonly AppSettings _settings;
        private readonly ISessionLogger _sessionLogger;
        private readonly IReportExporter _reportExporter;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IHotkeyCoordinator _hotkeyCoordinator;
        private readonly IDisplayService _displayService;
        private readonly ISessionFileParser _parser;
        private IDialogService _dialogService = null!;
        private string _timerText = "00:00.00";
        private string _runStatusSymbol = "■";
        private string _runLabelText = " Run #1";
        private string _bestTimeText = "00:00.00";
        private string _worstTimeText = "00:00.00";
        private string _lastRunTimeText = "00:00.00";
        private string _averageTimeText = "00:00.00";
        private string _totalTimeText = "00:00.00";
        private string _lootCountText = "0";
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
        private bool _showLoot = true;
        private bool _showSessionName = true;
        private bool _isActive;
        private bool _isDialogOpen;
        private double _windowLeft;
        private double _windowTop;
        private bool _isLootDialogOpen;

        public IDialogService DialogService { get => _dialogService; set => _dialogService = value; }

        public double WindowLeft
        {
            get => _windowLeft;
            set => SetProperty(ref _windowLeft, value);
        }

        public double WindowTop
        {
            get => _windowTop;
            set => SetProperty(ref _windowTop, value);
        }

        public Action? RequestClose { get; set; }

        public void OnClosing()
        {
            if (_timerEngine.IsRunning)
            {
                _sessionLogger.AppendRun(_timerEngine.RunCount, _timerEngine.CurrentElapsed, _timerEngine.CurrentLoot);
            }
            else
            {
                // Check if there's a PENDING run in the file that corresponds to current RunCount
                // Actually, if we just finished a run, the file is updated.
                // If we added loot but didn't start/stop, it might be PENDING.
                // The requirement says: "If there is a run that is not conluded but is recorded in the file 
                // such as when the loot is added on an unfinished run, the app exit should cause this 
                // unfinished run time to be populated with the current timer value."
                
                // If TimerEngine is NOT running, but we have PENDING loot, it's recorded in the file by SessionLogger.AppendActiveRunLoot.
                // We should probably just call AppendRun with current elapsed (which would be 0 or whatever was there).
                // But wait, if it's NOT running, CurrentElapsed is what was accumulated.
                
                var runs = _sessionLogger.GetSessionRuns().ToList();
                string pendingPrefix = $"#{_timerEngine.RunCount} PENDING";
                if (runs.Any(r => r.StartsWith(pendingPrefix)))
                {
                    _sessionLogger.AppendRun(_timerEngine.RunCount, _timerEngine.CurrentElapsed, _timerEngine.CurrentLoot);
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand OptionsCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public void ShowLootDialog()
        {
            if (_isLootDialogOpen) return;
            _isLootDialogOpen = true;

            try
            {
                _hotkeyCoordinator.IsPaused = true;
                string? loot = _dialogService.ShowLootDialog();
                if (loot != null)
                {
                    _timerEngine.AddLoot(loot);
                }
            }
            finally
            {
                _isLootDialogOpen = false;
                _hotkeyCoordinator.IsPaused = false;
            }
        }
        
        public MainViewModel(TimerEngine timerEngine, AppSettings settings, ISessionLogger sessionLogger, IReportExporter reportExporter, ISettingsProvider settingsProvider, IHotkeyCoordinator hotkeyCoordinator, IDisplayService displayService, ISessionFileParser parser)
        {
            _timerEngine = timerEngine;
            _settings = settings;
            _sessionLogger = sessionLogger;
            _reportExporter = reportExporter;
            _settingsProvider = settingsProvider;
            _hotkeyCoordinator = hotkeyCoordinator;
            _displayService = displayService;
            _parser = parser;

            WindowLeft = _settings.WindowLeft;
            WindowTop = _settings.WindowTop;

            if (_sessionLogger.HasSessionFile())
            {
                SessionName = _sessionLogger.GetSessionName() ?? "Session 1";
                var runs = _sessionLogger.GetSessionRuns().ToList();
                if (runs.Any())
                {
                    _statsTracker.LoadFromRuns(runs);
                    _timerEngine.RestoreState(_statsTracker.MaxRunNumber, _statsTracker.LastRunTime);
                }
                else
                {
                    SessionName = _sessionLogger.GetSessionName() ?? "Session 1";
                    _timerEngine.UpdateRunCount(0);
                }
            }
            else
            {
                SessionName = "Session 1";
                _sessionLogger.InitializeSession(SessionName, 0);
            }
            
            _timerEngine.StateChanged += () => 
            {
                UpdateDisplay();
            };
            _timerEngine.RunCompleted += (count, duration, loot) =>
            {
                _statsTracker.AddRun(duration);
                _sessionLogger.AppendRun(count, duration, loot);
                UpdateDisplay();
            };
            _timerEngine.LootAddedToLastRun += (loot) =>
            {
                _statsTracker.IncrementLootCount();
                _sessionLogger.UpdateLastRunLoot(loot);
                UpdateDisplay();
            };

            _timerEngine.LootAdded += (loot) =>
            {
                _statsTracker.IncrementLootCount();
                _sessionLogger.AppendActiveRunLoot(_timerEngine.RunCount, loot);
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
        public string LootCountText { get => _lootCountText; set => SetProperty(ref _lootCountText, value); }
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

        public bool ShowLoot 
        { 
            get => _showLoot; 
            set 
            { 
                if (SetProperty(ref _showLoot, value))
                    OnPropertyChanged(nameof(ShowStats));
            } 
        }

        public bool ShowStats => ShowBest || ShowLast || ShowWorst || ShowAvg || ShowTotal || ShowLoot;

        public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
        public bool IsDialogOpen { get => _isDialogOpen; set => SetProperty(ref _isDialogOpen, value); }

        // WindowWidth property removed

        private void TimerEngine_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateDisplay();
        }

        public void UpdateVisuals()
        {
            BackgroundBrush = _displayService.GetBackgroundBrush(_settings.WindowOpacity);
            TextOpacity = _settings.TextOpacity;
            ShowBest = _settings.ShowBest;
            ShowLast = _settings.ShowLast;
            ShowWorst = _settings.ShowWorst;
            ShowAvg = _settings.ShowAvg;
            ShowTotal = _settings.ShowTotal;
            ShowSessionName = _settings.ShowSessionName;
        }

        public void UpdateTooltip()
        {
            TooltipText = _displayService.GetTooltipText(_settings);
            ShowTooltip = _settings.ShowKeysTooltip;
        }

        public void UpdateDisplay()
        {
            TimeSpan elapsed = _timerEngine.CurrentElapsed;
            TimerText = _displayService.GetTimerText(elapsed, _settings.TimerFormat);

            RunStatusSymbol = _displayService.GetRunStatusSymbol(_timerEngine.IsRunning, _timerEngine.IsPaused, _timerEngine.RunCount);
            RunLabelText = _displayService.GetRunLabelText(_timerEngine.IsRunning, _timerEngine.RunCount, _settings.ShowRunCount);
            RunStatusForeground = _displayService.GetRunStatusForeground(_timerEngine.IsRunning, _timerEngine.IsPaused, _timerEngine.RunCount);
            TimerForeground = _displayService.GetTimerForeground(_timerEngine.IsRunning, _timerEngine.IsPaused, _timerEngine.RunCount);

            string statsFormat = _settings.ApplyFormatToStats ? _settings.TimerFormat : Constants.TimeFormatStandard;

            BestTimeText = _displayService.GetTimerText(_statsTracker.BestTime, statsFormat);
            WorstTimeText = _displayService.GetTimerText(_statsTracker.WorstTime, statsFormat);
            LastRunTimeText = _displayService.GetTimerText(_timerEngine.LastRunTime, statsFormat);
            
            AverageTimeText = _displayService.GetTimerText(_statsTracker.RunCount > 0 
                ? TimeSpan.FromTicks(_statsTracker.TotalTime.Ticks / _statsTracker.RunCount) 
                : TimeSpan.Zero, statsFormat);
            TotalTimeText = _displayService.GetTimerText(_statsTracker.TotalTime, statsFormat);
            LootCountText = _statsTracker.LootCount.ToString();
        }

        private void ExecuteReset()
        {
            if (_dialogService.ShowConfirmationDialog("Save before resetting?", "Save Confirmation") == true)
            {
                ExecuteSave();
            }

            _timerEngine.Reset();
            _statsTracker.Reset();
            _sessionLogger.DeleteSessionFile();
            SessionName = "Session 1";
            _sessionLogger.InitializeSession(SessionName, 0);
            UpdateDisplay();
        }

        public Action? SettingsChanged;

        private void ExecuteOptions()
        {
            _hotkeyCoordinator.IsPaused = true;
            string originalSessionName = SessionName;
            try
            {
                if (_dialogService.ShowOptionsDialog(_settings, SessionName, vm => 
                    {
                        if (vm.SessionName != SessionName)
                        {
                            SessionName = vm.SessionName;
                        }
                        vm.ApplyTo(_settings);
                        _timerEngine.UpdateSettings(_settings.Mode);
                        UpdateVisuals();
                        UpdateTooltip();
                        UpdateDisplay();
                        _hotkeyCoordinator.UpdateSettings(_settings);
                    }) == true)
                {
                    _sessionLogger.InitializeSession(SessionName, _timerEngine.RunCount);
                    _timerEngine.UpdateSettings(_settings.Mode);
                    UpdateVisuals();
                    UpdateTooltip();
                    UpdateDisplay();
                    _hotkeyCoordinator.UpdateSettings(_settings);
                    _settingsProvider.SaveSettings(_settings);
                }
                else
                {
                    SessionName = originalSessionName;
                    var loaded = _settingsProvider.LoadSettings();
                    _settings.CopyFrom(loaded);
                    _settings.WindowLeft = WindowLeft;
                    _settings.WindowTop = WindowTop;
                    _hotkeyCoordinator.UpdateSettings(_settings);
                    UpdateVisuals();
                    UpdateTooltip();
                    UpdateDisplay();
                }
            }
            finally
            {
                _hotkeyCoordinator.IsPaused = false;
            }
        }

        private void ExecuteSave()
        {
            try
            {
                string filename = $"{SessionName}.txt";
                if (string.IsNullOrWhiteSpace(SessionName)) filename = "Session.txt";

                var dialogResult = _dialogService.ShowSaveDialog(filename);
                if (dialogResult.result == true)
                {
                    if (System.IO.File.Exists(filename))
                    {
                        if (_dialogService.ShowConfirmationDialog($"File '{filename}' already exists. Overwrite?", "Overwrite Confirmation") != true)
                        {
                            return;
                        }
                    }

                    bool includeRunsWithoutLoot = dialogResult.includeEmpty ?? true;

                    string header = $"Session: {SessionName}{Environment.NewLine}" +
                                     $"Runs: {_timerEngine.RunCount}{Environment.NewLine}" +
                                     $"Best: {BestTimeText}{Environment.NewLine}" +
                                     $"Last: {LastRunTimeText}{Environment.NewLine}" +
                                     $"Worst: {WorstTimeText}{Environment.NewLine}" +
                                     $"Avg: {AverageTimeText}{Environment.NewLine}" +
                                     $"Total: {TotalTimeText}{Environment.NewLine}" +
                                     $"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                    _reportExporter.SaveSessionToFile(filename, header, includeRunsWithoutLoot, _sessionLogger);
                    _dialogService.ShowMessage($"Stats saved to {filename}", "Success");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Error saving stats: {ex.Message}", "Error");
            }
        }

    }
}

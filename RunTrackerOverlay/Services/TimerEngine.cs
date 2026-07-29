using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using RunTrackerOverlay.ViewModels;

namespace RunTrackerOverlay.Services
{
    public class TimerEngine : ViewModelBase
    {
        private readonly ISessionLogger _sessionLogger;
        private int _runCount;
        private bool _isRunning;
        private Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _accumulatedTime = TimeSpan.Zero;
        private TimeSpan _lastRunTime = TimeSpan.Zero;
        private TimeSpan _bestTime = TimeSpan.Zero;
        private TimeSpan _worstTime = TimeSpan.Zero;
        private TimeSpan _totalTime = TimeSpan.Zero;
        private bool _isContinuousMode;
        private string _currentLoot = string.Empty;

        private string _sessionName = string.Empty;

        public int RunCount { get => _runCount; private set => SetProperty(ref _runCount, value); }
        public bool IsRunning { get => _isRunning; private set => SetProperty(ref _isRunning, value); }
        public TimeSpan CurrentElapsed => _accumulatedTime + _stopwatch.Elapsed;
        public TimeSpan BestTime { get => _bestTime; private set => SetProperty(ref _bestTime, value); }
        public TimeSpan WorstTime { get => _worstTime; private set => SetProperty(ref _worstTime, value); }
        public TimeSpan TotalTime { get => _totalTime; private set => SetProperty(ref _totalTime, value); }
        public TimeSpan LastRunTime { get => _lastRunTime; private set => SetProperty(ref _lastRunTime, value); }
        public string SessionName { get => _sessionName; set => SetProperty(ref _sessionName, value); }

        public TimerEngine(ISessionLogger sessionLogger, bool isContinuousMode)
        {
            _sessionLogger = sessionLogger;
            _isContinuousMode = isContinuousMode;
            Reset(true);
        }

        public void UpdateSettings(bool isContinuousMode)
        {
            _isContinuousMode = isContinuousMode;
        }

        public void AddLoot(string loot)
        {
            if (string.IsNullOrWhiteSpace(loot)) return;

            loot = ToTitleCase(loot);

            if (IsRunning)
            {
                if (!string.IsNullOrEmpty(_currentLoot))
                {
                    _currentLoot += ", " + loot;
                }
                else
                {
                    _currentLoot = loot;
                }
                OnPropertyChanged("CurrentLoot");
            }
            else if (RunCount > 0)
            {
                UpdateLastRunLoot(loot);
            }
        }

        private string ToTitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            // TextInfo.ToTitleCase preserves all-caps words as acronyms, which we don't necessarily want here
            // but the requirement "item one, item two" -> "Item One, Item Two" suggests basic TitleCase.
            // If the user inputs "ITEM ONE", it will stay "ITEM ONE" with ToTitleCase.
            // To truly enforce word by word capitalization as described, we might want to Lower it first.
            return textInfo.ToTitleCase(input.ToLower());
        }

        public void InitializeSessionFile()
        {
            _sessionLogger.InitializeSession(SessionName, RunCount);
        }

        private void UpdateLastRunLoot(string loot)
        {
            _sessionLogger.UpdateLastRunLoot(loot);
            OnPropertyChanged("LastRunLoot");
        }

        public void StartRun()
        {
            RunCount++;
            IsRunning = true;
            _accumulatedTime = TimeSpan.Zero;
            _stopwatch.Restart();
            _currentLoot = string.Empty;
        }

        public void StopRun()
        {
            if (!IsRunning) return;

            _stopwatch.Stop();
            IsRunning = false;
            _accumulatedTime = _stopwatch.Elapsed;
            _stopwatch.Reset();
            LastRunTime = _accumulatedTime;
            
            TotalTime += _accumulatedTime;
            if (RunCount == 1 || _accumulatedTime < BestTime)
            {
                BestTime = _accumulatedTime;
            }

            if (RunCount == 1 || _accumulatedTime > WorstTime)
            {
                WorstTime = _accumulatedTime;
            }

            SaveRunToSession();
        }

        public void HandlePause()
        {
            if (!IsRunning)
            {
                StartRun();
            }
            else
            {
                StopRun();
                if (_isContinuousMode)
                {
                    StartRun();
                }
            }
        }

        public void HandleContinuousStop()
        {
            if (_isContinuousMode && IsRunning)
            {
                StopRun();
            }
        }

        public void Reset(bool deleteLog = true)
        {
            _stopwatch.Reset();
            IsRunning = false;
            RunCount = 0;
            _accumulatedTime = TimeSpan.Zero;
            LastRunTime = TimeSpan.Zero;
            BestTime = TimeSpan.Zero;
            WorstTime = TimeSpan.Zero;
            TotalTime = TimeSpan.Zero;

            if (deleteLog)
            {
                InitializeSessionFile();
            }
        }

        private void SaveRunToSession()
        {
            _sessionLogger.AppendRun(RunCount, LastRunTime, _currentLoot);
        }
    }
}

using System;
using System.Diagnostics;

using System.IO;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Services
{
    public class TimerEngine
    {
        private TrackerMode _mode;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _accumulatedTime = TimeSpan.Zero;
        private string _currentLoot = string.Empty;
        private FileSystemWatcher? _fileWatcher;
        private DateTime _lastFileChangedTime = DateTime.MinValue;
        private readonly string _d2rAchievementPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"Saved Games\Diablo II Resurrected\AchievementTracker.json");

        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public int RunCount { get; private set; }
        public TimeSpan CurrentElapsed
        {
            get
            {
                if (IsRunning && !IsPaused)
                {
                    return _accumulatedTime + _stopwatch.Elapsed;
                }
                return _accumulatedTime;
            }
        }
        public TimeSpan LastRunTime { get; private set; } = TimeSpan.Zero;
        public string CurrentLoot => _currentLoot;

        public event Action<int, TimeSpan, string>? RunCompleted;
        public event Action<string>? LootAdded;
        public event Action<string>? LootAddedToLastRun;
        public event Action? StateChanged;

        public TimerEngine(TrackerMode mode)
        {
            _mode = mode;
        }

        public void UpdateSettings(TrackerMode mode)
        {
            var oldMode = _mode;
            _mode = mode;
            if (oldMode == TrackerMode.D2RAuto && _mode != TrackerMode.D2RAuto)
            {
                StopFileWatcher();
            }
        }

        public void RestoreState(int runCount, TimeSpan lastRunTime)
        {
            RunCount = runCount;
            LastRunTime = lastRunTime;
            _accumulatedTime = lastRunTime;
            StateChanged?.Invoke();
        }

        public void UpdateRunCount(int runCount)
        {
            RunCount = runCount;
            StateChanged?.Invoke();
        }

        public void AddLoot(string loot)
        {
            if (string.IsNullOrWhiteSpace(loot)) return;

            loot = StringUtils.ToTitleCase(loot);

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
                LootAdded?.Invoke(loot);
                StateChanged?.Invoke();
            }
            else if (RunCount > 0)
            {
                LootAddedToLastRun?.Invoke(loot);
            }
        }

        public void StartRun()
        {
            if (IsRunning) return;
            RunCount++;
            IsRunning = true;
            IsPaused = false;
            _accumulatedTime = TimeSpan.Zero;
            _stopwatch.Restart();
            _currentLoot = string.Empty;
            StateChanged?.Invoke();
        }

        public void StopRun()
        {
            if (!IsRunning) return;

            if (!IsPaused)
            {
                _stopwatch.Stop();
                _accumulatedTime += _stopwatch.Elapsed;
            }
            IsRunning = false;
            IsPaused = false;
            _stopwatch.Reset();
            LastRunTime = _accumulatedTime;

            var loot = _currentLoot;
            RunCompleted?.Invoke(RunCount, LastRunTime, loot);
            StateChanged?.Invoke();
        }

        public void PauseResume()
        {
            if (!IsRunning) return;

            if (IsPaused)
            {
                // Resume
                _stopwatch.Restart();
                IsPaused = false;
            }
            else
            {
                // Pause
                _stopwatch.Stop();
                _accumulatedTime += _stopwatch.Elapsed;
                _stopwatch.Reset();
                IsPaused = true;
            }
            StateChanged?.Invoke();
        }

        public void HandlePause()
        {
            if (_mode == TrackerMode.D2RAuto)
            {
                if (!IsRunning)
                {
                    StartRun();
                    StartFileWatcher();
                }
                else
                {
                    StopRun();
                    StopFileWatcher();
                }
                return;
            }

            if (!IsRunning)
            {
                StartRun();
            }
            else
            {
                StopRun();
                if (_mode == TrackerMode.Continuous)
                {
                    StartRun();
                }
            }
        }

        private void StartFileWatcher()
        {
            StopFileWatcher();

            try
            {
                var directory = Path.GetDirectoryName(_d2rAchievementPath);
                var fileName = Path.GetFileName(_d2rAchievementPath);

                if (directory != null && Directory.Exists(directory))
                {
                    _fileWatcher = new FileSystemWatcher(directory, fileName);
                    _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                    _fileWatcher.Changed += OnFileChanged;
                    _fileWatcher.EnableRaisingEvents = true;
                }
            }
            catch
            {
                // Silently fail if path is inaccessible
            }
        }

        private void StopFileWatcher()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Changed -= OnFileChanged;
                _fileWatcher.Dispose();
                _fileWatcher = null;
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (_mode == TrackerMode.D2RAuto && IsRunning)
            {
                var now = DateTime.Now;
                if ((now - _lastFileChangedTime).TotalMilliseconds < 300)
                {
                    return;
                }
                _lastFileChangedTime = now;

                StopRun();
                StartRun();
            }
        }

        public void HandleContinuousStop()
        {
            if (_mode == TrackerMode.Continuous && IsRunning)
            {
                StopRun();
            }
        }

        public void Reset()
        {
            _stopwatch.Reset();
            IsRunning = false;
            IsPaused = false;
            RunCount = 0;
            _accumulatedTime = TimeSpan.Zero;
            LastRunTime = TimeSpan.Zero;
            _currentLoot = string.Empty;
            StateChanged?.Invoke();
        }
    }
}

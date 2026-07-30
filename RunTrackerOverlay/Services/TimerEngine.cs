using System;
using System.Diagnostics;

namespace RunTrackerOverlay.Services
{
    public class TimerEngine
    {
        private bool _isContinuousMode;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _accumulatedTime = TimeSpan.Zero;
        private string _currentLoot = string.Empty;

        public bool IsRunning { get; private set; }
        public int RunCount { get; private set; }
        public TimeSpan CurrentElapsed => _accumulatedTime + _stopwatch.Elapsed;
        public TimeSpan LastRunTime { get; private set; } = TimeSpan.Zero;
        public string CurrentLoot => _currentLoot;

        public event Action<int, TimeSpan, string>? RunCompleted;
        public event Action<string>? LootAddedToLastRun;
        public event Action? StateChanged;

        public TimerEngine(bool isContinuousMode)
        {
            _isContinuousMode = isContinuousMode;
        }

        public void UpdateSettings(bool isContinuousMode)
        {
            _isContinuousMode = isContinuousMode;
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
            _accumulatedTime = TimeSpan.Zero;
            _stopwatch.Restart();
            _currentLoot = string.Empty;
            StateChanged?.Invoke();
        }

        public void StopRun()
        {
            if (!IsRunning) return;

            _stopwatch.Stop();
            IsRunning = false;
            _accumulatedTime = _stopwatch.Elapsed;
            _stopwatch.Reset();
            LastRunTime = _accumulatedTime;

            var loot = _currentLoot;
            RunCompleted?.Invoke(RunCount, LastRunTime, loot);
            StateChanged?.Invoke();
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

        public void Reset()
        {
            _stopwatch.Reset();
            IsRunning = false;
            RunCount = 0;
            _accumulatedTime = TimeSpan.Zero;
            LastRunTime = TimeSpan.Zero;
            _currentLoot = string.Empty;
            StateChanged?.Invoke();
        }
    }
}

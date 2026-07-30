using System;

namespace RunTrackerOverlay.Services
{
    public class StatisticsTracker
    {
        public TimeSpan BestTime { get; private set; } = TimeSpan.Zero;
        public TimeSpan WorstTime { get; private set; } = TimeSpan.Zero;
        public TimeSpan TotalTime { get; private set; } = TimeSpan.Zero;
        public int RunCount { get; private set; }

        public void AddRun(TimeSpan duration)
        {
            RunCount++;
            TotalTime += duration;

            if (RunCount == 1 || duration < BestTime)
            {
                BestTime = duration;
            }

            if (duration > WorstTime)
            {
                WorstTime = duration;
            }
        }

        public void Reset()
        {
            BestTime = TimeSpan.Zero;
            WorstTime = TimeSpan.Zero;
            TotalTime = TimeSpan.Zero;
            RunCount = 0;
        }
    }
}

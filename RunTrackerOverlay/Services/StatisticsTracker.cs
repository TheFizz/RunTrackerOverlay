using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Services
{
    public class StatisticsTracker
    {
        public TimeSpan BestTime { get; private set; } = TimeSpan.Zero;
        public TimeSpan WorstTime { get; private set; } = TimeSpan.Zero;
        public TimeSpan TotalTime { get; private set; } = TimeSpan.Zero;
        public TimeSpan LastRunTime { get; private set; } = TimeSpan.Zero;
        public int RunCount { get; private set; }
        public int MaxRunNumber { get; private set; }
        public int LootCount { get; private set; }

        public void AddRun(TimeSpan duration)
        {
            RunCount++;
            TotalTime += duration;
            LastRunTime = duration;

            if (RunCount == 1 || duration < BestTime)
            {
                BestTime = duration;
            }

            if (duration > WorstTime)
            {
                WorstTime = duration;
            }
        }

        public void AddRun(int runNumber, TimeSpan duration)
        {
            if (runNumber > MaxRunNumber) MaxRunNumber = runNumber;
            AddRun(duration);
        }

        public void IncrementLootCount()
        {
            LootCount++;
        }

        public void Reset()
        {
            BestTime = TimeSpan.Zero;
            WorstTime = TimeSpan.Zero;
            TotalTime = TimeSpan.Zero;
            LastRunTime = TimeSpan.Zero;
            RunCount = 0;
            MaxRunNumber = 0;
            LootCount = 0;
        }

        public void LoadFromSessionData(SessionData data)
        {
            Reset();
            foreach (var run in data.Runs)
            {
                if (run.RunNumber > MaxRunNumber) MaxRunNumber = run.RunNumber;

                if (run.IsPending)
                {
                    LootCount += run.Loot.Count;
                }
                else if (run.Duration.HasValue)
                {
                    AddRun(run.RunNumber, run.Duration.Value);
                    LootCount += run.Loot.Count;
                }
            }
        }

        public void LoadFromRuns(IEnumerable<string> runs)
        {
            // Keeping this for potential compatibility, but using SessionFileParser is preferred
            var parser = new SessionFileParser();
            var data = parser.Parse(new[] { "" }.Concat(runs));
            LoadFromSessionData(data);
        }
    }
}

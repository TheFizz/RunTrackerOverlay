using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Services
{
    public interface ISessionFileParser
    {
        SessionData Parse(IEnumerable<string> lines);
        string FormatRun(SessionRun run);
    }

    public class SessionFileParser : ISessionFileParser
    {
        public SessionData Parse(IEnumerable<string> lines)
        {
            var data = new SessionData();
            var lineList = lines.ToList();

            if (lineList.Count > 0)
            {
                data.SessionName = lineList[0];
                foreach (var line in lineList.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var run = ParseLine(line);
                    if (run != null)
                    {
                        data.Runs.Add(run);
                    }
                }
            }

            return data;
        }

        public string FormatRun(SessionRun run)
        {
            string runPrefix = $"#{run.RunNumber}";
            if (run.IsPending)
            {
                string lootPart = run.Loot.Count > 0 ? $" {string.Join(", ", run.Loot)}" : "";
                return $"{runPrefix} PENDING{lootPart}";
            }
            else
            {
                TimeSpan duration = run.Duration ?? TimeSpan.Zero;
                string timeStr = duration.TotalDays >= 1 ? "99:59.99" : duration.ToString(@"mm\:ss\.ff");
                string lootPart = run.Loot.Count > 0 ? $" {string.Join(", ", run.Loot)}" : "";
                return $"{runPrefix} {timeStr}{lootPart}";
            }
        }

        private SessionRun? ParseLine(string line)
        {
            // Format: #1 01:23.45 [Loot1, Loot2] or #1 PENDING [Loot1, Loot2]
            var parts = line.Split(' ', 3);
            if (parts.Length < 2) return null;

            if (!parts[0].StartsWith("#") || !int.TryParse(parts[0].Substring(1), out int runNum))
            {
                return null;
            }

            var run = new SessionRun { RunNumber = runNum };

            if (parts[1] == "PENDING")
            {
                run.IsPending = true;
                if (parts.Length == 3)
                {
                    run.Loot.AddRange(parts[2].Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)));
                }
            }
            else
            {
                run.IsPending = false;
                if (parts[1] == "99:59.99")
                {
                    run.Duration = TimeSpan.FromHours(99) + TimeSpan.FromMinutes(59) + TimeSpan.FromSeconds(59) + TimeSpan.FromMilliseconds(990);
                }
                else if (TimeSpan.TryParseExact(parts[1], @"mm\:ss\.ff", CultureInfo.InvariantCulture, out var duration))
                {
                    run.Duration = duration;
                }

                if (parts.Length == 3)
                {
                    run.Loot.AddRange(parts[2].Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)));
                }
            }

            return run;
        }
    }
}

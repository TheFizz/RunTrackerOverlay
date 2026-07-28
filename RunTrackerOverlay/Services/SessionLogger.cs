using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RunTrackerOverlay.Services
{
    public interface ISessionLogger
    {
        void InitializeSession(string sessionName, int runCount);
        void AppendRun(int runCount, TimeSpan duration, string loot);
        void UpdateLastRunLoot(string loot);
        void DeleteSessionFile();
        IEnumerable<string> GetSessionRuns();
    }

    public class SessionLogger : ISessionLogger
    {
        private const string SessionFileName = "currentSession.txt";

        public void InitializeSession(string sessionName, int runCount)
        {
            try
            {
                if (runCount > 0 && File.Exists(SessionFileName))
                {
                    var existingLines = File.ReadAllLines(SessionFileName);
                    if (existingLines.Length > 1)
                    {
                        var newLines = new List<string>();
                        newLines.Add(sessionName ?? string.Empty);
                        for (int i = 1; i < existingLines.Length; i++)
                        {
                            newLines.Add(existingLines[i]);
                        }
                        File.WriteAllLines(SessionFileName, newLines);
                        return;
                    }
                }

                File.WriteAllLines(SessionFileName, new[] { sessionName ?? string.Empty });
            }
            catch { }
        }

        public void AppendRun(int runCount, TimeSpan duration, string loot)
        {
            try
            {
                string timeStr = duration.TotalDays >= 1 ? "99:59.99" : duration.ToString(@"mm\:ss\.ff");
                string lootStr = !string.IsNullOrEmpty(loot) ? $" {loot}" : "";
                File.AppendAllText(SessionFileName, $"#{runCount} {timeStr}{lootStr}{Environment.NewLine}");
            }
            catch { }
        }

        public void UpdateLastRunLoot(string loot)
        {
            try
            {
                if (!File.Exists(SessionFileName)) return;

                var lines = File.ReadAllLines(SessionFileName).ToList();
                if (lines.Count <= 1) return;

                string lastLine = lines[lines.Count - 1];
                string[] parts = lastLine.Split(' ', 3);
                
                if (parts.Length == 3)
                {
                    lastLine = $"{parts[0]} {parts[1]} {parts[2]}, {loot}";
                }
                else if (parts.Length == 2)
                {
                    lastLine = $"{parts[0]} {parts[1]} {loot}";
                }

                lines[lines.Count - 1] = lastLine;
                File.WriteAllLines(SessionFileName, lines);
            }
            catch { }
        }

        public void DeleteSessionFile()
        {
            try
            {
                if (File.Exists(SessionFileName))
                {
                    File.Delete(SessionFileName);
                }
            }
            catch { }
        }

        public IEnumerable<string> GetSessionRuns()
        {
            try
            {
                if (File.Exists(SessionFileName))
                {
                    return File.ReadAllLines(SessionFileName).Skip(1);
                }
            }
            catch { }
            return Enumerable.Empty<string>();
        }
    }
}

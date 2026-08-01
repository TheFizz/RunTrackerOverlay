using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Services
{
    public interface ISessionLogger
    {
        void InitializeSession(string sessionName, int runCount);
        void AppendRun(int runCount, TimeSpan duration, string loot);
        void AppendActiveRunLoot(int runCount, string loot);
        void UpdateLastRunLoot(string loot);
        void DeleteSessionFile();
        IEnumerable<string> GetSessionRuns();
        string? GetSessionName();
        bool HasSessionFile();
    }

    public class SessionLogger : ISessionLogger
    {
        private const string SessionFileName = "currentSession.txt";
        private readonly ISessionFileParser _parser;

        public SessionLogger(ISessionFileParser parser)
        {
            _parser = parser;
        }

        public void InitializeSession(string sessionName, int runCount)
        {
            try
            {
                if (File.Exists(SessionFileName))
                {
                    var lines = File.ReadAllLines(SessionFileName);
                    var data = _parser.Parse(lines);
                    data.SessionName = sessionName ?? string.Empty;

                    var newLines = new List<string> { data.SessionName };
                    newLines.AddRange(lines.Skip(1));
                    File.WriteAllLines(SessionFileName, newLines);
                    return;
                }

                File.WriteAllLines(SessionFileName, new[] { sessionName ?? string.Empty });
            }
            catch { }
        }

        public void AppendRun(int runCount, TimeSpan duration, string loot)
        {
            try
            {
                if (File.Exists(SessionFileName))
                {
                    var lines = File.ReadAllLines(SessionFileName);
                    var data = _parser.Parse(lines);
                    data.Runs.RemoveAll(r => r.RunNumber == runCount && r.IsPending);

                    var newLines = new List<string> { data.SessionName ?? string.Empty };
                    foreach (var r in data.Runs)
                    {
                        newLines.Add(_parser.FormatRun(r));
                    }
                    File.WriteAllLines(SessionFileName, newLines);
                }

                var newRun = new SessionRun
                {
                    RunNumber = runCount,
                    Duration = duration,
                    IsPending = false
                };
                if (!string.IsNullOrEmpty(loot))
                {
                    newRun.Loot.AddRange(loot.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)));
                }

                File.AppendAllText(SessionFileName, _parser.FormatRun(newRun) + Environment.NewLine);
            }
            catch { }
        }

        public void AppendActiveRunLoot(int runCount, string loot)
        {
            try
            {
                if (!File.Exists(SessionFileName)) return;

                var lines = File.ReadAllLines(SessionFileName);
                var data = _parser.Parse(lines);
                var run = data.Runs.LastOrDefault(r => r.RunNumber == runCount && r.IsPending);

                if (run != null)
                {
                    run.Loot.AddRange(loot.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)));
                }
                else
                {
                    run = new SessionRun
                    {
                        RunNumber = runCount,
                        IsPending = true
                    };
                    run.Loot.AddRange(loot.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)));
                    data.Runs.Add(run);
                }

                var newLines = new List<string> { data.SessionName ?? string.Empty };
                foreach (var r in data.Runs)
                {
                    newLines.Add(_parser.FormatRun(r));
                }
                File.WriteAllLines(SessionFileName, newLines);
            }
            catch { }
        }

        public void UpdateLastRunLoot(string loot)
        {
            try
            {
                if (!File.Exists(SessionFileName)) return;

                var lines = File.ReadAllLines(SessionFileName);
                var data = _parser.Parse(lines);
                if (data.Runs.Count == 0) return;

                var lastRun = data.Runs.Last();
                lastRun.Loot.AddRange(loot.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)));

                var newLines = new List<string> { data.SessionName ?? string.Empty };
                foreach (var r in data.Runs)
                {
                    newLines.Add(_parser.FormatRun(r));
                }
                File.WriteAllLines(SessionFileName, newLines);
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

        public string? GetSessionName()
        {
            try
            {
                if (File.Exists(SessionFileName))
                {
                    var lines = File.ReadAllLines(SessionFileName);
                    if (lines.Length > 0)
                    {
                        return lines[0];
                    }
                }
            }
            catch { }
            return null;
        }

        public bool HasSessionFile()
        {
            return File.Exists(SessionFileName);
        }
    }
}

using System;
using System.IO;

namespace RunTrackerOverlay.Services
{
    public interface IReportExporter
    {
        void SaveSessionToFile(string filename, string header, bool includeRunsWithoutLoot, ISessionLogger sessionLogger);
    }

    public class FileReportExporter : IReportExporter
    {
        public void SaveSessionToFile(string filename, string header, bool includeRunsWithoutLoot, ISessionLogger sessionLogger)
        {
            string content = header + Environment.NewLine + Environment.NewLine;
            
            var runs = sessionLogger.GetSessionRuns();
            foreach (var line in runs)
            {
                if (!includeRunsWithoutLoot)
                {
                    string[] parts = line.Split(' ', 3);
                    if (parts.Length < 3 || string.IsNullOrWhiteSpace(parts[2]))
                    {
                        continue;
                    }
                }
                content += line + Environment.NewLine;
            }

            File.WriteAllText(filename, content);
        }
    }
}

using System;
using System.IO;
using System.Text;

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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header);
            sb.AppendLine();
            
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
                sb.AppendLine(line);
            }

            File.WriteAllText(filename, sb.ToString());
        }
    }
}

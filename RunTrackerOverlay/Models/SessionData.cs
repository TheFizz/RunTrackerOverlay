using System;
using System.Collections.Generic;

namespace RunTrackerOverlay.Models
{
    public class SessionRun
    {
        public int RunNumber { get; set; }
        public TimeSpan? Duration { get; set; }
        public bool IsPending { get; set; }
        public List<string> Loot { get; set; } = new List<string>();

        public string? LootString => Loot.Count > 0 ? string.Join(", ", Loot) : null;
    }

    public class SessionData
    {
        public string? SessionName { get; set; }
        public List<SessionRun> Runs { get; set; } = new List<SessionRun>();
    }
}

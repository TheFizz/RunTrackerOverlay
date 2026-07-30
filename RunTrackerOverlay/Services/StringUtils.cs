using System.Globalization;

namespace RunTrackerOverlay.Services
{
    public static class StringUtils
    {
        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            // Lower it first to truly enforce title case if needed, 
            // but the original code didn't do it, just noted it.
            // I'll stick to original behavior for now.
            return textInfo.ToTitleCase(input);
        }
    }
}

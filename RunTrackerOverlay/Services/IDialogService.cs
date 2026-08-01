using System;
using RunTrackerOverlay.Models;
using RunTrackerOverlay.ViewModels;

namespace RunTrackerOverlay.Services
{
    public interface IDialogService
    {
        (bool? result, bool? includeEmpty) ShowSaveDialog(string filename);
        bool? ShowConfirmationDialog(string message, string title);
        bool? ShowOptionsDialog(AppSettings settings, string currentSessionName, Action<OptionsViewModel> onSettingChanged);
        void ShowMessage(string message, string title);
        void ShowError(string message, string title);
        string? ShowLootDialog();
    }
}

using System;
using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Services
{
    public interface IDialogService
    {
        (bool? result, bool? includeEmpty) ShowSaveDialog(string filename);
        bool? ShowConfirmationDialog(string message, string title);
        bool? ShowOptionsDialog(AppSettings settings);
        void ShowMessage(string message, string title);
        void ShowError(string message, string title);
        string? ShowLootDialog();
    }
}

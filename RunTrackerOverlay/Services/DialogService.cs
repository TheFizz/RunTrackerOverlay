using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using RunTrackerOverlay.Models;
using RunTrackerOverlay.ViewModels;
using RunTrackerOverlay.Views;
using Point = System.Windows.Point;

namespace RunTrackerOverlay.Services
{
    public class DialogService : IDialogService
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IWindowController _windowController;

        public DialogService(MainViewModel mainViewModel, IWindowController windowController)
        {
            _mainViewModel = mainViewModel;
            _windowController = windowController;
        }

        private Window? GetActiveWindow()
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is MainWindow)
                    return window;
            }
            return System.Windows.Application.Current.MainWindow;
        }

        private T ShowDialogWithState<T>(Func<T> dialogAction)
        {
            _mainViewModel.IsDialogOpen = true;
            try
            {
                return dialogAction();
            }
            finally
            {
                _mainViewModel.IsDialogOpen = false;
            }
        }

        public (bool? result, bool? includeEmpty) ShowSaveDialog(string filename)
        {
            return ShowDialogWithState(() =>
            {
                SaveDialog saveDialog = new SaveDialog(filename) { Owner = GetActiveWindow() };
                bool? result = saveDialog.ShowDialog();
                return (result, saveDialog.IncludeEmpty);
            });
        }

        public bool? ShowConfirmationDialog(string message, string title)
        {
            return ShowDialogWithState(() =>
            {
                ConfirmationDialog confirmDialog = new ConfirmationDialog(message, title) { Owner = GetActiveWindow() };
                return confirmDialog.ShowDialog();
            });
        }

        public bool? ShowOptionsDialog(AppSettings settings, Action<OptionsViewModel> onSettingChanged)
        {
            return ShowDialogWithState(() =>
            {
                OptionsWindow optionsWin = new OptionsWindow(settings) { Owner = GetActiveWindow() };
                
                EventHandler<string> handler = (s, e) => onSettingChanged(optionsWin.ViewModel);
                optionsWin.ViewModel.SettingChanged += handler;
                
                try
                {
                    return optionsWin.ShowDialog();
                }
                finally
                {
                    optionsWin.ViewModel.SettingChanged -= handler;
                }
            });
        }

        public void ShowMessage(string message, string title)
        {
            ShowDialogWithState(() =>
            {
                MessageDialog msgDialog = new MessageDialog(message, title) { Owner = GetActiveWindow() };
                msgDialog.ShowDialog();
                return true;
            });
        }

        public void ShowError(string message, string title)
        {
            ShowDialogWithState(() =>
            {
                MessageDialog msgDialog = new MessageDialog(message, title, isError: true) { Owner = GetActiveWindow() };
                msgDialog.ShowDialog();
                return true;
            });
        }

        public string? ShowLootDialog()
        {
            return ShowDialogWithState(() =>
            {
                LootDialog lootDialog = new LootDialog();
                
                Point mousePoint = _windowController.GetCursorPosition();
                var screen = Screen.FromPoint(new System.Drawing.Point((int)mousePoint.X, (int)mousePoint.Y));
                var workArea = screen.WorkingArea;
                
                lootDialog.Left = workArea.Left + (workArea.Width - 300) / 2;
                lootDialog.Top = workArea.Top + (workArea.Height - 150) / 2;
                
                lootDialog.WindowStartupLocation = WindowStartupLocation.Manual;
                lootDialog.Owner = GetActiveWindow();

                if (lootDialog.ShowDialog() == true)
                {
                    return lootDialog.LootResult;
                }
                return null;
            });
        }
    }
}

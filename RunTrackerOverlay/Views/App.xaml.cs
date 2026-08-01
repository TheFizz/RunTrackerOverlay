using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using RunTrackerOverlay.Models;
using RunTrackerOverlay.Services;
using RunTrackerOverlay.ViewModels;

namespace RunTrackerOverlay.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ISessionFileParser sessionFileParser = new SessionFileParser();
            ISessionLogger sessionLogger = new SessionLogger(sessionFileParser);
            ISettingsProvider settingsProvider = new FileSettingsProvider();
            IReportExporter reportExporter = new FileReportExporter();
            IWindowController windowController = new WindowController();
            IHotkeyCoordinator hotkeyCoordinator = new HotkeyCoordinator();
            IDisplayService displayService = new DisplayService();

            AppSettings settings = settingsProvider.LoadSettings();
            TimerEngine timerEngine = new TimerEngine(settings.Mode);

            RunTrackerOverlay.ViewModels.MainViewModel viewModel = new RunTrackerOverlay.ViewModels.MainViewModel(
                timerEngine, 
                settings, 
                sessionLogger, 
                reportExporter, 
                settingsProvider, 
                hotkeyCoordinator,
                displayService,
                sessionFileParser);
            IDialogService dialogService = new DialogService(viewModel, windowController);
            viewModel.DialogService = dialogService;

            MainWindow mainWindow = new MainWindow(
                viewModel, 
                settings, 
                sessionLogger,
                settingsProvider, 
                windowController, 
                hotkeyCoordinator);

            mainWindow.Show();
        }
    }
}
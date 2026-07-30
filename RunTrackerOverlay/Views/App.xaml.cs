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

            ISessionLogger sessionLogger = new SessionLogger();
            ISettingsProvider settingsProvider = new FileSettingsProvider();
            IReportExporter reportExporter = new FileReportExporter();
            IWindowController windowController = new WindowController();
            IHotkeyCoordinator hotkeyCoordinator = new HotkeyCoordinator();

            AppSettings settings = settingsProvider.LoadSettings();
            TimerEngine timerEngine = new TimerEngine(settings.IsContinuousMode);
            sessionLogger.InitializeSession(settings.SessionName, 0);

            RunTrackerOverlay.ViewModels.MainViewModel viewModel = new RunTrackerOverlay.ViewModels.MainViewModel(timerEngine, settings, sessionLogger, reportExporter);

            MainWindow mainWindow = new MainWindow(
                viewModel, 
                settings, 
                timerEngine, 
                sessionLogger, 
                settingsProvider, 
                reportExporter, 
                windowController, 
                hotkeyCoordinator);

            mainWindow.Show();
        }
    }
}
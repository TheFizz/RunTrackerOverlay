using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;

using RunTrackerOverlay.Models;
using RunTrackerOverlay.Services;
using RunTrackerOverlay.ViewModels;

namespace RunTrackerOverlay.Views
{
    public partial class MainWindow : Window
    {
        private readonly AppSettings _settings;
        private readonly TimerEngine _timerEngine;
        private readonly MainViewModel _viewModel;
        private readonly ISessionLogger _sessionLogger;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IReportExporter _reportExporter;
        private readonly IWindowController _windowController;
        private readonly IHotkeyCoordinator _hotkeyCoordinator;

        private DispatcherTimer? _displayTimer;

        private double _virtualLeft;
        private double _virtualTop;
        private int _mouseOffsetX;
        private int _mouseOffsetY;
        private bool _isDragging;
        private bool _isLootDialogOpen;
        private bool _isUpdatingPosition;

        public MainWindow(
            MainViewModel viewModel,
            AppSettings settings,
            TimerEngine timerEngine,
            ISessionLogger sessionLogger,
            ISettingsProvider settingsProvider,
            IReportExporter reportExporter,
            IWindowController windowController,
            IHotkeyCoordinator hotkeyCoordinator)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _settings = settings;
            _timerEngine = timerEngine;
            _sessionLogger = sessionLogger;
            _settingsProvider = settingsProvider;
            _reportExporter = reportExporter;
            _windowController = windowController;
            _hotkeyCoordinator = hotkeyCoordinator;

            DataContext = _viewModel;

            _viewModel.RequestSaveDialog = (filename) => ShowDialogWithState(() =>
            {
                SaveDialog saveDialog = new SaveDialog(filename) { Owner = this };
                bool? result = saveDialog.ShowDialog();
                return (result, saveDialog.IncludeEmpty);
            });

            _viewModel.RequestConfirmation = (message, title) => ShowDialogWithState(() =>
            {
                ConfirmationDialog confirmDialog = new ConfirmationDialog(message, title) { Owner = this };
                return confirmDialog.ShowDialog();
            });

            _viewModel.RequestOptionsDialog = () => ShowDialogWithState(() =>
            {
                OptionsWindow optionsWin = new OptionsWindow((RunTrackerOverlay.Models.AppSettings)_settings) { Owner = this };

                optionsWin.ViewModel.SettingChanged += (s, propName) =>
                {
                    optionsWin.ViewModel.ApplyTo(_settings);
                    if (propName == nameof(OptionsViewModel.IsContinuousMode))
                    {
                        _timerEngine.UpdateSettings(_settings.IsContinuousMode);
                    }
                    if (propName == nameof(OptionsViewModel.ActivationKey) ||
                        propName == nameof(OptionsViewModel.FocusKey) ||
                        propName == nameof(OptionsViewModel.PauseKey) ||
                        propName == nameof(OptionsViewModel.LootKey))
                    {
                        _hotkeyCoordinator.UpdateSettings(_settings);
                    }
                    ApplySettings(updatePosition: false);
                };

                bool? result = optionsWin.ShowDialog();

                if (result == true)
                {
                    SyncSettings();
                    optionsWin.ViewModel.ApplyTo(_settings);
                    _timerEngine.UpdateSettings(_settings.IsContinuousMode);
                    _sessionLogger.InitializeSession(_settings.SessionName, _timerEngine.RunCount);
                    _hotkeyCoordinator.UpdateSettings(_settings);
                    ApplySettings(updatePosition: false);
                    _settingsProvider.SaveSettings(_settings);
                }
                else
                {
                    var oldLeft = this.Left;
                    var oldTop = this.Top;
                    var loaded = _settingsProvider.LoadSettings(); // Reload original from disk
                    _settings.CopyFrom(loaded);      // Update the existing reference
                    _settings.WindowLeft = oldLeft;
                    _settings.WindowTop = oldTop;
                    _hotkeyCoordinator.UpdateSettings(_settings);
                    ApplySettings();
                }

                return result;
            });

            _viewModel.OptionsRequested = () => _hotkeyCoordinator.IsPaused = true;
            _viewModel.OptionsClosed = () => _hotkeyCoordinator.IsPaused = false;
            _viewModel.SettingsChanged = () => ApplySettings(updatePosition: false);
            _viewModel.RequestClose = () => Close();

            _viewModel.ShowMessage = (msg, title) => ShowDialogWithState(() =>
            {
                MessageDialog msgDialog = new MessageDialog(msg, title) { Owner = this };
                return msgDialog.ShowDialog();
            });

            _viewModel.ShowError = (msg, title) => ShowDialogWithState(() =>
            {
                MessageDialog msgDialog = new MessageDialog(msg, title, isError: true) { Owner = this };
                return msgDialog.ShowDialog();
            });

            _hotkeyCoordinator.Initialize(
                _settings, 
                _timerEngine, 
                () => Dispatcher.BeginInvoke(new Action(ShowLootDialog)), 
                () => Dispatcher.BeginInvoke(new Action(() => _windowController.ActivateWindow(this))));

            Activated += (s, e) => 
            { 
                if (!_viewModel.IsDialogOpen)
                {
                    _viewModel.IsActive = true; 
                    ApplySettings(false); 
                }
            };
            Deactivated += (s, e) => 
            { 
                if (!_viewModel.IsDialogOpen)
                {
                    _viewModel.IsActive = false; 
                    ApplySettings(false); 
                }
            };

            SizeChanged += MainWindow_SizeChanged;

            _displayTimer = new DispatcherTimer(DispatcherPriority.Render);
            _displayTimer.Interval = TimeSpan.FromMilliseconds(10);
            _displayTimer.Tick += (s, e) => _viewModel.UpdateDisplay();
            _displayTimer.Start();

            ApplySettings();

            Application.Current.Exit += (s, e) => CleanUp();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => CleanUp();
        }

        private bool _isCleanedUp;
        private readonly object _cleanupLock = new object();

        private void CleanUp()
        {
            lock (_cleanupLock)
            {
                if (_isCleanedUp) return;
                _isCleanedUp = true;
            }

            _hotkeyCoordinator.Dispose();

            if (_displayTimer != null)
            {
                _displayTimer.Stop();
                _displayTimer = null;
            }

            _sessionLogger.DeleteSessionFile();
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == OptionsLink || e.OriginalSource == ResetLink || e.OriginalSource == SaveLink || e.OriginalSource == CloseLink) return;
            DragMove();
        }


        private void ApplySettings(bool updatePosition = true)
        {
            if (updatePosition)
            {
                this.Left = _settings.WindowLeft;
                this.Top = _settings.WindowTop;
            }
            _viewModel.UpdateVisuals();
            _viewModel.UpdateTooltip();
            _viewModel.UpdateDisplay();
            _hotkeyCoordinator.UpdateSettings(_settings);
            _windowController.SetClickThrough(this, !_viewModel.IsActive);
        }

        private T ShowDialogWithState<T>(Func<T> dialogAction)
        {
            _viewModel.IsDialogOpen = true;
            try
            {
                return dialogAction();
            }
            finally
            {
                _viewModel.IsDialogOpen = false;
            }
        }

        private void SyncSettings()
        {
            _settings.WindowLeft = this.Left;
            _settings.WindowTop = this.Top;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_viewModel.IsDirty)
            {
                _viewModel.IsDialogOpen = true;
                try
                {
                    if (_viewModel.RequestConfirmation?.Invoke("You have unsaved changes. Quit anyway?", "Quit Without Saving") != true)
                    {
                        e.Cancel = true;
                    }
                }
                finally
                {
                    _viewModel.IsDialogOpen = false;
                }
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isDragging || _isUpdatingPosition || !_settings.IsSnappingEnabled) return;

            var screen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var area = screen.WorkingArea;
            int snapDistance = RunTrackerOverlay.Models.Constants.SnapDistance;

            bool needsAdjustment = false;
            double newLeft = this.Left;
            double newTop = this.Top;

            // Check if we were snapped to top
            if (Math.Abs(this.Top - area.Top) < snapDistance)
            {
                newTop = area.Top;
                needsAdjustment = true;
            }
            // Check if we were snapped to bottom
            else if (Math.Abs((this.Top + e.PreviousSize.Height) - area.Bottom) < snapDistance)
            {
                newTop = area.Bottom - e.NewSize.Height;
                needsAdjustment = true;
            }

            // Check if we were snapped to left
            if (Math.Abs(this.Left - area.Left) < snapDistance)
            {
                newLeft = area.Left;
                needsAdjustment = true;
            }
            // Check if we were snapped to right
            else if (Math.Abs((this.Left + e.PreviousSize.Width) - area.Right) < snapDistance)
            {
                newLeft = area.Right - e.NewSize.Width;
                needsAdjustment = true;
            }

            if (needsAdjustment)
            {
                _isUpdatingPosition = true;
                this.Left = newLeft;
                this.Top = newTop;
                _isUpdatingPosition = false;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            _windowController.HandleSnapping(hwnd, msg, lParam, _settings, ref _isDragging, ref _mouseOffsetX, ref _mouseOffsetY, ref _virtualLeft, ref _virtualTop);
            return IntPtr.Zero;
        }

        private void ShowLootDialog()
        {
            if (_isLootDialogOpen) return;
            _isLootDialogOpen = true;
            _viewModel.IsDialogOpen = true;

            _hotkeyCoordinator.IsPaused = true;

            LootDialog lootDialog = new LootDialog();
            
            Point mousePoint = _windowController.GetCursorPosition();
            var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)mousePoint.X, (int)mousePoint.Y));
            var workArea = screen.WorkingArea;
            
            lootDialog.Left = workArea.Left + (workArea.Width - 300) / 2;
            lootDialog.Top = workArea.Top + (workArea.Height - 150) / 2;
            
            lootDialog.WindowStartupLocation = WindowStartupLocation.Manual;

            lootDialog.Owner = this;
            try
            {
                if (lootDialog.ShowDialog() == true)
                {
                    _timerEngine.AddLoot(lootDialog.LootResult);
                }
            }
            finally
            {
                _isLootDialogOpen = false;
                _viewModel.IsDialogOpen = false;
                _hotkeyCoordinator.IsPaused = false;
            }
        }


        protected override void OnClosed(EventArgs e)
        {
            SyncSettings();
            _settingsProvider.SaveSettings(_settings);
            CleanUp();

            base.OnClosed(e);
        }
    }
}
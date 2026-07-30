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
        private readonly MainViewModel _viewModel;
        private readonly ISessionLogger _sessionLogger;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IWindowController _windowController;
        private readonly IHotkeyCoordinator _hotkeyCoordinator;

        private DispatcherTimer? _displayTimer;

        private double _virtualLeft;
        private double _virtualTop;
        private int _mouseOffsetX;
        private int _mouseOffsetY;
        private bool _isDragging;
        private bool _isUpdatingPosition;

        public MainWindow(
            MainViewModel viewModel,
            AppSettings settings,
            ISessionLogger sessionLogger,
            ISettingsProvider settingsProvider,
            IWindowController windowController,
            IHotkeyCoordinator hotkeyCoordinator)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _settings = settings;
            _sessionLogger = sessionLogger;
            _settingsProvider = settingsProvider;
            _windowController = windowController;
            _hotkeyCoordinator = hotkeyCoordinator;

            DataContext = _viewModel;

            _viewModel.RequestClose = () => Close();

            _hotkeyCoordinator.Initialize(
                _settings, 
                _viewModel.TimerEngine, 
                () => Dispatcher.BeginInvoke(new Action(_viewModel.ShowLootDialog)), 
                () => Dispatcher.BeginInvoke(new Action(() => _windowController.ActivateWindow(this))));

            Activated += (s, e) => 
            { 
                if (!_viewModel.IsDialogOpen)
                {
                    _viewModel.IsActive = true; 
                    _windowController.SetClickThrough(this, false);
                }
            };

            Deactivated += (s, e) => 
            { 
                if (!_viewModel.IsDialogOpen)
                {
                    _viewModel.IsActive = false; 
                    _windowController.SetClickThrough(this, true);
                }
            };

            SizeChanged += MainWindow_SizeChanged;

            _displayTimer = new DispatcherTimer(DispatcherPriority.Render);
            _displayTimer.Interval = TimeSpan.FromMilliseconds(10);
            _displayTimer.Tick += (s, e) => _viewModel.UpdateDisplay();
            _displayTimer.Start();

            _viewModel.UpdateVisuals();
            _viewModel.UpdateTooltip();
            _viewModel.UpdateDisplay();
            _hotkeyCoordinator.UpdateSettings(_settings);
            _windowController.SetClickThrough(this, !_viewModel.IsActive);

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_viewModel.IsDirty)
            {
                if (_viewModel.DialogService.ShowConfirmationDialog("You have unsaved changes. Quit anyway?", "Quit Without Saving") != true)
                {
                    e.Cancel = true;
                }
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_isDragging || _isUpdatingPosition || !_settings.IsSnappingEnabled) return;

            var screen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var area = screen.WorkingArea;
            var bounds = screen.Bounds;
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
            else if (Math.Abs(this.Top - bounds.Top) < snapDistance)
            {
                newTop = bounds.Top;
                needsAdjustment = true;
            }
            // Check if we were snapped to bottom
            else if (Math.Abs((this.Top + e.PreviousSize.Height) - area.Bottom) < snapDistance)
            {
                newTop = area.Bottom - e.NewSize.Height;
                needsAdjustment = true;
            }
            else if (Math.Abs((this.Top + e.PreviousSize.Height) - bounds.Bottom) < snapDistance)
            {
                newTop = bounds.Bottom - e.NewSize.Height;
                needsAdjustment = true;
            }

            // Check if we were snapped to left
            if (Math.Abs(this.Left - area.Left) < snapDistance)
            {
                newLeft = area.Left;
                needsAdjustment = true;
            }
            else if (Math.Abs(this.Left - bounds.Left) < snapDistance)
            {
                newLeft = bounds.Left;
                needsAdjustment = true;
            }
            // Check if we were snapped to right
            else if (Math.Abs((this.Left + e.PreviousSize.Width) - area.Right) < snapDistance)
            {
                newLeft = area.Right - e.NewSize.Width;
                needsAdjustment = true;
            }
            else if (Math.Abs((this.Left + e.PreviousSize.Width) - bounds.Right) < snapDistance)
            {
                newLeft = bounds.Right - e.NewSize.Width;
                needsAdjustment = true;
            }

            if (needsAdjustment)
            {
                _isUpdatingPosition = true;
                _viewModel.WindowLeft = newLeft;
                _viewModel.WindowTop = newTop;
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
            
            if (_isDragging)
            {
                _viewModel.WindowLeft = this.Left;
                _viewModel.WindowTop = this.Top;
            }

            // if (msg == NativeMethods.WM_ACTIVATE)
            // {
            //     int loword = (int)((long)wParam & 0xFFFF);
            //     if (loword == NativeMethods.WA_INACTIVE)
            //     {
            //         if (!_viewModel.IsDialogOpen)
            //         {
            //             _viewModel.IsActive = false;
            //             _windowController.SetClickThrough(this, true);
            //         }
            //     }
            // }

            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            _settings.WindowLeft = _viewModel.WindowLeft;
            _settings.WindowTop = _viewModel.WindowTop;
            _settingsProvider.SaveSettings(_settings);
            CleanUp();

            base.OnClosed(e);
        }
    }
}
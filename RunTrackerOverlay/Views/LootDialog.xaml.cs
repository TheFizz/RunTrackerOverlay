using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using RunTrackerOverlay.Services;

namespace RunTrackerOverlay.Views
{
    public partial class LootDialog : Window
    {
        public string LootResult { get; private set; } = string.Empty;

        public LootDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Center the window more accurately once we know its real size if it's manual
            if (WindowStartupLocation == WindowStartupLocation.Manual)
            {
                // Re-calculate to center perfectly
                NativeMethods.POINT mousePoint;
                if (NativeMethods.GetCursorPos(out mousePoint))
                {
                    var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(mousePoint.X, mousePoint.Y));
                    var workArea = screen.WorkingArea;
                    Left = workArea.Left + (workArea.Width - ActualWidth) / 2;
                    Top = workArea.Top + (workArea.Height - ActualHeight) / 2;
                }
            }

            ForceForeground();

            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                LootInput.Focus();
                Keyboard.Focus(LootInput);
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void ForceForeground()
        {
            var handle = new WindowInteropHelper(this).Handle;
            
            // Get the current foreground window and its thread ID
            IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
            if (foregroundWindow == handle)
            {
                this.Activate();
                return;
            }

            uint foregroundThreadId = NativeMethods.GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
            uint nativeThreadId = NativeMethods.GetCurrentThreadId();

            if (foregroundThreadId != nativeThreadId && foregroundThreadId != 0)
            {
                // Attach our input processing thread to the foreground window's thread
                NativeMethods.AttachThreadInput(nativeThreadId, foregroundThreadId, true);
                
                // Bring to foreground
                NativeMethods.SetForegroundWindow(handle);
                this.Activate();
                
                // Detach
                NativeMethods.AttachThreadInput(nativeThreadId, foregroundThreadId, false);
            }
            else
            {
                NativeMethods.SetForegroundWindow(handle);
                this.Activate();
            }
            
            // Also try to set focus directly via API
            NativeMethods.SetFocus(handle);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LootInput.Text))
            {
                DialogResult = false;
            }
            else
            {
                LootResult = LootInput.Text;
                DialogResult = true;
            }
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void LootInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveButton_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                CancelButton_Click(sender, e);
            }
        }
    }
}

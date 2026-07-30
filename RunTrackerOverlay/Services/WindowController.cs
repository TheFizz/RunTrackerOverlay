using System;
using System.Windows;
using System.Windows.Interop;

using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.Services
{
    public interface IWindowController
    {
        void SetClickThrough(Window window, bool clickThrough);
        void ActivateWindow(Window window);
        Point GetCursorPosition();
        void HandleSnapping(IntPtr hwnd, int msg, IntPtr lParam, AppSettings settings, ref bool isDragging, ref int mouseOffsetX, ref int mouseOffsetY, ref double virtualLeft, ref double virtualTop);
    }

    public class WindowController : IWindowController
    {
        public void SetClickThrough(Window window, bool clickThrough)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            int extendedStyle = NativeMethods.GetWindowLong(handle, NativeMethods.GWL_EXSTYLE);
            int newStyle = clickThrough 
                ? (extendedStyle | NativeMethods.WS_EX_TRANSPARENT) 
                : (extendedStyle & ~NativeMethods.WS_EX_TRANSPARENT);

            if (extendedStyle != newStyle)
            {
                NativeMethods.SetWindowLong(handle, NativeMethods.GWL_EXSTYLE, newStyle);
            }
        }

        public void ActivateWindow(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            
            NativeMethods.SetForegroundWindow(handle);
            NativeMethods.BringWindowToTop(handle);
            window.Activate();
        }

        public Point GetCursorPosition()
        {
            NativeMethods.GetCursorPos(out NativeMethods.POINT pt);
            return new Point(pt.X, pt.Y);
        }

        public void HandleSnapping(IntPtr hwnd, int msg, IntPtr lParam, AppSettings settings, ref bool isDragging, ref int mouseOffsetX, ref int mouseOffsetY, ref double virtualLeft, ref double virtualTop)
        {
            if (msg == NativeMethods.WM_ENTERSIZEMOVE)
            {
                isDragging = true;
                NativeMethods.GetWindowRect(hwnd, out NativeMethods.RECT rect);
                NativeMethods.GetCursorPos(out NativeMethods.POINT pt);
                mouseOffsetX = pt.X - rect.Left;
                mouseOffsetY = pt.Y - rect.Top;
                virtualLeft = rect.Left;
                virtualTop = rect.Top;
            }
            else if (msg == NativeMethods.WM_EXITSIZEMOVE)
            {
                isDragging = false;
            }
            else if (msg == NativeMethods.WM_MOVING && isDragging && settings.IsSnappingEnabled)
            {
                NativeMethods.RECT rect = (NativeMethods.RECT)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(NativeMethods.RECT))!;
                NativeMethods.GetCursorPos(out NativeMethods.POINT pt);
                virtualLeft = pt.X - mouseOffsetX;
                virtualTop = pt.Y - mouseOffsetY;
                
                var screen = System.Windows.Forms.Screen.FromHandle(hwnd);
                var area = screen.WorkingArea;
                var bounds = screen.Bounds; // Full screen area including taskbar

                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                int snapDistance = 20;

                // Horizontal Snapping (Left/Right)
                if (Math.Abs(virtualLeft - area.Left) < snapDistance)
                {
                    rect.Left = area.Left;
                    rect.Right = rect.Left + width;
                }
                else if (Math.Abs(virtualLeft - bounds.Left) < snapDistance) // True Left
                {
                    rect.Left = bounds.Left;
                    rect.Right = rect.Left + width;
                }
                else if (Math.Abs((virtualLeft + width) - area.Right) < snapDistance)
                {
                    rect.Right = area.Right;
                    rect.Left = rect.Right - width;
                }
                else if (Math.Abs((virtualLeft + width) - bounds.Right) < snapDistance) // True Right
                {
                    rect.Right = bounds.Right;
                    rect.Left = rect.Right - width;
                }
                else
                {
                    rect.Left = (int)Math.Round(virtualLeft);
                    rect.Right = rect.Left + width;
                }

                // Vertical Snapping (Top/Bottom)
                if (Math.Abs(virtualTop - area.Top) < snapDistance)
                {
                    rect.Top = area.Top;
                    rect.Bottom = rect.Top + height;
                }
                else if (Math.Abs(virtualTop - bounds.Top) < snapDistance) // True Top
                {
                    rect.Top = bounds.Top;
                    rect.Bottom = rect.Top + height;
                }
                else if (Math.Abs((virtualTop + height) - area.Bottom) < snapDistance)
                {
                    rect.Bottom = area.Bottom;
                    rect.Top = rect.Bottom - height;
                }
                else if (Math.Abs((virtualTop + height) - bounds.Bottom) < snapDistance) // True Bottom
                {
                    rect.Bottom = bounds.Bottom;
                    rect.Top = rect.Bottom - height;
                }
                else
                {
                    rect.Top = (int)Math.Round(virtualTop);
                    rect.Bottom = rect.Top + height;
                }

                System.Runtime.InteropServices.Marshal.StructureToPtr(rect, lParam, true);
            }
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace RunTrackerOverlay.Services
{
    public class GlobalHotkeyService : IDisposable
    {
        private IntPtr _hookID = IntPtr.Zero;
        private readonly NativeMethods.LowLevelKeyboardProc _proc;

        public event Action? OnRunStopPressed;
        public event Action? OnFocusPressed;
        public event Action? OnContinuousStopPressed;
        public event Action? OnPauseResumePressed;
        public event Action? OnLootPressed;

        public uint RunStopKey { get; set; }
        public uint FocusKey { get; set; }
        public uint ContinuousStopKey { get; set; }
        public uint PauseResumeKey { get; set; }
        public uint LootKey { get; set; }
        public bool IsPaused { get; set; }

        public GlobalHotkeyService()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        private IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                if (curModule == null) return IntPtr.Zero;
                return NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (IsPaused)
            {
                return NativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            if (nCode >= 0 && (wParam == (IntPtr)NativeMethods.WM_KEYDOWN || wParam == (IntPtr)NativeMethods.WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == RunStopKey)
                {
                    OnRunStopPressed?.Invoke();
                }
                else if (vkCode == FocusKey)
                {
                    OnFocusPressed?.Invoke();
                    return (IntPtr)1;
                }
                else if (vkCode == ContinuousStopKey)
                {
                    OnContinuousStopPressed?.Invoke();
                }
                else if (vkCode == PauseResumeKey)
                {
                    OnPauseResumePressed?.Invoke();
                }
                else if (vkCode == LootKey)
                {
                    OnLootPressed?.Invoke();
                    return (IntPtr)1;
                }
            }
            return NativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_hookID != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        ~GlobalHotkeyService()
        {
            Dispose(false);
        }
    }
}

using System;
using System.Windows.Input;

using RunTrackerOverlay.Models;
using RunTrackerOverlay.Services;
using RunTrackerOverlay.ViewModels;

namespace RunTrackerOverlay.Services
{
    public interface IHotkeyCoordinator : IDisposable
    {
        void Initialize(AppSettings settings, TimerEngine timerEngine, Action showLootDialog, Action onFocusPressed);
        void UpdateSettings(AppSettings settings);
        bool IsPaused { get; set; }
    }

    public class HotkeyCoordinator : IHotkeyCoordinator
    {
        private readonly GlobalHotkeyService _hotkeyService;
        private TimerEngine? _timerEngine;
        private Action? _showLootDialog;
        private Action? _onFocusPressed;

        public HotkeyCoordinator()
        {
            _hotkeyService = new GlobalHotkeyService();
        }

        public bool IsPaused
        {
            get => _hotkeyService.IsPaused;
            set => _hotkeyService.IsPaused = value;
        }

        public void Initialize(AppSettings settings, TimerEngine timerEngine, Action showLootDialog, Action onFocusPressed)
        {
            _timerEngine = timerEngine;
            _showLootDialog = showLootDialog;
            _onFocusPressed = onFocusPressed;

            _hotkeyService.OnRunStopPressed += () => _timerEngine.HandlePause();
            _hotkeyService.OnFocusPressed += () => _onFocusPressed?.Invoke();
            _hotkeyService.OnContinuousStopPressed += () => _timerEngine.HandleContinuousStop();
            _hotkeyService.OnLootPressed += () => _showLootDialog?.Invoke();

            UpdateSettings(settings);
        }

        public void UpdateSettings(AppSettings settings)
        {
            _hotkeyService.RunStopKey = (uint)KeyInterop.VirtualKeyFromKey(settings.ActivationKey);
            _hotkeyService.FocusKey = (uint)KeyInterop.VirtualKeyFromKey(settings.FocusKey);
            _hotkeyService.ContinuousStopKey = (uint)KeyInterop.VirtualKeyFromKey(settings.PauseKey);
            _hotkeyService.LootKey = (uint)KeyInterop.VirtualKeyFromKey(settings.LootKey);
        }

        public void Dispose()
        {
            _hotkeyService.Dispose();
        }
    }
}

using System;
using System.Windows.Input;

using RunTrackerOverlay.Models;

namespace RunTrackerOverlay.ViewModels
{
    public class OptionsViewModel : ViewModelBase
    {
        private bool _isSnappingEnabled;
        private bool _showKeysTooltip;
        private bool _isContinuousMode;
        private bool _showBest;
        private bool _showLast;
        private bool _showWorst;
        private bool _showAvg;
        private bool _showTotal;
        private bool _showSessionName;
        private bool _hideMilliseconds;
        private string _sessionName = "";
        private double _windowOpacity;
        private double _textOpacity;
        private Key _activationKey;
        private Key _focusKey;
        private Key _pauseKey;
        private Key _lootKey;

        private bool _isListeningForActivationKey;
        private bool _isListeningForFocusKey;
        private bool _isListeningForPauseKey;
        private bool _isListeningForLootKey;

        private string _activationKeyText = "";
        private string _focusKeyText = "";
        private string _pauseKeyText = "";
        private string _lootKeyText = "";

        public OptionsViewModel(AppSettings settings)
        {
            IsSnappingEnabled = settings.IsSnappingEnabled;
            ShowKeysTooltip = settings.ShowKeysTooltip;
            IsContinuousMode = settings.IsContinuousMode;
            ShowBest = settings.ShowBest;
            ShowLast = settings.ShowLast;
            ShowWorst = settings.ShowWorst;
            ShowAvg = settings.ShowAvg;
            ShowTotal = settings.ShowTotal;
            ShowSessionName = settings.ShowSessionName;
            HideMilliseconds = settings.HideMilliseconds;
            SessionName = settings.SessionName;
            WindowOpacity = settings.WindowOpacity;
            TextOpacity = settings.TextOpacity;
            ActivationKey = settings.ActivationKey;
            FocusKey = settings.FocusKey;
            PauseKey = settings.PauseKey;
            LootKey = settings.LootKey;

            UpdateKeyTexts();
        }

        public bool IsSnappingEnabled 
        { 
            get => _isSnappingEnabled; 
            set 
            {
                if (SetProperty(ref _isSnappingEnabled, value))
                    SettingChanged?.Invoke(this, nameof(IsSnappingEnabled));
            } 
        }
        public bool ShowKeysTooltip 
        { 
            get => _showKeysTooltip; 
            set 
            {
                if (SetProperty(ref _showKeysTooltip, value))
                    SettingChanged?.Invoke(this, nameof(ShowKeysTooltip));
            } 
        }
        public bool IsContinuousMode 
        { 
            get => _isContinuousMode; 
            set 
            {
                if (SetProperty(ref _isContinuousMode, value))
                    SettingChanged?.Invoke(this, nameof(IsContinuousMode));
            } 
        }
        public bool ShowBest 
        { 
            get => _showBest; 
            set 
            {
                if (SetProperty(ref _showBest, value))
                    SettingChanged?.Invoke(this, nameof(ShowBest));
            } 
        }
        public bool ShowLast 
        { 
            get => _showLast; 
            set 
            {
                if (SetProperty(ref _showLast, value))
                    SettingChanged?.Invoke(this, nameof(ShowLast));
            } 
        }
        public bool ShowWorst 
        { 
            get => _showWorst; 
            set 
            {
                if (SetProperty(ref _showWorst, value))
                    SettingChanged?.Invoke(this, nameof(ShowWorst));
            } 
        }
        public bool ShowAvg 
        { 
            get => _showAvg; 
            set 
            {
                if (SetProperty(ref _showAvg, value))
                    SettingChanged?.Invoke(this, nameof(ShowAvg));
            } 
        }
        public bool ShowTotal 
        { 
            get => _showTotal; 
            set 
            {
                if (SetProperty(ref _showTotal, value))
                    SettingChanged?.Invoke(this, nameof(ShowTotal));
            } 
        }
        public bool ShowSessionName 
        { 
            get => _showSessionName; 
            set 
            {
                if (SetProperty(ref _showSessionName, value))
                    SettingChanged?.Invoke(this, nameof(ShowSessionName));
            } 
        }
        public bool HideMilliseconds 
        { 
            get => _hideMilliseconds; 
            set 
            {
                if (SetProperty(ref _hideMilliseconds, value))
                    SettingChanged?.Invoke(this, nameof(HideMilliseconds));
            }
        }
        public string SessionName 
        { 
            get => _sessionName; 
            set 
            {
                if (SetProperty(ref _sessionName, value))
                    SettingChanged?.Invoke(this, nameof(SessionName));
            }
        }
        public double WindowOpacity 
        { 
            get => _windowOpacity; 
            set 
            {
                if (SetProperty(ref _windowOpacity, value))
                    SettingChanged?.Invoke(this, nameof(WindowOpacity));
            }
        }
        public double TextOpacity 
        { 
            get => _textOpacity; 
            set 
            {
                if (SetProperty(ref _textOpacity, value))
                    SettingChanged?.Invoke(this, nameof(TextOpacity));
            }
        }
        public Key ActivationKey { get => _activationKey; set { _activationKey = value; UpdateKeyTexts(); SettingChanged?.Invoke(this, nameof(ActivationKey)); } }
        public Key FocusKey { get => _focusKey; set { _focusKey = value; UpdateKeyTexts(); SettingChanged?.Invoke(this, nameof(FocusKey)); } }
        public Key PauseKey { get => _pauseKey; set { _pauseKey = value; UpdateKeyTexts(); SettingChanged?.Invoke(this, nameof(PauseKey)); } }
        public Key LootKey { get => _lootKey; set { _lootKey = value; UpdateKeyTexts(); SettingChanged?.Invoke(this, nameof(LootKey)); } }

        public string ActivationKeyText { get => _activationKeyText; private set => SetProperty(ref _activationKeyText, value); }
        public string FocusKeyText { get => _focusKeyText; private set => SetProperty(ref _focusKeyText, value); }
        public string PauseKeyText { get => _pauseKeyText; private set => SetProperty(ref _pauseKeyText, value); }
        public string LootKeyText { get => _lootKeyText; private set => SetProperty(ref _lootKeyText, value); }

        public event EventHandler<string>? SettingChanged;
        
        private void UpdateKeyTexts()
        {
            ActivationKeyText = _isListeningForActivationKey ? "Press any key..." : ActivationKey.ToString();
            FocusKeyText = _isListeningForFocusKey ? "Press any key..." : FocusKey.ToString();
            PauseKeyText = _isListeningForPauseKey ? "Press any key..." : PauseKey.ToString();
            LootKeyText = _isListeningForLootKey ? "Press any key..." : LootKey.ToString();
        }

        public void StartListeningForActivation() { ResetListening(); _isListeningForActivationKey = true; UpdateKeyTexts(); }
        public void StartListeningForFocus() { ResetListening(); _isListeningForFocusKey = true; UpdateKeyTexts(); }
        public void StartListeningForPause() { ResetListening(); _isListeningForPauseKey = true; UpdateKeyTexts(); }
        public void StartListeningForLoot() { ResetListening(); _isListeningForLootKey = true; UpdateKeyTexts(); }

        private void ResetListening()
        {
            _isListeningForActivationKey = false;
            _isListeningForFocusKey = false;
            _isListeningForPauseKey = false;
            _isListeningForLootKey = false;
        }

        public bool HandleKeyDown(Key key)
        {
            if (_isListeningForActivationKey)
            {
                ActivationKey = key;
                _isListeningForActivationKey = false;
                UpdateKeyTexts();
                return true;
            }
            if (_isListeningForFocusKey)
            {
                FocusKey = key;
                _isListeningForFocusKey = false;
                UpdateKeyTexts();
                return true;
            }
            if (_isListeningForPauseKey)
            {
                PauseKey = key;
                _isListeningForPauseKey = false;
                UpdateKeyTexts();
                return true;
            }
            if (_isListeningForLootKey)
            {
                LootKey = key;
                _isListeningForLootKey = false;
                UpdateKeyTexts();
                return true;
            }
            return false;
        }

        public void ApplyTo(AppSettings settings)
        {
            settings.IsSnappingEnabled = IsSnappingEnabled;
            settings.ShowKeysTooltip = ShowKeysTooltip;
            settings.IsContinuousMode = IsContinuousMode;
            settings.ShowBest = ShowBest;
            settings.ShowLast = ShowLast;
            settings.ShowWorst = ShowWorst;
            settings.ShowAvg = ShowAvg;
            settings.ShowTotal = ShowTotal;
            settings.ShowSessionName = ShowSessionName;
            settings.HideMilliseconds = HideMilliseconds;
            settings.SessionName = SessionName;
            settings.WindowOpacity = WindowOpacity;
            settings.TextOpacity = TextOpacity;
            settings.ActivationKey = ActivationKey;
            settings.FocusKey = FocusKey;
            settings.PauseKey = PauseKey;
            settings.LootKey = LootKey;
        }
    }
}

using System;
using System.Windows.Input;
using RunTrackerOverlay.Models;
using RunTrackerOverlay.Services;

namespace RunTrackerOverlay.ViewModels
{
    public class OptionsViewModel : ViewModelBase
    {
        private string _sessionName = "";
        private bool _isSnappingEnabled;
        private bool _showKeysTooltip;
        private TrackerMode _mode;
        private bool _showBest;
        private bool _showLast;
        private bool _showWorst;
        private bool _showAvg;
        private bool _showTotal;
        private bool _showLoot;
        private bool _showSessionName;
        private bool _showRunCount;
        private string _timerFormat = "";
        private bool _isTimerFormatInvalid;
        private bool _applyFormatToStats;
        private double _windowOpacity;
        private double _textOpacity;
        private Key _startStopNextKey;
        private Key _focusKey;
        private Key _stopContKey;
        private Key _pauseResumeKey;
        private Key _lootKey;

        private bool _isListeningForStartStopNextKey;
        private bool _isListeningForFocusKey;
        private bool _isListeningForStopContKey;
        private bool _isListeningForPauseResumeKey;
        private bool _isListeningForLootKey;

        private string _startStopNextKeyText = "";
        private string _focusKeyText = "";
        private string _stopContKeyText = "";
        private string _pauseResumeKeyText = "";
        private string _lootKeyText = "";

        public OptionsViewModel(AppSettings settings, string currentSessionName)
        {
            SessionName = currentSessionName;
            IsSnappingEnabled = settings.IsSnappingEnabled;
            ShowKeysTooltip = settings.ShowKeysTooltip;
            Mode = settings.Mode;
            ShowBest = settings.ShowBest;
            ShowLast = settings.ShowLast;
            ShowWorst = settings.ShowWorst;
            ShowAvg = settings.ShowAvg;
            ShowTotal = settings.ShowTotal;
            ShowLoot = settings.ShowLoot;
            ShowSessionName = settings.ShowSessionName;
            ShowRunCount = settings.ShowRunCount;
            TimerFormat = settings.TimerFormat;
            _isTimerFormatInvalid = !TimeUtils.IsValidFormat(TimerFormat);
            ApplyFormatToStats = settings.ApplyFormatToStats;
            WindowOpacity = settings.WindowOpacity;
            TextOpacity = settings.TextOpacity;
            StartStopNextKey = settings.StartStopNextKey;
            FocusKey = settings.FocusKey;
            StopContKey = settings.StopContKey;
            PauseResumeKey = settings.PauseResumeKey;
            LootKey = settings.LootKey;

            UpdateKeyTexts();
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
        public TrackerMode Mode 
        { 
            get => _mode; 
            set 
            {
                if (SetProperty(ref _mode, value))
                    SettingChanged?.Invoke(this, nameof(Mode));
            } 
        }

        public TrackerMode[] AllModes => (TrackerMode[])Enum.GetValues(typeof(TrackerMode));

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
        public bool ShowLoot 
        { 
            get => _showLoot; 
            set 
            {
                if (SetProperty(ref _showLoot, value))
                    SettingChanged?.Invoke(this, nameof(ShowLoot));
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
        public bool ShowRunCount 
        { 
            get => _showRunCount; 
            set 
            {
                if (SetProperty(ref _showRunCount, value))
                    SettingChanged?.Invoke(this, nameof(ShowRunCount));
            } 
        }
        public string TimerFormat 
        { 
            get => _timerFormat; 
            set 
            {
                if (SetProperty(ref _timerFormat, value))
                {
                    IsTimerFormatInvalid = !TimeUtils.IsValidFormat(value);
                    SettingChanged?.Invoke(this, nameof(TimerFormat));
                }
            }
        }
        public bool IsTimerFormatInvalid
        {
            get => _isTimerFormatInvalid;
            private set => SetProperty(ref _isTimerFormatInvalid, value);
        }
        public bool ApplyFormatToStats 
        { 
            get => _applyFormatToStats; 
            set 
            {
                if (SetProperty(ref _applyFormatToStats, value))
                    SettingChanged?.Invoke(this, nameof(ApplyFormatToStats));
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
        public Key StartStopNextKey { get => _startStopNextKey; set { _startStopNextKey = value; UpdateKeyTexts(); SettingChanged?.Invoke(this, nameof(StartStopNextKey)); } }
        public Key FocusKey { get => _focusKey; set { _focusKey = value; UpdateKeyTexts(); SettingChanged?.Invoke(this, nameof(FocusKey)); } }
        public Key StopContKey { get => _stopContKey; set { _stopContKey = value; UpdateKeyTexts(); SettingChanged?.Invoke(this, nameof(StopContKey)); } }
        public Key PauseResumeKey { get => _pauseResumeKey; set { _pauseResumeKey = value; UpdateKeyTexts(); SettingChanged?.Invoke(this, nameof(PauseResumeKey)); } }
        public Key LootKey { get => _lootKey; set { _lootKey = value; UpdateKeyTexts(); SettingChanged?.Invoke(this, nameof(LootKey)); } }

        public string StartStopNextKeyText { get => _startStopNextKeyText; private set => SetProperty(ref _startStopNextKeyText, value); }
        public string FocusKeyText { get => _focusKeyText; private set => SetProperty(ref _focusKeyText, value); }
        public string StopContKeyText { get => _stopContKeyText; private set => SetProperty(ref _stopContKeyText, value); }
        public string PauseResumeKeyText { get => _pauseResumeKeyText; private set => SetProperty(ref _pauseResumeKeyText, value); }
        public string LootKeyText { get => _lootKeyText; private set => SetProperty(ref _lootKeyText, value); }

        public event EventHandler<string>? SettingChanged;
        
        private void UpdateKeyTexts()
        {
            StartStopNextKeyText = _isListeningForStartStopNextKey ? "Press any key..." : StartStopNextKey.ToString();
            FocusKeyText = _isListeningForFocusKey ? "Press any key..." : FocusKey.ToString();
            StopContKeyText = _isListeningForStopContKey ? "Press any key..." : StopContKey.ToString();
            PauseResumeKeyText = _isListeningForPauseResumeKey ? "Press any key..." : PauseResumeKey.ToString();
            LootKeyText = _isListeningForLootKey ? "Press any key..." : LootKey.ToString();
        }

        public void StartListeningForStartStopNext() { ResetListening(); _isListeningForStartStopNextKey = true; UpdateKeyTexts(); }
        public void StartListeningForFocus() { ResetListening(); _isListeningForFocusKey = true; UpdateKeyTexts(); }
        public void StartListeningForStopCont() { ResetListening(); _isListeningForStopContKey = true; UpdateKeyTexts(); }
        public void StartListeningForPauseResume() { ResetListening(); _isListeningForPauseResumeKey = true; UpdateKeyTexts(); }
        public void StartListeningForLoot() { ResetListening(); _isListeningForLootKey = true; UpdateKeyTexts(); }

        private void ResetListening()
        {
            _isListeningForStartStopNextKey = false;
            _isListeningForFocusKey = false;
            _isListeningForStopContKey = false;
            _isListeningForPauseResumeKey = false;
            _isListeningForLootKey = false;
        }

        public bool HandleKeyDown(Key key)
        {
            if (_isListeningForStartStopNextKey)
            {
                StartStopNextKey = key;
                _isListeningForStartStopNextKey = false;
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
            if (_isListeningForStopContKey)
            {
                StopContKey = key;
                _isListeningForStopContKey = false;
                UpdateKeyTexts();
                return true;
            }
            if (_isListeningForPauseResumeKey)
            {
                PauseResumeKey = key;
                _isListeningForPauseResumeKey = false;
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
            settings.Mode = Mode;
            settings.ShowBest = ShowBest;
            settings.ShowLast = ShowLast;
            settings.ShowWorst = ShowWorst;
            settings.ShowAvg = ShowAvg;
            settings.ShowTotal = ShowTotal;
            settings.ShowLoot = ShowLoot;
            settings.ShowSessionName = ShowSessionName;
            settings.ShowRunCount = ShowRunCount;
            settings.TimerFormat = TimerFormat;
            settings.ApplyFormatToStats = ApplyFormatToStats;
            settings.WindowOpacity = WindowOpacity;
            settings.TextOpacity = TextOpacity;
            settings.StartStopNextKey = StartStopNextKey;
            settings.FocusKey = FocusKey;
            settings.StopContKey = StopContKey;
            settings.PauseResumeKey = PauseResumeKey;
            settings.LootKey = LootKey;
        }
    }
}

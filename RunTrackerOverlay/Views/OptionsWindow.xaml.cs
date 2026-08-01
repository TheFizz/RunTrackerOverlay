using System;
using System.Windows;
using System.Windows.Input;

using RunTrackerOverlay.Models;
using RunTrackerOverlay.ViewModels;

namespace RunTrackerOverlay.Views
{
    public partial class OptionsWindow : Window
    {
        private readonly AppSettings _settings;
        public OptionsViewModel ViewModel { get; }

        public OptionsWindow(AppSettings settings, string currentSessionName)
        {
            InitializeComponent();
            _settings = settings;
            ViewModel = new OptionsViewModel(settings, currentSessionName);
            DataContext = ViewModel;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void KeySelectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartListeningForStartStopNext();
        }

        private void StopContKeySelectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartListeningForStopCont();
        }

        private void PauseResumeKeySelectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartListeningForPauseResume();
        }

        private void FocusKeySelectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartListeningForFocus();
        }

        private void LootKeySelectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartListeningForLoot();
        }

        private void OptionsWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (ViewModel.HandleKeyDown(e.Key))
            {
                e.Handled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyTo(_settings);
            DialogResult = true;
            Close();
        }
    }
}

using System;
using System.Windows;
using System.Windows.Input;

using RunTrackerOverlay.Models;
using RunTrackerOverlay.ViewModels;

namespace RunTrackerOverlay.Views
{
    public partial class OptionsWindow : Window
    {
        public OptionsViewModel ViewModel { get; }

        public OptionsWindow(AppSettings settings)
        {
            InitializeComponent();
            ViewModel = new OptionsViewModel(settings);
            DataContext = ViewModel;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void KeySelectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartListeningForActivation();
        }

        private void PauseKeySelectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartListeningForPause();
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
            DialogResult = true;
            Close();
        }
    }
}

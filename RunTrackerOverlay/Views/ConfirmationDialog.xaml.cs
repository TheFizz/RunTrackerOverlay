using System.Windows;
using System.Windows.Input;

namespace RunTrackerOverlay.Views
{
    public partial class ConfirmationDialog : Window
    {
        public ConfirmationDialog(string message, string title = "Confirmation")
        {
            InitializeComponent();
            this.Title = title;
            MessageText.Text = message;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}

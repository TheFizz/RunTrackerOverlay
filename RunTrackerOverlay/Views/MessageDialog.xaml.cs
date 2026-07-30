using System.Windows;
using System.Windows.Input;

namespace RunTrackerOverlay.Views
{
    public partial class MessageDialog : Window
    {
        public MessageDialog(string message, string title = "Message", bool isError = false)
        {
            InitializeComponent();
            this.Title = title;
            TitleText.Text = title;
            MessageText.Text = message;
            
            if (isError)
            {
                TitleText.Foreground = System.Windows.Media.Brushes.IndianRed;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}

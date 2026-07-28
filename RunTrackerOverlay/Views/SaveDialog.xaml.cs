using System.Windows;
using System.Windows.Input;

namespace RunTrackerOverlay.Views
{
    public partial class SaveDialog : Window
    {
        public bool? IncludeEmpty { get; private set; }

        public SaveDialog(string filename)
        {
            InitializeComponent();
            MessageText.Text = $"Session will be saved as {filename}";
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            IncludeEmpty = true;
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            IncludeEmpty = false;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IncludeEmpty = null;
            DialogResult = false;
            Close();
        }
    }
}

using System.Windows;

namespace Districts.New.Gui
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        private void ConfirmAndClose(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void JustClose(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}

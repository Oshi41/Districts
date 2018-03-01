using System.Windows;

namespace Districts.Views.Management
{
    /// <summary>
    /// Логика взаимодействия для ManageView.xaml
    /// </summary>
    public partial class ManageView : Window
    {
        public ManageView()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

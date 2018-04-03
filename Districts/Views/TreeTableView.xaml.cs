using System.Windows;

namespace Districts.Views
{
    /// <summary>
    ///     Логика взаимодействия для TreeTableView.xaml
    /// </summary>
    public partial class TreeTableView : Window
    {
        public TreeTableView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
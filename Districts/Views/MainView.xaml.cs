using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Districts.Settings;

namespace Districts.Views
{
    /// <summary>
    ///     Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private ApplicationSettings settings;

        public MainView()
        {
            InitializeComponent();
            settings = ApplicationSettings.ReadOrCreate();
        }

        private void FillImage(object sender, RoutedEventArgs e)
        {
            if (sender is Image image)
            {
                var icon = Properties.Resources.gIcon;
            }
        }
    }
}
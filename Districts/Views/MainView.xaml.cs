using System.Windows;
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
    }
}
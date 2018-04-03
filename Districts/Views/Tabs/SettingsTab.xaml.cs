using System.Windows;
using System.Windows.Controls;

namespace Districts.Views.Tabs
{
    /// <summary>
    ///     Interaction logic for SettingsTab.xaml
    /// </summary>
    public partial class SettingsTab : UserControl
    {
        public SettingsTab()
        {
            InitializeComponent();
        }

        private void CloseSettingsEdit(object sender, RoutedEventArgs e)
        {
            SettingsButton.IsChecked = false;
            //SettingsButton.Command?.Execute(SettingsButton.IsChecked);
        }
    }
}
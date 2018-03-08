using System.IO;
using System.Windows;
using System.Windows.Controls;
using Districts.Settings;

namespace Districts.Views.Tabs
{
    /// <summary>
    /// Interaction logic for SettingsTab.xaml
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

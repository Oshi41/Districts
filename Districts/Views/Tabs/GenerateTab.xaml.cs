using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Districts.Views.Tabs
{
    /// <summary>
    ///     Interaction logic for GenerateTab.xaml
    /// </summary>
    public partial class GenerateTab : UserControl
    {
        public GenerateTab()
        {
            InitializeComponent();
        }

        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            //ToggleButton.IsChecked = false;
        }

        private void Switch(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink link)
                if (link.Tag is TabControl tab)
                    tab.SelectedIndex = 0;
        }
    }
}
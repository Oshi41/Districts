using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Districts.Views.Tabs
{
    /// <summary>
    ///     Interaction logic for DownloadTab.xaml
    /// </summary>
    public partial class DownloadTab : UserControl
    {
        public DownloadTab()
        {
            InitializeComponent();
        }

        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            //ToggleButton.IsChecked = false;
        }

        /// <summary>
        ///     Для отладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectText(object sender, RoutedEventArgs e)
        {
            //var menu = new ContextMenu();
            //var item = new MenuItem();
            //item.Click += (o, args) => { Box.SelectionLength = 0; };

            //menu.Items.Add(item);
            //Box.ContextMenu = menu;
        }

        private void Switch(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink link)
                if (link.Tag is TabControl tab)
                    tab.SelectedIndex = 0;
        }
    }
}
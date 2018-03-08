using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Districts.Helper;

namespace Districts.Views.Tabs
{
    /// <summary>
    /// Interaction logic for EditTreeTab.xaml
    /// </summary>
    public partial class EditTreeTab : UserControl
    {
        public EditTreeTab()
        {
            InitializeComponent();
        }

        private void BlocReleaseTab(object sender)
        {
            if (!(sender is ToggleButton button)) return;

            var pressed = button.IsChecked ?? false;
            var tab = WpfHelper.GetFirstParentByType<TabControl>(button);
            if (tab == null) return;

            if (pressed)
            {
                var selected = tab.SelectedItem as TabControl;
                foreach (var item in tab.Items.OfType<TabItem>())
                {
                    item.IsEnabled = false;
                }

                if (selected != null)
                    selected.IsEnabled = true;

            }
            else
            {
                foreach (var item in tab.Items.OfType<TabItem>())
                {
                    item.IsEnabled = true;
                }
            }
        }

        private void BlockTab(object sender, RoutedEventArgs e)
        {
            BlocReleaseTab(sender);
        }

        private void CloseEditing(object sender, RoutedEventArgs e)
        {
            ToggleButton.IsChecked = false;
            //ToggleButton.Command?.Execute(ToggleButton.IsChecked);
        }
    }
}

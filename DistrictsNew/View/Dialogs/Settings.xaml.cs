using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace DistrictsNew.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void AllowOnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            if (!e.Text.All(char.IsDigit))
            {
                e.Handled = true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Districts.Helper;

namespace Districts.New.Gui.Controls
{
    /// <summary>
    /// Interaction logic for EditSingleHomeView.xaml
    /// </summary>
    public partial class EditSingleHomeView : UserControl
    {
        public EditSingleHomeView()
        {
            InitializeComponent();
        }

        private void RestrictText(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;

            var regex = new Regex(@"[^0-9,\-]");
            // ищем запрещенные символы
            var matches = regex.Matches(box.Text)
                .GetEnumerator()
                .ToList<Match>();

            // запомнили Индекс каретки
            var carretIndex = box.CaretIndex;
            var addToCarretIndex = 0;

            foreach (var match in matches)
                // Если вырезали до каретки, уменьшаем индекс
                if (match.Index <= carretIndex)
                    addToCarretIndex++;

            // заменили только на разрешенные символы
            var temp = regex.Replace(box.Text, "");
            // замена повторяющихся значений "-" на один такой знак
            temp = Regex.Replace(temp, @"(-+)", "-");
            box.Text = temp;

            box.CaretIndex = Math.Max(carretIndex - addToCarretIndex, 0);
        }
    }
}

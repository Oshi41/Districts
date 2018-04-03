using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Districts.Helper;

namespace Districts.Views
{
    /// <summary>
    ///     Логика взаимодействия для EditHomeInfo.xaml
    /// </summary>
    public partial class EditHomeInfo : Window
    {
        public EditHomeInfo()
        {
            InitializeComponent();
        }

        private void SuccessClose(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
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

            box.CaretIndex = carretIndex - addToCarretIndex;
        }
    }
}
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Districts.Views
{
    /// <summary>
    /// Логика взаимодействия для EditHomeInfo.xaml
    /// </summary>
    public partial class EditHomeInfo : Window
    {
        public EditHomeInfo()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        

        private void RestrictText(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;
            var s = Regex.Replace(box.Text, @"[^0-9,+\-]", "");
            box.Text = s;

        }

    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Districts.Views.Controls
{
    /// <summary>
    /// Interaction logic for SearchLineBox.xaml
    /// </summary>
    public partial class SearchLineBox : UserControl
    {
        public SearchLineBox()
        {
            InitializeComponent();
        }

        #region Dependency Property

        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            "SearchText", typeof(string), typeof(SearchLineBox), new PropertyMetadata(default(string)));

        public string SearchText
        {
            get { return (string) GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register(
            "SearchCommand", typeof(ICommand), typeof(SearchLineBox), new PropertyMetadata(default(ICommand)));

        public ICommand SearchCommand
        {
            get { return (ICommand) GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        public static readonly DependencyProperty CancelSearchCommandProperty = DependencyProperty.Register(
            "CancelSearchCommand", typeof(ICommand), typeof(SearchLineBox), new PropertyMetadata(default(ICommand)));

        public ICommand CancelSearchCommand
        {
            get { return (ICommand) GetValue(CancelSearchCommandProperty); }
            set { SetValue(CancelSearchCommandProperty, value); }
        }

        #endregion
    }
}

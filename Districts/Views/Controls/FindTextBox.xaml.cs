using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Districts.Views.Controls
{
    /// <summary>
    /// Interaction logic for FindTextBox.xaml
    /// </summary>
    public partial class FindTextBox : UserControl
    {
        public FindTextBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register(
            "SearchCommand", typeof(ICommand), typeof(FindTextBox), new PropertyMetadata(default(ICommand)));

        public ICommand SearchCommand
        {
            get { return (ICommand) GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        public static readonly DependencyProperty CancelSearchCommandProperty = DependencyProperty.Register(
            "CancelSearchCommand", typeof(ICommand), typeof(FindTextBox), new PropertyMetadata(default(ICommand)));

        public ICommand CancelSearchCommand
        {
            get { return (ICommand) GetValue(CancelSearchCommandProperty); }
            set { SetValue(CancelSearchCommandProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            "SearchText", typeof(string), typeof(FindTextBox), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SearchText
        {
            get { return (string) GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }
    }
}

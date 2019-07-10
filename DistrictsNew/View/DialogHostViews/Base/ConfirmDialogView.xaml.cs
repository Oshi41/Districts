using System.Windows;
using System.Windows.Controls;

namespace DistrictsNew.View.DialogHostViews
{
    /// <summary>
    /// Interaction logic for ConfirmDialogView.xaml
    /// </summary>
    public partial class ConfirmDialogView : UserControl
    {
        #region Props

        public static readonly DependencyProperty OkTextProperty = DependencyProperty.Register(
            "OkText", typeof(string), typeof(ConfirmDialogView), 
            new PropertyMetadata(Properties.Resources.OK));

        public string OkText
        {
            get { return (string) GetValue(OkTextProperty); }
            set { SetValue(OkTextProperty, value); }
        }

        public static readonly DependencyProperty CancelTextProperty = DependencyProperty.Register(
            "CancelText", typeof(string), typeof(ConfirmDialogView), 
            new PropertyMetadata(Properties.Resources.Cancel));

        public string CancelText
        {
            get { return (string) GetValue(CancelTextProperty); }
            set { SetValue(CancelTextProperty, value); }
        }

        #endregion

        public ConfirmDialogView()
        {
            InitializeComponent();
        }
    }
}

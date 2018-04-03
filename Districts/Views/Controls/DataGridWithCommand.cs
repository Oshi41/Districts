using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Districts.Views.Controls
{
    internal class DataGridWithCommand : DataGrid
    {
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register(
            "DoubleClickCommand", typeof(ICommand), typeof(DataGridWithCommand),
            new PropertyMetadata(default(ICommand)));

        public ICommand DoubleClickCommand
        {
            get => (ICommand) GetValue(DoubleClickCommandProperty);
            set => SetValue(DoubleClickCommandProperty, value);
        }

        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);

            e.Row.MouseDoubleClick += OnDoubleClick;
        }

        protected override void OnUnloadingRow(DataGridRowEventArgs e)
        {
            base.OnUnloadingRow(e);

            e.Row.MouseDoubleClick -= OnDoubleClick;
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row) DoubleClickCommand?.Execute(row.DataContext);
        }
    }
}
using System.ComponentModel;
using Districts.New.Gui;
using Districts.New.Interfaces;

namespace Districts.New.Implementation
{
    class DialogProvider : IDialogProvider
    {
        public bool ShowDialog(INotifyPropertyChanged vm, int height)
        {
            var window = new DialogWindow
            {
                DataContext = vm,
                MinHeight = height,
                MinWidth = height / 1.25
            };

            return window.ShowDialog() == true;
        }
    }
}

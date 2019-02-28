using System.ComponentModel;

namespace Districts.New.Interfaces
{
    interface IDialogProvider
    {
        bool ShowDialog(INotifyPropertyChanged vm, int height);
    }
}
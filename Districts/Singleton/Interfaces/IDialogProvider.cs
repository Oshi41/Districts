using System.Collections.Specialized;
using System.ComponentModel;

namespace Districts.Singleton
{
    interface IDialogProvider
    {
        bool ShowDialog(INotifyPropertyChanged vm, int height);
    }
}
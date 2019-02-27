using System.Collections.Specialized;

namespace Districts.Singleton
{
    interface IDialogProvider
    {
        bool ShowDialog(INotifyCollectionChanged vm, int height);
    }
}
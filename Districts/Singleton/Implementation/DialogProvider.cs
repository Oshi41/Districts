using System.Collections.Specialized;
using Districts.New.Gui;

namespace Districts.Singleton
{
    class DialogProvider : IDialogProvider
    {
        public bool ShowDialog(INotifyCollectionChanged vm, int height)
        {
            var window = new DialogWindow
            {
                Content = vm,
                MinHeight = height,
                MinWidth = height / 1.25
            };

            return window.ShowDialog() == true;
        }
    }
}

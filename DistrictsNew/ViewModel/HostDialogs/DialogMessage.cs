using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvvm;

namespace DistrictsNew.ViewModel.HostDialogs
{
    public class DialogMessage : BindableBase
    {
        public DialogMessage(string text, 
            bool isError = false,
            string okCaption = null,
            string cancelCaption = null)
        {
            Text = text;
            IsError = isError;
            OkCaption = okCaption;
            CancelCaption = cancelCaption ?? Properties.Resources.Cancel;
        }

        public bool IsError { get; }
        public string OkCaption { get; }
        public string CancelCaption { get; }

        public string Text { get; }
    }
}

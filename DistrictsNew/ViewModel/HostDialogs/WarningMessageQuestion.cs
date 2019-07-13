using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvvm;

namespace DistrictsNew.ViewModel.HostDialogs
{
    public class WarningMessageQuestion : BindableBase
    {
        public WarningMessageQuestion(string text, bool isError = false)
        {
            Text = text;
            IsError = isError;
        }

        public bool IsError { get; }

        public string Text { get; }
    }
}

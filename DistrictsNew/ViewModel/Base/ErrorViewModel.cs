using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvvm;

namespace DistrictsNew.ViewModel.Base
{
    public class ErrorViewModel : BindableBase, IDataErrorInfo
    {
        protected IDataErrorInfo Info { get; }

        public ErrorViewModel()
        {
            Info = new ErrorInfo.ErrorInfo(ValidateError, ValidateWarning);
        }

        public string this[string columnName] => Info[columnName];

        public string Error => Info.Error;

        protected virtual string ValidateError(string column)
        {
            return null;
        }

        protected virtual string ValidateWarning(string column)
        {
            return null;
        }
    }
}

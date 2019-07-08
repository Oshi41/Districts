using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvvm;

namespace DistrictsNew.ViewModel
{
    public class ExtendedBindable : BindableBase, IDataErrorInfo
    {
        private string _error;

        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string> _warnings = new ConcurrentDictionary<string, string>();

        protected virtual string ValidateError(string column)
        {
            return null;
        }

        protected virtual string ValidateWarning(string column)
        {
            return null;
        }

        #region Implementation of IDataErrorInfo

        public string this[string columnName]
        {
            get
            {
                var error = ValidateError(columnName);
                var warning = ValidateWarning(columnName);

                _errors.AddOrUpdate(columnName, x => error, (key, value) => error);
                _warnings.AddOrUpdate(columnName, x => warning, (key, value) => warning);

                return error ?? warning;
            }
        }

        public string Error
        {
            get
            {
                if (_error != null)
                    return _error;

                if (_errors.Any(x => x.Value != null))
                {
                    return _errors.First(x => x.Value != null).Value;
                }

                return null;
            }
        }

        #endregion
    }
}

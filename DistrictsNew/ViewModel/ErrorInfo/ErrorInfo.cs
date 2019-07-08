using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrictsNew.ViewModel.ErrorInfo
{
    class ErrorInfo : IDataErrorInfo
    {
        
        #region Fields

        private readonly Func<string, string> _errorValidation;
        private readonly Func<string, string> _warningValidation;
        private readonly ConcurrentDictionary<string, string> _errors = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string> _warnings = new ConcurrentDictionary<string, string>();
        private string _error;

        #endregion

        #region IDataErrorInfo

        public string this[string columnName]
        {
            get
            {
                var error = _errorValidation(columnName);
                var warning = _warningValidation(columnName);

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

        #region Methods

        public void SetError(string error = null)
        {
            _error = error;
        }

        #endregion

        public ErrorInfo(Func<string, string> errorValidation, Func<string, string> warningValidation)
        {
            _errorValidation = errorValidation;
            _warningValidation = warningValidation;

            if (errorValidation == null)
                throw new ArgumentException($"{nameof(errorValidation)} is null");

            if (warningValidation == null)
                throw new ArgumentException($"{nameof(warningValidation)} is null");
        }

    }
}

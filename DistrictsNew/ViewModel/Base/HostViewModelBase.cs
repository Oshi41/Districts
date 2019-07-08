using DistrictsLib.Interfaces;
using DistrictsNew.ViewModel.Base;
using Mvvm;
using Mvvm.Commands;

namespace DistrictsNew.ViewModel
{
    /// <summary>
    /// Базовая вид-модель для контента всплывающего диалога
    /// </summary>
    public abstract class HostViewModelBase : ChangesViewModel
    {

        ///// <summary>
        ///// Обработчик кнопок "ОК", "Отмена"
        ///// </summary>
        //public DelegateCommand<bool> SubmitCommand { get; }

        //protected HostViewModelBase()
        //{
        //    SubmitCommand = new DelegateCommand<bool>(OnSubmit, OnCanSubmit);
        //}

        //protected abstract bool OnCanSubmit(bool arg);

        //protected abstract void OnSubmit(bool obj);
        protected HostViewModelBase(IChangeNotifier changeNotifier) 
            : base(changeNotifier)
        {

        }
    }
}

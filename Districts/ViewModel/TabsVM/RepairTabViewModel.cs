using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.Helper;
using Districts.New.Implementation;
using Districts.Singleton;
using Mvvm;
using Mvvm.Commands;

namespace Districts.ViewModel.TabsVM
{
    class RepairTabViewModel : BindableBase
    {
        #region Fields

        private readonly Lazy<CardGenerator.CardGenerator> _generator =
    new Lazy<CardGenerator.CardGenerator>(() => new CardGenerator.CardGenerator());

        private readonly IMessageHelper _messageHelper = IoC.Instance.Get<IMessageHelper>();

        private bool _isGenerating;

        #endregion

        public ICommand SortCommand { get; private set; }
        public ICommand ShuffleCommand { get; private set; }

        public RepairTabViewModel()
        {
            SortCommand = new DelegateCommand(() => OnRepair(true), () => !_isGenerating);
            ShuffleCommand = new DelegateCommand(() => OnRepair(false), () => !_isGenerating);
        }

        private async void OnRepair(bool isSorting)
        {
            if (_isGenerating)
                return;

            try
            {
                _isGenerating = true;
                await Task.Run(() => _generator.Value.Repair(isSorting));
            }
            catch (Exception e)
            {
                Tracer.Tracer.WriteError(e);
            }
            finally
            {
                _isGenerating = false;
                _messageHelper.ShowDoneBubble();
            }
        }
    }
}

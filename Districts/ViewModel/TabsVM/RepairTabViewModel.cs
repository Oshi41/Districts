using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.Helper;
using Districts.MVVM;

namespace Districts.ViewModel.TabsVM
{
    class RepairTabViewModel : ObservableObject
    {
        #region Fields

        private readonly Lazy<CardGenerator.CardGenerator> _generator =
    new Lazy<CardGenerator.CardGenerator>(() => new CardGenerator.CardGenerator());

        private bool _isGenerating;

        #endregion

        public ICommand SortCommand { get; private set; }
        public ICommand ShuffleCommand { get; private set; }

        public RepairTabViewModel()
        {
            SortCommand = new Command(() => OnRepair(true), () => !_isGenerating);
            ShuffleCommand = new Command(() => OnRepair(false), () => !_isGenerating);
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
                Tracer.WriteError(e);
            }
            finally
            {
                _isGenerating = false;
                MessageHelper.ShowDoneBubble();
            }
        }
    }
}

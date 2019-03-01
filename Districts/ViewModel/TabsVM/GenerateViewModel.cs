using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.New.Implementation;
using Districts.Singleton;
using Mvvm;
using Mvvm.Commands;

namespace Districts.ViewModel.TabsVM
{
    internal class GenerateViewModel : BindableBase
    {
        private bool _bestDistribution;
        private bool _isGenerating;
        private bool _isPrinting;
        private bool _isSorted;
        private readonly IMessageHelper _messageHelper = IoC.Instance.Get<IMessageHelper>();

        public GenerateViewModel()
        {
            GenerateCommand = new DelegateCommand(OnGenerateCommand, () => !_isGenerating);
            PrintCommand = new DelegateCommand(OnPrintCommand, () => !_isPrinting);
        }        

        public ICommand GenerateCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public bool BestDistribution
        {
            get => _bestDistribution;
            set
            {
                if (value == _bestDistribution) return;
                _bestDistribution = value;
                OnPropertyChanged();
            }
        }
        
        public bool IsSorted
        {
            get { return _isSorted; }
            set
            {
                if (value == _isSorted) return;
                _isSorted = value;
                OnPropertyChanged();
            }
        }

        private void OnPrintCommand()
        {
            _isPrinting = true;

            try
            {
                var generator = new CardGenerator.CardGenerator();
                _messageHelper.ShowAwait();
                generator.PrintVisual();
            }
            catch (Exception e)
            {
                Tracer.Tracer.WriteError(e);
            }
            finally
            {
                _isPrinting = false;
                _messageHelper.ShowDone();
            }
        }

        private async void OnGenerateCommand()
        {
            _isGenerating = true;

            try
            {
                var generator = new CardGenerator.CardGenerator();
                await Task.Run(() => generator.Generate(BestDistribution, IsSorted));
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
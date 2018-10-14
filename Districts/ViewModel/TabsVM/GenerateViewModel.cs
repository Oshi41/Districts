using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.Helper;
using Districts.MVVM;

namespace Districts.ViewModel.TabsVM
{
    internal class GenerateViewModel : ObservableObject
    {
        private bool _bestDistribution;
        private bool _isGenerating;
        private bool _isPrinting;
        private bool _isSorted;

        public GenerateViewModel()
        {
            GenerateCommand = new Command(OnGenerateCommand, () => !_isGenerating);
            PrintCommand = new Command(OnPrintCommand, () => !_isPrinting);
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
                MessageHelper.ShowAwait();
                generator.PrintVisual();
            }
            catch (Exception e)
            {
                Tracer.WriteError(e);
            }
            finally
            {
                _isPrinting = false;
                MessageHelper.ShowDone();
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
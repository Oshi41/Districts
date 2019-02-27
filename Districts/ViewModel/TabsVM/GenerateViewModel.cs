using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Districts.Helper;
using Microsoft.Expression.Interactivity.Core;
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
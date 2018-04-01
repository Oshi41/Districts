using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Districts.Helper;
using Districts.MVVM;
using Districts.Settings;
using MessageBox = System.Windows.MessageBox;

namespace Districts.ViewModel.TabsVM
{
    class GenerateViewModel : ObservableObject
    {
        private bool _isGenerating;
        private bool _isPrinting;
        private bool _bestDistribution;

        public ICommand GenerateCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public bool BestDistribution
        {
            get { return _bestDistribution; }
            set
            {
                if (value == _bestDistribution) return;
                _bestDistribution = value;
                OnPropertyChanged();
            }
        }

        public GenerateViewModel()
        {
            GenerateCommand = new Command(OnGenerateCommand, () => !_isGenerating);
            PrintCommand = new Command(OnPrintCommand, () => !_isPrinting);
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
                await Task.Run(() => generator.GenerateNew(BestDistribution));
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

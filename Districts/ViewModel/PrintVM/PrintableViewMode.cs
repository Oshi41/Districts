using System.Linq;
using Districts.JsonClasses;
using Mvvm;

namespace Districts.ViewModel.PrintVM
{
    public class PrintableViewMode : BindableBase
    {
        private OneCardSizeViewModel _firstBack;
        private OneCardSizeViewModel _firstFront;
        private OneCardSizeViewModel _secondBack;
        private OneCardSizeViewModel _secondFront;


        public PrintableViewMode(Card first, Card second)
        {
            FillModelsF(first);
            FillModelsS(second);
            //FillModels(first, ref _firstFront, ref _firstBack);
            //FillModels(second, ref _secondFront, ref _secondBack);

            //OnPropertyChanged(nameof(FirstBack));
            //OnPropertyChanged(nameof(FirstFront));
            //OnPropertyChanged(nameof(SecondBack));
            //OnPropertyChanged(nameof(SecondFront));
        }

        public OneCardSizeViewModel FirstFront
        {
            get => _firstFront;
            set
            {
                if (Equals(value, _firstFront)) return;
                _firstFront = value;
                OnPropertyChanged();
            }
        }

        public OneCardSizeViewModel FirstBack
        {
            get => _firstBack;
            set
            {
                if (Equals(value, _firstBack)) return;
                _firstBack = value;
                OnPropertyChanged();
            }
        }

        public OneCardSizeViewModel SecondFront
        {
            get => _secondFront;
            set
            {
                if (Equals(value, _secondFront)) return;
                _secondFront = value;
                OnPropertyChanged();
            }
        }

        public OneCardSizeViewModel SecondBack
        {
            get => _secondBack;
            set
            {
                if (Equals(value, _secondBack)) return;
                _secondBack = value;
                OnPropertyChanged();
            }
        }

        private void FillModelsF(Card card)
        {
            var number = card.Number;
            var halfIndex = card.Doors.Count / 2;
            var firstHalf = card.Doors.GetRange(0, halfIndex);
            var secondHalf = card.Doors.Except(firstHalf).ToList();

            FirstFront = new OneCardSizeViewModel(number, firstHalf);
            FirstBack = new OneCardSizeViewModel(number, secondHalf);
        }

        private void FillModelsS(Card card)
        {
            var number = card.Number;
            var halfIndex = card.Doors.Count / 2;
            var firstHalf = card.Doors.GetRange(0, halfIndex);
            var secondHalf = card.Doors.Except(firstHalf).ToList();

            SecondFront = new OneCardSizeViewModel(number, firstHalf);
            SecondBack = new OneCardSizeViewModel(number, secondHalf);
        }

        private void FillModels(Card card,
            ref OneCardSizeViewModel first,
            ref OneCardSizeViewModel second)
        {
            var number = card.Number;
            var halfIndex = card.Doors.Count / 2;
            var firstHalf = card.Doors.GetRange(0, halfIndex);
            var secondHalf = card.Doors.Except(firstHalf).ToList();

            first = new OneCardSizeViewModel(number, firstHalf);
            second = new OneCardSizeViewModel(number, secondHalf);
        }
    }
}
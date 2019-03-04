using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Districts.Helper;
using Districts.JsonClasses;
using Districts.MVVM;

namespace Districts.ViewModel
{
    class CopyCardViewModel : ObservableObject
    {
        private Card _selected;
        private bool _isLoaded;
        private ObservableCollection<Card> _cards;

        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set => Set(ref _cards, value);
        }

        public Card Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        public bool IsLoaded
        {
            get => _isLoaded;
            set
            {
                if (Set(ref _isLoaded, value))
                {
                    if (IsLoaded)
                    {
                        Cards = new ObservableCollection<Card>(LoadingWork.LoadCards().Select(x => x.Value).ToList());
                    }
                    else
                    {
                        Cards.Clear();
                        Selected = null;
                    }
                }
            }
        }
    }
}

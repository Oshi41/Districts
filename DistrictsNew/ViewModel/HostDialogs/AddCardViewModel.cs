using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistrictsLib.Interfaces;
using DistrictsNew.ViewModel.Base;

namespace DistrictsNew.ViewModel.HostDialogs
{
    class AddCardViewModel : ChangesViewModel
    {
        private readonly IList<string> _existingCards;
        private string _cardName;

        public string CardName
        {
            get => _cardName;
            set => SetProperty(ref _cardName, value);
        }

        public AddCardViewModel(IChangeNotifier changeNotifier, IList<string> existingCards)
            : base(changeNotifier)
        {
            _existingCards = existingCards;
        }

        protected override string ValidateError(string column)
        {
            if (string.Equals(column, nameof(CardName))
                && !string.IsNullOrWhiteSpace(CardName)
                && _existingCards.Contains(CardName))
            {
                return Properties.Resources.AddCard_AlreadyExisting;
            }

            return base.ValidateError(column);
        }
    }
}

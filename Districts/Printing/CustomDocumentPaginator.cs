using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Districts.JsonClasses;
using Districts.ViewModel.PrintVM;
using Districts.Views;

namespace Districts.Printing
{
    class CustomDocumentPaginator : DocumentPaginator
    {
        private readonly List<Card> _cards;
        private readonly PrintCapabilities _printCapabilities;
        private Size _pageSize;
        private int _count;

        public CustomDocumentPaginator(List<Card> cards, PrintDialog dlg)
        {
            _cards = cards;
            _count = (int) Math.Ceiling((decimal)(_cards.Count / 2));
            _printCapabilities = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);
            _pageSize = new Size((double) _printCapabilities.OrientedPageMediaWidth, (double) _printCapabilities.OrientedPageMediaHeight);
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            var first = _cards[pageNumber * 2];
            var second = pageNumber * 2 + 1 >= _cards.Count
                ? null
                : _cards[pageNumber * 2 + 1];

            var vm = new PrintableViewMode(first, second);
            var control = new PrintableCard {DataContext = vm};

            // Force render
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            control.Arrange(new Rect(_pageSize));
            control.UpdateLayout();

            // ожтдаю отрисовки
            Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);

            var result = new DocumentPage(control);

            return result;
        }


        public override bool IsPageCountValid => PageCount <= Math.Ceiling((decimal)(_cards.Count / 2));
        public override int PageCount => _count;

        public override Size PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public override IDocumentPaginatorSource Source => null;
    }
}

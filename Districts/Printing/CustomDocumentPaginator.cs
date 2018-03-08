using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Districts.JsonClasses;
using Districts.ViewModel.PrintVM;
using Districts.Views;

namespace Districts.Printing
{
    /// <summary>
    /// Печать кучи карточек в один многостраничный файл
    /// </summary>
    class CustomDocumentPaginator : DocumentPaginator
    {
        private readonly List<Card> _cards;
        private readonly PrintCapabilities _printCapabilities;
        private Size _pageSize;
        private int _count;

        /// <summary>
        /// Создаю документ на основе карточек и настроек принтера
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="dlg"></param>
        public CustomDocumentPaginator(List<Card> cards, PrintDialog dlg)
        {
            _cards = cards;
            _count = (int)Math.Ceiling((decimal)(_cards.Count / 2));
            _printCapabilities = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);

            SetSize(_printCapabilities);
        }

        private void SetSize(PrintCapabilities capabilities)
        {
            var settings = new PrinterSettings();
            var a4 = settings.PaperSizes.OfType<PaperSize>().FirstOrDefault(x => x.Kind == PaperKind.A4);
            _pageSize = new Size(a4.Width, a4.Height);


            //var printerSize = new Size(0, 0);
            ////// устанавливаю размер по принтеру
            ////if (capabilities?.OrientedPageMediaWidth != null 
            ////    && capabilities.OrientedPageMediaHeight.HasValue)
            ////{
            ////    printerSize = new Size((double)capabilities.OrientedPageMediaWidth,
            ////                    (double)capabilities.OrientedPageMediaHeight);
            ////}

            //
            ////if (a4 != null)
            ////{
            ////    if (printerSize.Height >= a4.Height
            ////        || printerSize.Width >= a4.Width)
            ////    {
            //printerSize 
            ////    }
            ////}

            //_pageSize = printerSize;
        }


        public override DocumentPage GetPage(int pageNumber)
        {
            var first = _cards[pageNumber * 2];
            var second = pageNumber * 2 + 1 >= _cards.Count
                ? null
                : _cards[pageNumber * 2 + 1];

            var vm = new PrintableViewMode(first, second);
            var control = new PrintableCard { DataContext = vm };

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

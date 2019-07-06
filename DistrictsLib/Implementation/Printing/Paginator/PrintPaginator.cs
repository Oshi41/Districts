using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DistrictsLib.Implementation.Printing.WPF;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsLib.Implementation.Printing.Paginator
{
    class PrintPaginator : DocumentPaginator
    {
        private readonly IList<Card> _cards;

        #region Fields

        private bool _isPageCountValid;
        private Size _pageSize;

        private readonly PrintDialog _dlg;
        private readonly PrintCapabilities _printCapabilities;

        #endregion

        public PrintPaginator(IList<Card> cards, PrintDialog dlg)
        {
            Source = null;

            _cards = cards;
            _dlg = dlg;
            _printCapabilities = _dlg.PrintQueue.GetPrintCapabilities(_dlg.PrintTicket);

            SetA4Size();
        }

        #region Overrides of DocumentPaginator

        public override DocumentPage GetPage(int pageNumber)
        {
            var cards = _cards.Skip(pageNumber - 1).Take(4).ToList();

            var view = new PaperSide
            {
                DataContext = cards
            };

            AwaitRender(view);

            return new DocumentPage(view);
        }

        // У нас по 4 карточки на страницу
        public override bool IsPageCountValid => PageCount <= Math.Ceiling((double)_cards.Count / 4);

        public override int PageCount { get; }

        public override Size PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }

        public override IDocumentPaginatorSource Source { get; }

        #endregion

        private void SetA4Size()
        {
            var settings = new PrinterSettings();
            var a4 = settings.PaperSizes
                .OfType<PaperSize>()
                .FirstOrDefault(x => x.Kind == PaperKind.A4);

            _pageSize = new Size(a4.Width, a4.Height);
        }

        private void AwaitRender(UIElement control)
        {

        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Districts.JsonClasses;
using Districts.ViewModel.PrintVM;
using Districts.Views;

namespace Districts.Printing
{
    class FastCardPrinter : DocumentPaginator
    {
        private readonly List<Card> _cards;
        private readonly PrintCapabilities _printCapabilities;
        private Size _pageSize;
        private int _count;

        private List<Task<PrintableCard>> _printedCards = new List<Task<PrintableCard>>();

        /// <summary>
        /// Отличается тем, что сразу ставит прогружаться кучу контролов.
        /// Ест больше, зато быстрее
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="dlg"></param>
        public FastCardPrinter(List<Card> cards, PrintDialog dlg)
        {
            _cards = cards;
            _count = (int)Math.Ceiling((decimal)(_cards.Count / 2));
            _printCapabilities = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);
            _pageSize = new Size((double)_printCapabilities.OrientedPageMediaWidth, (double)_printCapabilities.OrientedPageMediaHeight);

            for (int i = 0; i < PageCount; i++)
            {
                PrintableCard Func()
                {
                    var first = _cards[i * 2];
                    var second = i * 2 + 1 >= _cards.Count
                        ? null
                        : _cards[i * 2 + 1];

                    var vm = new PrintableViewMode(first, second);
                    var control = new PrintableCard {DataContext = vm};

                    // Force render
                    control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    control.Arrange(new Rect(_pageSize));
                    control.UpdateLayout();

                    // ожтдаю отрисовки
                    Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
                    return control;
                }

                var task = new Task<PrintableCard>(Func);
                _printedCards.Add(task);
                task.Start();
            }
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            var task = _printedCards[pageNumber];
            if (task.Status == TaskStatus.Running)
                task.Wait();

            var result = new DocumentPage(task.Result);
            return result;
        }


        public override bool IsPageCountValid => PageCount <= Math.Ceiling((decimal)(_cards.Count / 2));
        public override int PageCount => _printedCards.Count;

        public override Size PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public override IDocumentPaginatorSource Source => null;
    }
}

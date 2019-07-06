using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using DistrictsLib.Implementation.Printing.Paginator;
using DistrictsLib.Interfaces;
using DistrictsLib.Legacy.JsonClasses;
using PrintDialog = System.Windows.Controls.PrintDialog;

namespace DistrictsLib.Implementation.Printing
{
    class Printable : IPrintable
    {
        #region Implementation of IPrintable

        public void Print(IList<Card> cards)
        {
            var dlg = new PrintDialog();
            var paginator = new PrintPaginator(cards, dlg);

            if (ShowPreview(dlg, paginator))
            {
                dlg.PrintDocument(paginator, "Карточки участков");
            }
        }

        private bool ShowPreview(PrintDialog dlg, DocumentPaginator paginator)
        {
            var temp = string.Empty;

            try
            {
                temp = Path.GetTempPath();

                //GetTempFileName creates a file, the XpsDocument throws an exception if the file already
                //exists, so delete it. Possible race condition if someone else calls GetTempFileName
                File.Delete(temp);

                using (var xpsDocument = new XpsDocument(temp, FileAccess.ReadWrite))
                {
                    var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                    writer.Write(paginator);

                    DocumentViewer previewWindow = new DocumentViewer
                    {
                        Document = xpsDocument.GetFixedDocumentSequence()
                    };

                    var printpriview = new Window
                    {
                        Content = previewWindow,
                        Title = "Print Preview"
                    };

                    printpriview.ShowDialog();

                    return MessageBox.Show("Продолжить?", 
                               "Подтверждение", 
                               MessageBoxButton.YesNo,
                               MessageBoxImage.Question) == MessageBoxResult.Yes;
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
            finally
            {
                if (File.Exists(temp))
                    File.Delete(temp);
            }

            return true;
        }

        #endregion
    }
}

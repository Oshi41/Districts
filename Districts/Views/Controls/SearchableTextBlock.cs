using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Districts.Views.Controls
{
    class SearchableTextBlock : TextBlock
    {

        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            "SearchText", typeof(string), typeof(SearchableTextBlock), 
            new FrameworkPropertyMetadata(null, AfterSearch));

        private static void AfterSearch(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SearchableTextBlock block)
            {
                //  негде или нечего искать 
                if (string.IsNullOrWhiteSpace(block.Text) || string.IsNullOrEmpty(block.SearchText))
                {
                    block.ClearText();
                    return;
                }

                var index = block.Text.IndexOf(block.SearchText, StringComparison.InvariantCultureIgnoreCase);
                if (index < 0)
                {
                    block.ClearText();
                    return;
                }

                // индекс последнего слова
                var endIndex = index + block.SearchText.Length;

                // что до строки поиска
                var first = block.Text.Substring(0, index);
                // найденное и выделяемое
                var second = block.Text.Substring(index, block.SearchText.Length);
                // что стоит после
                var third = block.Text.Substring(endIndex, block.Text.Length - endIndex);

                // особый Run для выделяемого
                var run = new Run(second)
                {
                    Background = block.SelectedBrush,
                    FontWeight = FontWeights.Bold
                };

                // очищаю предыдущее
                block.Inlines.Clear();

                // вставляю текст
                block.Inlines.Add(new Run(first));
                block.Inlines.Add(run);
                block.Inlines.Add(new Run(third));
            }
        }

        private void ClearText()
        {
            // Cбрасываем поиск
            var text = Text;
            Inlines.Clear();
            Text = text;
        }

        public string SearchText
        {
            get { return (string) GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }


        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(SearchableTextBlock), 
            new PropertyMetadata(Brushes.Yellow));

        public Brush SelectedBrush
        {
            get { return (Brush) GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Districts.Views.Controls
{
    internal class NumericInput : TextBox
    {
        public NumericInput()
        {
            PreviewTextInput += OnPreviewTextInput;
            AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(OnPaste));
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string) e.DataObject.GetData(typeof(string));
                SetText(text);
                e.Handled = true;
            }
            else
            {
                e.CancelCommand();
            }
        }


        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SetText(e.Text);
        }

        private void SetText(string input)
        {
            var selectionStart = SelectionStart;
            var selectionLength = SelectionLength;

            var newText = string.Empty;
            foreach (var c in input)
                if (char.IsDigit(c) || char.IsControl(c))
                    newText += c;

            Text = newText;

            SelectionStart = selectionStart <= Text.Length
                ? selectionStart
                : Text.Length;

            SelectionLength = selectionLength;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Districts.Views.Controls
{
    class NumericInput : TextBox
    {
        public NumericInput()
        {
            PreviewTextInput += OnPreviewTextInput;
            this.AddHandler(DataObject.PastingEvent, new DataObjectPastingEventHandler(OnPaste));
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
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
            Int32 selectionStart = this.SelectionStart;
            Int32 selectionLength = this.SelectionLength;

            String newText = String.Empty;
            foreach (Char c in input)
            {
                if (Char.IsDigit(c) || Char.IsControl(c))
                    newText += c;
            }

            Text = newText;

            SelectionStart = selectionStart <= Text.Length
                ? selectionStart
                : Text.Length;

            SelectionLength = selectionLength;
        }
    }
}

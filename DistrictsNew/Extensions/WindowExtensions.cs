﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DistrictsNew.View.Dialogs;
using DistrictsNew.ViewModel.Base;

namespace DistrictsNew.Extensions
{
    public static class WindowExtensions
    {
        public static bool? AskUser(string question, string title)
        {
            var result = MessageBox.Show(question,
                title,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
                return null;

            return result == MessageBoxResult.Yes;
        }

        public static bool? ShowDialog(this ChangesViewModel vm,
            string title,
            double? width = null,
            double? height = null)
        {
            var window = new DialogWindowBase
            {
                DataContext = vm,
                Title = title,
            };

            if (height == null && width == null)
            {
                width = 400;
            }

            if (height == null)
            {
                height = width * 1.25;
            }

            if (width == null)
            {
                width = height / 1.25;
            }

            window.Width = width.Value;
            window.Height = height.Value;

            return window.ShowDialog();
        }
    }
}

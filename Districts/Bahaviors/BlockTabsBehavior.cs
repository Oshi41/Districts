﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using Districts.Helper;

namespace Districts.Bahaviors
{
    /// <summary>
    /// Блокирует остальные табы
    /// </summary>
    class BlockTabsBehavior : Behavior<ButtonBase>
    {
        private bool _isBlocked;

        #region Overrided

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Click += BlockOrReleaseTab;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Click -= BlockOrReleaseTab;
        }

        #endregion

        private void BlockOrReleaseTab(object sender, RoutedEventArgs e)
        {
            var tab = WpfHelper.GetFirstParentByType<TabControl>(AssociatedObject);
            if (tab == null || tab.Items.Count <= 0)
                return;

            if (_isBlocked)
            {
                foreach (var item in tab.Items.OfType<TabItem>())
                    item.IsEnabled = true;
            }
            else
            {
                var selected = tab.SelectedItem as TabControl;
                foreach (var item in tab.Items.OfType<TabItem>())
                    item.IsEnabled = false;

                if (selected != null)
                    selected.IsEnabled = true;
            }

            _isBlocked = !_isBlocked;
        }
    }
}

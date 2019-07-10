using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DistrictsNew.ResourceDictionaries.Styles
{
    public partial class Window
    {
        private void ExecuteForParentWindow(FrameworkElement element, Action<System.Windows.Window> action)
        {
            if (element != null
                && action != null
                && element.TemplatedParent is FrameworkElement templateParent
                && System.Windows.Window.GetWindow(templateParent) is System.Windows.Window window)
            {
                action(window);
            }
        }

        private void IconMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                ExecuteForParentWindow(element, window =>
                {
                    var point = element.PointToScreen(new Point(element.ActualWidth / 2, element.ActualHeight));
                    SystemCommands.ShowSystemMenu(window, point);
                });
            }
        }

        private void IconMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
                ExecuteForParentWindow(sender as FrameworkElement, SystemCommands.CloseWindow);
        }

        private void MinButtonClick(object sender, RoutedEventArgs e)
        {
            ExecuteForParentWindow(sender as FrameworkElement, SystemCommands.MinimizeWindow);
        }

        private void MaxButtonClick(object sender, RoutedEventArgs e)
        {
            ExecuteForParentWindow(sender as FrameworkElement, window =>
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    SystemCommands.RestoreWindow(window);
                }
                else
                {
                    SystemCommands.MaximizeWindow(window);
                }
            });
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            ExecuteForParentWindow(sender as FrameworkElement, SystemCommands.CloseWindow);
        }
    }
}

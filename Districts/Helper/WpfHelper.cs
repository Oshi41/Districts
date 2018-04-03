using System.Windows;
using System.Windows.Media;

namespace Districts.Helper
{
    public static class WpfHelper
    {
        public static T GetFirstParentByType<T>(DependencyObject child)
            where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is T result)
                    return result;
                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}
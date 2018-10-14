using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Districts.Helper
{
    public static class WpfHelper
    {
        public static T GetFirstParentByType<T>(DependencyObject child)
            where T : DependencyObject
        {
            if (child == null)
                return null;

            var parent = child is Visual || child is Visual3D
                ? VisualTreeHelper.GetParent(child)
                : LogicalTreeHelper.GetParent(child);

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
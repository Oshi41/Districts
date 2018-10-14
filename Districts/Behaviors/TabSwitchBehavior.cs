using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using Districts.Helper;

namespace Districts.Behaviors
{
    class TabSwitchBehavior : Behavior<Hyperlink>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Click += OnClick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Click -= OnClick;
        }

        public static readonly DependencyProperty TabIndexProperty = DependencyProperty.Register(
            "TabIndex", typeof(int), typeof(TabSwitchBehavior),
            new PropertyMetadata(-1, OnIndexChanged));

        private static void OnIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabSwitchBehavior behavior && behavior.AssociatedObject != null)
            {
                behavior.OnClick(null, null);
            }
        }

        public int TabIndex
        {
            get { return (int) GetValue(TabIndexProperty); }
            set { SetValue(TabIndexProperty, value); }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (TabIndex < 0)
                return;

            var tab = WpfHelper.GetFirstParentByType<TabControl>(AssociatedObject);
            if (tab == null || tab.SelectedIndex == TabIndex)
                return;

            tab.SelectedIndex = TabIndex;
        }
    }
}

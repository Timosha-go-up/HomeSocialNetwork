using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HomeSocialNetwork
{
    public static class ButtonHelpers  // ← public static!
    {
        // Присоединённое свойство — public
        public static HorizontalAlignment GetContentAlignment(DependencyObject obj)
            => (HorizontalAlignment)obj.GetValue(ContentAlignmentProperty);

        public static void SetContentAlignment(DependencyObject obj, HorizontalAlignment value)
            => obj.SetValue(ContentAlignmentProperty, value);

        public static readonly DependencyProperty ContentAlignmentProperty =
            DependencyProperty.RegisterAttached(
                "ContentAlignment",
                typeof(HorizontalAlignment),
                typeof(ButtonHelpers),
                new PropertyMetadata(HorizontalAlignment.Center)
            );
    }
}

using System.Windows;

namespace Autobus.ViewModel
{
    public class MovableControl
    {
        public static readonly DependencyProperty DraggedDataProperty = DependencyProperty.RegisterAttached("DraggedData", typeof(object), typeof(MovableControl), new PropertyMetadata(null));

        public static readonly DependencyProperty PlacedDataProperty = DependencyProperty.RegisterAttached("PlacedData", typeof(object), typeof(MovableControl), new PropertyMetadata(null));

        public static object GetDraggedData(DependencyObject obj)
        {
            return obj.GetValue(DraggedDataProperty);
        }

        public static object GetPlacedData(DependencyObject obj)
        {
            return obj.GetValue(PlacedDataProperty);
        }

        public static void SetDraggedData(DependencyObject obj, object value)
        {
            obj.SetValue(DraggedDataProperty, value);
        }

        public static void SetPlacedData(DependencyObject obj, object value)
        {
            obj.SetValue(PlacedDataProperty, value);
        }
    }
}
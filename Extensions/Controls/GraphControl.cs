using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Extensions
{
    public partial class GraphControl : Control, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register("Series", typeof(ObservableCollection<Chart>), typeof(GraphControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        static GraphControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphControl), new FrameworkPropertyMetadata(typeof(GraphControl)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Chart> Series
        {
            get => (ObservableCollection<Chart>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        public Visibility SeriesListVisibility
        {
            get => seriesListVisibility;

            set
            {
                if (seriesListVisibility != value)
                {
                    seriesListVisibility = value;
                    OnPropertyChanged(nameof(SeriesListVisibility));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()) && Series?.Any() == true)
            {
                DrawGraph(drawingContext, Series);
            }
        }

        private Visibility seriesListVisibility;

        private DrawingContext DrawGraph(DrawingContext drawingContext, ObservableCollection<Chart> Series)
        {
            double max = Series.Max(z => z.ChartValue);
            Pen pen = null;
            DrawingGroup dg = null;

            for (int i = 1; i <= Series.Count; i++)
            {
                Chart item = Series[i - 1];
                pen = new Pen(item.ChartBrush, ActualWidth / Series.Count);
                pen.Freeze();

                dg = new();
                using (DrawingContext graph = dg.Open())
                {
                    Point point0 = new((pen.Thickness * i) - (pen.Thickness / 2), ActualHeight);
                    Point point1 = new((pen.Thickness * i) - (pen.Thickness / 2), ActualHeight - (item.ChartValue / max * ActualHeight));
                    graph.DrawLine(pen, point0, point1);
                    drawingContext.DrawDrawing(dg);
                }
                dg.Freeze();
            }
            return drawingContext;
        }
    }
}

using Contract;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rectangle2D
{
    public class Rectangle2D : IShape
    {
        public Point2D _leftTop = new Point2D();
        public Point2D _rightBottom = new Point2D();
        public string Name => "Rectangle";

        public string Icon => "/Images/rectangle.png";

        public SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush(Colors.Black);
        public int Thickness { get; set; } = 1;
        public DoubleCollection Dash { get; set; } = new DoubleCollection() { 1 };

        public IShape Clone()
        {
            return new Rectangle2D();
        }

        public UIElement Draw()
        {
            var left = Math.Min(_rightBottom.X, _leftTop.X);
            var top = Math.Min(_rightBottom.Y, _leftTop.Y);

            var right = Math.Max(_rightBottom.X, _leftTop.X);
            var bottom = Math.Max(_rightBottom.Y, _leftTop.Y);

            var width = right - left;
            var height = bottom - top;

            var rect = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = ColorBrush,
                StrokeThickness = Thickness,
                StrokeDashArray = Dash,
            };

            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);

            return rect;
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D()
            {
                X = x,
                Y = y
            };
        }

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D()
            {
                X = x,
                Y = y
            };
        }
    }

}

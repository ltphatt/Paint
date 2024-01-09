
using Contract;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rectangle2D
{
    public class Rectangle2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        public string Name => "Rectangle";

        public string Icon => "/Images/rectangle.png";

        public IShape Clone()
        {
            return new Rectangle2D();
        }

        public UIElement Draw(SolidColorBrush brush, int thickness)
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
                Stroke = brush,
                StrokeThickness = thickness,
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

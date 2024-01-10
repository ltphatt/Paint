
using Contract;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ellipse2D
{
    public class Ellipse2D : IShape
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        public string Name => "Ellipse";

        public string Icon => "/Images/ellipse.png";

        public SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush(Colors.Black);
        public int Thickness { get; set; } = 1;
        public DoubleCollection Dash { get; set; } = new DoubleCollection() { 1 };

        public IShape Clone()
        {
            return new Ellipse2D();
        }

        public UIElement Draw()
        {
            var left = Math.Min(_rightBottom.X, _leftTop.X);
            var top = Math.Min(_rightBottom.Y, _leftTop.Y);

            var right = Math.Max(_rightBottom.X, _leftTop.X);
            var bottom = Math.Max(_rightBottom.Y, _leftTop.Y);

            var width = right - left;
            var height = bottom - top;

            var ellipse = new Ellipse()
            {
                Width = width,
                Height = height,
                Stroke = ColorBrush,
                StrokeThickness = Thickness,
                StrokeDashArray = Dash,
            };

            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);

            return ellipse;
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

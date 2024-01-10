
using Contract;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Triangle2D
{
    public class Triangle2D : IShape
    {
        public Point2D _leftTop = new Point2D();
        public Point2D _rightBottom = new Point2D();
        public string Name => "Triangle";

        public string Icon => "/Images/right-triangle.png";

        public SolidColorBrush ColorBrush {get;set;} = new SolidColorBrush(Colors.Black);
        public int Thickness { get; set; } = 1;
        public DoubleCollection Dash { get; set; } = new DoubleCollection() { 1 };

        public IShape Clone()
        {
            return new Triangle2D();
        }

        public UIElement Draw()
        {
            var left = Math.Min(_leftTop.X ,_rightBottom.X);
            var top = Math.Min(_leftTop.Y , _rightBottom.Y);

            var polygon = new Polygon()
            {
                Stroke = ColorBrush,
                StrokeThickness = Thickness,
                StrokeDashArray = Dash,
                Fill = Brushes.Transparent,
            };

            polygon.Points.Add(new Point(_leftTop.X, _leftTop.Y));
            polygon.Points.Add(new Point(_leftTop.X, _rightBottom.Y));
            polygon.Points.Add(new Point(_rightBottom.X, _rightBottom.Y));

            Canvas.SetLeft(polygon, left);
            Canvas.SetTop(polygon, top);

            return polygon;
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

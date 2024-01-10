using Contract;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Square2D
{
    public class Square2D : IShape
    {
        public Point2D _leftTop = new Point2D();
        public Point2D _rightBottom = new Point2D();

        public string Name => "Square";

        public string Icon => "/Images/square.png";

        public SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush(Colors.Black);
        public int Thickness { get; set; } = 1;
        public DoubleCollection Dash { get; set; } = new DoubleCollection() { 1 };

        public IShape Clone()
        {
            return new Square2D();
        }

        public UIElement Draw()
        {
            var left = Math.Min(_rightBottom.X, _leftTop.X);
            var top = Math.Min(_rightBottom.Y, _leftTop.Y);

            var right = Math.Max(_rightBottom.X, _leftTop.X);

            var width = right - left;

            var square = new Rectangle()
            {
                Width = width,
                Height = width,
                Stroke = ColorBrush,
                StrokeThickness = Thickness,
                StrokeDashArray = Dash,
            };

            if (_rightBottom.X > _leftTop.X && _rightBottom.Y > _leftTop.Y)
            {
                Canvas.SetLeft(square, _leftTop.X);
                Canvas.SetTop(square, _leftTop.Y);
            }
            else if (_rightBottom.X < _leftTop.X && _rightBottom.Y > _leftTop.Y)
            {
                Canvas.SetLeft(square, _rightBottom.X);
                Canvas.SetTop(square, _leftTop.Y);
            }
            else if (_rightBottom.X > _leftTop.X && _rightBottom.Y < _leftTop.Y)
            {
                Canvas.SetLeft(square, _leftTop.X);
                Canvas.SetTop(square, _rightBottom.Y);
            }
            else
            {
                Canvas.SetLeft(square, _rightBottom.X);
                Canvas.SetTop(square, _rightBottom.Y);
            }

            return square;
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom.X = x;
            _rightBottom.Y = y;

            var width = Math.Abs(_rightBottom.X - _leftTop.X);
            var height = Math.Abs(_rightBottom.Y - _leftTop.Y);

            if (width < height)
            {
                if (_rightBottom.Y < _leftTop.Y)
                    _rightBottom.Y = _leftTop.Y - width;
                else
                    _rightBottom.Y = _leftTop.Y + width;
            }
            else if (width > height)
            {
                if (_rightBottom.X < _leftTop.X)
                    _rightBottom.X = _leftTop.X - height;
                else _rightBottom.X = _leftTop.X + height;
            }
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

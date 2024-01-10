using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Contract;
using Newtonsoft.Json;

namespace Line2D
{
    public class Line2D : IShape
    {
        public Point2D Start = new Point2D();
        public Point2D End = new Point2D();

        public string Name => "Line";
        public string Icon => "Images/line.png";

        public SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush(Colors.Black);
        public int Thickness { get; set; } = 1;
        public DoubleCollection Dash { get; set; } = new DoubleCollection() { 1 };

        public IShape Clone()
        {
            return new Line2D();
        }
        public UIElement Draw()
        {
            Line l = new Line()
            {
                X1 = Start.X,
                Y1 = Start.Y,
                X2 = End.X,
                Y2 = End.Y,
                StrokeThickness = Thickness,
                Stroke = ColorBrush,
                StrokeDashArray = Dash
            };
            return l;
        }

        public void HandleEnd(double x, double y)
        {
            End = new Point2D()
            {
                X = x,
                Y = y
            };
        }

        public void HandleStart(double x, double y)
        {
            Start = new Point2D()
            {
                X = x,
                Y = y
            };
        }
    }

}

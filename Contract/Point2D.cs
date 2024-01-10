using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Newtonsoft.Json;

namespace Contract
{
    public class Point2D : IShape
    {
        public double X { get; set; }
        public double Y { get; set; }

        public string Name => "Point2D";
        public string Icon { get; }
        public SolidColorBrush ColorBrush { get; set; }
        public int Thickness { get; set; }
        public DoubleCollection Dash { get; set; }

        public IShape Clone()
        {
            return new Point2D();
        }

        public UIElement Draw(int thickness)
        {
            Line l = new Line()
            {
                X1 = X,
                Y1 = Y,
                X2 = X,
                Y2 = Y,
                StrokeThickness = thickness,
                Stroke = new SolidColorBrush(Colors.Red),
            };
            return l;
        }

        public UIElement Draw()
        {
            throw new NotImplementedException();
        }

        public void HandleEnd(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void HandleStart(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}

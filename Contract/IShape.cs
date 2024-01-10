using System;
using System.Windows;
using System.Windows.Media;

namespace Contract
{
    public interface IShape
    {
        string Name { get; }
        string Icon { get; }
        void HandleStart(double x, double y);
        void HandleEnd(double x, double y);
        UIElement Draw();
        IShape Clone();

        SolidColorBrush ColorBrush { get; set; }

        int Thickness { get; set; }

        DoubleCollection Dash { get; set; }

    }
}

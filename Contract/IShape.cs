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
        UIElement Draw(SolidColorBrush brush, int thickness);
        IShape Clone();
    }
}

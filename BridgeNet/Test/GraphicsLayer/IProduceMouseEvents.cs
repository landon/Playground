using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsLayer
{
    public interface IProduceMouseEvents
    {
        event Action<double, double> MouseMoved;
        event Action<double, double, MouseButton> MouseButtonUp;
        event Action<double, double, MouseButton> MouseButtonDown;
        event Action<double, double, MouseButton> MouseButtonDoubleClicked;
    }
}

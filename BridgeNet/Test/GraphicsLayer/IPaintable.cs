using System;
using System.Collections.Generic;
using System.Text;

namespace GraphicsLayer
{
    public interface IPaintable
    {
        void Paint(IGraphics g, int width, int height);
    }
}

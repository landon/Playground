using System;
using System.Collections.Generic;
using System.Text;

namespace Graphs
{
    public interface IHittable
    {
        bool Hit(double x, double y);
    }
}

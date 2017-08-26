using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;

namespace TouchTest
{
    class TouchHistory : Item
    {
        List<Point> _events = new List<Point>();
        public void AddEvent(Touch t)
        {
            var now = DateTime.Now;
            _events.Add(new Point(now, t));
        }

        public class Point
        {
            public DateTime Time { get; private set; }
            public int ClientX { get; private set; }
            public int ClientY { get; private set; }
            public int PageX {get; private set; }
            public int PageY { get; private set; }
            public int ScreenX { get; private set; }
            public int ScreenY { get; private set; }
            public int RadiusX { get; private set; }
            public int RadiusY { get; private set; }
            public double Force { get; private set; }
            public int RotationAngle { get; private set; }

            public Point(DateTime time, Touch t)
            {
                Time = time;
                ClientX = t.ClientX;
                ClientY = t.ClientY;
                PageX = t.PageX;
                PageY = t.PageY;
                ScreenX = t.ScreenX;
                ScreenY = t.ScreenY;
                RadiusX = t.RadiusX;
                RadiusY = t.RadiusY;
                Force = t.Force;
                RotationAngle = t.RotationAngle;
            }
        }

        public void Draw(CanvasRenderingContext2D context)
        {
            if (_events.Count > 1)
            {
                context.MoveTo(_events[0].ClientX, _events[0].ClientY);
                foreach(var e in _events.Skip(1))
                {
                    context.LineTo(e.ClientX, e.ClientY);
                }
                context.Stroke();
            }
        }
    }
}

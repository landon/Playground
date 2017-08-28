using Bridge;
using Bridge.Html5;
using Newtonsoft.Json;
using System;
using System.Linq;


namespace TouchTest
{
    public class App
    {
        static HTMLCanvasElement _canvas; 
        static CanvasRenderingContext2D _context;
        static Set<TouchHistory> _history = new Set<TouchHistory>();

        public static void Main()
        {
            Window.OnLoad += OnLoad;
            Window.OnResize += OnResize;
            Window.OnDeviceOrientation += OnDeviceOrientation;

            _canvas = Document.CreateElement<HTMLCanvasElement>("canvas");

            Document.Body.AppendChild(_canvas);
        }

        static void OnDeviceOrientation(Event e)
        {
            GoFull();
        }

        static void OnResize(Event e)
        {
            GoFull();
        }

        static void GoFull()
        {
            _canvas.Width = Window.InnerWidth;
            _canvas.Height = Window.InnerHeight;
            Redraw();
        }

        static void OnLoad(Event e)
        {
            _context = _canvas.GetContext("2d").As<CanvasRenderingContext2D>();
            _context.StrokeStyle = "#222222";
            AttachTouchEvents(_canvas);
        }

        static void AttachTouchEvents(HTMLCanvasElement canvas)
        {
            canvas.OnTouchCancel += OnTouchCancel;
            canvas.OnTouchEnd += OnTouchEnd;
            canvas.OnTouchEnter += OnTouchEnter;
            canvas.OnTouchLeave += OnTouchLeave;
            canvas.OnTouchMove += OnTouchMove;
            canvas.OnTouchStart += OnTouchStart;
        }

        static void OnTouchCancel(TouchEvent<HTMLCanvasElement> e)
        {
            e.PreventDefault();
            foreach (var t in e.ChangedTouches)
                _history.Remove(t.Identifier);

            Redraw();
        }
        static void OnTouchEnd(TouchEvent<HTMLCanvasElement> e)
        {
            e.PreventDefault();
            foreach (var t in e.ChangedTouches)
                _history.Remove(t.Identifier);

            Redraw();
        }
        static void OnTouchEnter(TouchEvent<HTMLCanvasElement> e)
        {
            e.PreventDefault();
        }
        static void OnTouchLeave(TouchEvent<HTMLCanvasElement> e)
        {
            e.PreventDefault();
        }
        static void OnTouchMove(TouchEvent<HTMLCanvasElement> e)
        {
            e.PreventDefault();
            foreach (var t in e.ChangedTouches)
            {
                var th = _history.Find(t.Identifier);
                if (th != null)
                    th.AddEvent(t);
            }

            Redraw();   
        }

        static void OnTouchStart(TouchEvent<HTMLCanvasElement> e)
        {
            e.PreventDefault();
            foreach (var t in e.ChangedTouches)
            {
                var th = new TouchHistory() { Id = t.Identifier };
                th.AddEvent(t);
                _history.Add(th);
            }

            Redraw();
        }

        static void Redraw()
        {
            _context.ClearRect(0, 0, _canvas.Width, _canvas.Height);
            foreach (var h in _history)
                h.Draw(_context);

            
        }
    }
}
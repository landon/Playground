﻿using Bridge.Html5;
using GraphicsLayer;
using Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class TabCanvas : ICanvas
    {
        bool _ctrlDown;
        string _title;
        public HTMLCanvasElement Canvas { get; private set; }
        public GraphCanvas GraphCanvas { get; private set; }

        public TabCanvas(HTMLCanvasElement canvas, GraphCanvas graphCanvas)
        {
            GraphCanvas = graphCanvas;
            Canvas = canvas;
            GraphCanvas.Canvas = this;

            Canvas.OnDblClick += OnMouseDoubleClick;
            Canvas.OnMouseDown += OnMouseButtonDown;
            Canvas.OnMouseUp += OnMouseButtonUp;
            Canvas.OnMouseMove += OnMouseMove;
           

            Canvas.OnLoad = OnLoad;

            GraphCanvas.GraphModified += OnGraphModified;
            GraphCanvas.NameModified += OnNameModified;
        }

        void OnNameModified(string name)
        {
            Title = name;
        }

        void OnGraphModified(Graph g)
        {
            Invalidate();
        }

        void OnLoad(Event<HTMLCanvasElement> e)
        {
            Invalidate();
        }

        void OnMouseMove(MouseEvent<HTMLCanvasElement> e)
        {
            _ctrlDown = e.CtrlKey;
            if (MouseMoved != null)
                MouseMoved(e.ClientX, e.ClientY);
        }

        void OnMouseButtonDown(MouseEvent<HTMLCanvasElement> e)
        {
            _ctrlDown = e.CtrlKey;
           // Canvas.SetCapture(true);  // chrome does not seem to support this, what?

            if (e.ShiftKey)
            {
                if (MouseButtonDoubleClicked != null)
                    MouseButtonDoubleClicked(e.ClientX, e.ClientY, e.Button == 0 ? MouseButton.Left : MouseButton.Right);
            }
            else
            {
                if (MouseButtonDown != null)
                    MouseButtonDown(e.ClientX, e.ClientY, e.Button == 0 ? MouseButton.Left : MouseButton.Right);
            }
        }

        void OnMouseButtonUp(MouseEvent<HTMLCanvasElement> e)
        {
            _ctrlDown = e.CtrlKey;
           // Canvas.SetCapture(false);
            if (e.ShiftKey)
                return;


            if (MouseButtonUp != null)
                MouseButtonUp(e.ClientX, e.ClientY, e.Button == 0 ? MouseButton.Left : MouseButton.Right);
        }

        void OnMouseDoubleClick(MouseEvent<HTMLCanvasElement> e)
        {
        }

        public void SetClipboardText(string text)
        {
        }

        public string GetClipboardText()
        {
            return "";
        }

        public bool IsControlKeyDown
        {
            get { return _ctrlDown; }
        }

        public System.Collections.Generic.IEnumerable<object> SelectedObjects
        {
            set
            {
            }
        }

        public void Invalidate()
        {
            var graphics = new Graphics(Canvas);
            GraphCanvas.Paint(graphics, Canvas.Width, Canvas.Height);
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                Invalidate();
            }
        }

        public event Action<double, double> MouseMoved;
        public event Action<double, double, MouseButton> MouseButtonUp;
        public event Action<double, double, MouseButton> MouseButtonDown;
        public event Action<double, double, MouseButton> MouseButtonDoubleClicked;
    }
}
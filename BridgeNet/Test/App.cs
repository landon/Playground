using Bridge;
using Bridge.Html5;
using Graphs;
using GraphicsLayer;
using Newtonsoft.Json;
using System;

namespace Test
{
    public class App
    {
        public static void Main()
        {
           Window.OnLoad += OnWindowLoad;
        }

        static void OnWindowLoad(Event e)
        {
            var canvas = new HTMLCanvasElement();
            canvas.Id = "IAmWebGraphs";
            canvas.Width = (int)(0.7 * Window.InnerWidth);
            canvas.Height = (int)(1.0 * Window.InnerHeight);

            var G = new Graph();
            var graphCanvas = new GraphCanvas(G);
            var tc = new TabCanvas(canvas, graphCanvas);
            tc.Invalidate();

            Document.Body.FirstElementChild.FirstElementChild.FirstElementChild.FirstElementChild.AppendChild(canvas);
        }
    }
}
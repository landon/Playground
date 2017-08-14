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
        static HTMLElement _sageContainer;

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
            _sageContainer = Document.Body.FirstElementChild.FirstElementChild.FirstElementChild.Children[1];
        }

        public static void TellSage(string s)
        {
            if (_sageContainer.ChildElementCount > 0)
                _sageContainer.RemoveChild(_sageContainer.Children[0]);
            var div = new HTMLDivElement();
            div.ClassName = "compute";
            div.TextContent = s;
            _sageContainer.AppendChild(div);

            Script.Write("sagecell.makeSagecell({\"inputLocation\": \"div.compute\", hide: [\"permalink\"]});");
        }
    }
}
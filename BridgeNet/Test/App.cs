using Bridge;
using Bridge.Html5;
using Graphs;
using GraphicsLayer;
using Newtonsoft.Json;
using System;
using Bridge.jQuery2;

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

            var container = Document.Body.FirstElementChild;
            var dividedGrid = container.Children[1];
            dividedGrid.FirstElementChild.FirstElementChild.AppendChild(canvas);
            _sageContainer = dividedGrid.FirstElementChild.Children[1];

            jQuery.Select("#SageManual").On("click", () => tc.SageManual());
            jQuery.Select("#SageChromaticNumber").On("click", () => tc.SageChromaticNumber());
            jQuery.Select("#SageChromaticPolynomial").On("click", () => tc.SageChromaticPolynomial());
        }

        public static void TellSage(string s)
        {
            AppendSageDiv(s);

            Script.Write("sagecell.makeSagecell({\"inputLocation\": \"div.compute\", hide: [\"permalink\"], autoeval:false});");
        }

        public static void TellSageAuto(string s)
        {
            AppendSageDiv(s);

            Script.Write("sagecell.makeSagecell({\"inputLocation\": \"div.compute\", hide: [\"permalink\", \"evalButton\", \"fullScreen\", \"editor\"], autoeval:true});");
        }

        static void AppendSageDiv(string s)
        {
            if (_sageContainer.ChildElementCount > 0)
                _sageContainer.RemoveChild(_sageContainer.Children[0]);
            var div = new HTMLDivElement();
            div.ClassName = "compute";
            div.TextContent = s;
            _sageContainer.AppendChild(div);
        }

        
    }
}
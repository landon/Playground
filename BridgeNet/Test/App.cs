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
            var canvas = new HTMLCanvasElement();
            canvas.Width = 800;
            canvas.Height = 600;

            var v = new Vertex(0.5, 0.1);
            var w = new Vertex(0.3, 0.5);
            v.Color = new ARGB(100, 0, 0);
            w.Color = new ARGB(0, 255, 0);
            var G = new Graph();
            G.AddVertex(v);
            G.AddVertex(w);
            G.AddEdge(v, w);

            var graphCanvas = new GraphCanvas(G);
            var tc = new TabCanvas(canvas, graphCanvas);
            tc.Invalidate();

            Document.Body.AppendChild(canvas);
        }
    }
}
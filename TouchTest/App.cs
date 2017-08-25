using Bridge;
using Bridge.Html5;
using Newtonsoft.Json;
using System;

namespace TouchTest
{
    public class App
    {
        public static void Main()
        {
            var canvas = Document.CreateElement<HTMLCanvasElement>("TheCanvas");
            canvas.ClassName = "full";
            Document.Body.AppendChild(canvas);
        }
    }
}
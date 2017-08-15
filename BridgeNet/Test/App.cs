using Bridge;
using Bridge.Html5;
using Graphs;
using GraphicsLayer;
using Newtonsoft.Json;
using System;
using Bridge.jQuery2;
using System.Collections.Generic;

namespace Test
{
    public class App
    {
        static HTMLElement _sageContainer;
        static HTMLLIElement _newSheetTab;
        static string _currentTabCanvas;
        static int _tabID = 1;

        static Dictionary<string, TabCanvas> _canvasLookup = new Dictionary<string, TabCanvas>();

        public static void Main()
        {
            Window.OnLoad += OnWindowLoad;
        }

        static void OnWindowLoad(Event eee)
        {
            Window.AddEventListener("copy", (Action<Event>)((Event e) =>
            {
                e.PreventDefault();
                var tc = _canvasLookup[_currentTabCanvas];
                var s = tc.GraphCanvas.DoCopy();
                if (!string.IsNullOrEmpty(s))
                    e.ToDynamic().clipboardData.setData("Text", s);
            }));

            Window.AddEventListener("paste", (Action<Event>)((Event e) =>
            {
                e.PreventDefault();
                var s = e.ToDynamic().clipboardData.getData("Text");
                var tc = _canvasLookup[_currentTabCanvas];
                tc.GraphCanvas.DoPaste(s ?? "");
            }));

            jQuery.Document.On("keydown", (Action<KeyboardEvent>)OnKeyDown);
            jQuery.Document.On("contextmenu", ".IAmAGraphCanvas", (Action<Event>)((Event e) => { e.PreventDefault(); }));


            _sageContainer = Document.GetElementById("SageContainer");
            jQuery.Select("#SageManual").On("click", () => CurrentTabCanvas.SageManual());
            jQuery.Select("#SageChromaticNumber").On("click", () => CurrentTabCanvas.SageChromaticNumber());
            jQuery.Select("#SageChromaticPolynomial").On("click", () => CurrentTabCanvas.SageChromaticPolynomial());
            jQuery.Select("#SageGraph6").On("click", () => CurrentTabCanvas.SageGraph6());
            jQuery.Select("#SageSparse6").On("click", () => CurrentTabCanvas.SageSparse6());

            NewTab();
            AddNewSheetTab();

            jQuery.Select(".nav-tabs").On("click", "a", (Action<Event>)((Event ee) =>
            {
                ee.PreventDefault();
                if (!jQuery.This.HasClass("new-sheet-anchor"))
                {
                    var d = jQuery.This.ToDynamic();
                    _currentTabCanvas = ee.Target.As<HTMLAnchorElement>().Href;
                    var canvas = CurrentTabCanvas.Canvas;
                    d.tab("show");
                }
            }));

            jQuery.Select(".new-sheet-anchor").On("click", (Action<Event>)((Event ee) =>
            {
                ee.PreventDefault();
                NewTab();
            }));
        }

        static void AddNewSheetTab()
        {
            var tabControl = Document.GetElementById("TabControl");
            _newSheetTab = new HTMLLIElement();
            _newSheetTab.Id = "new-sheet";

            var anchor = new HTMLAnchorElement();
            anchor.ClassName = "new-sheet-anchor";
            anchor.TextContent = "+";
            anchor.Href = "#";

            _newSheetTab.AppendChild(anchor);
            tabControl.AppendChild(_newSheetTab);
        }

        public static void NewTab(string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = "sheet " + _tabID;

            var canvas = new HTMLCanvasElement();
            canvas.Id = "Tab" + _tabID;
            canvas.Width = (int)(Window.InnerWidth);
            canvas.Height = (int)(Window.InnerHeight);
            canvas.ClassName = "IAmAGraphCanvas";

            canvas.OnShow += delegate 
            {
                canvas.Width = canvas.ParentElement.ClientWidth;
                canvas.Height = canvas.ParentElement.ClientHeight;
            };

            var G = new Graph();
            var graphCanvas = new GraphCanvas(G);
            var tc = new TabCanvas(canvas, graphCanvas);
            tc.Invalidate();

            var tabPane = new HTMLDivElement();
            tabPane.ClassName = "tab-pane active";
            tabPane.Id = "Tab" + _tabID;
            tabPane.AppendChild(canvas);

            var tabControlContent = Document.GetElementById("TabControlContent");
            foreach (var child in tabControlContent.Children)
                child.ClassName = "tab-pane";
            tabControlContent.AppendChild(tabPane);

            var tab = new HTMLLIElement();
            tab.ClassName = "active";
            
            var anchor = new HTMLAnchorElement();
            anchor.SetAttribute("data-toggle", "tab");
            anchor.TextContent = name;
            anchor.Href = "#Tab" + _tabID;
            _canvasLookup[anchor.Href] = tc;
            _currentTabCanvas = anchor.Href;

            tab.AppendChild(anchor);

            var tabControl = Document.GetElementById("TabControl");
            foreach (var child in tabControl.Children)
                child.ClassName = "narf";
            tabControl.InsertBefore(tab, _newSheetTab);

            _tabID++;
        }

        public static TabCanvas CurrentTabCanvas { get { return _canvasLookup[_currentTabCanvas]; } }

        static void OnKeyDown(KeyboardEvent e)
        {
            var tc = CurrentTabCanvas;

            switch (e.Key)
            {
                case "f":
                    tc.GraphCanvas.DoZoomFit();
                    break;
                case "-":
                    tc.GraphCanvas.DoZoom(-1, new Box(0.5, 0.5));
                    break;
                case "+":
                    tc.GraphCanvas.DoZoom(1, new Box(0.5, 0.5));
                    break;
                case "i":
                    tc.GraphCanvas.ToggleVertexIndices();
                    break;
                case "I":
                    tc.GraphCanvas.RotateVertexIndices();
                    break;
                case "j":
                    tc.GraphCanvas.ToggleEdgeIndices();
                    break;
                case "J":
                    tc.GraphCanvas.RotateEdgeIndices();
                    break;
                case "n":
                    NewTab();
                    break;
                case "r":
                    tc.GraphCanvas.DoReverseSelectedEdges();
                    break;
                case "R":
                    tc.GraphCanvas.DoRotateSelectedEdges();
                    break;
                case "y":
                    tc.GraphCanvas.DoRedo();
                    break;
                case "Y":
                    tc.GraphCanvas.DoUndo();
                    break;
                case "Delete":
                    tc.GraphCanvas.DoDelete();
                    break;

            }
        }

        public static void TellSage(string s)
        {
            AppendSageDiv(s);

            Script.Write("sagecell.makeSagecell({\"inputLocation\": \"div.compute\", hide: [\"permalink\", \"fullScreen\"], autoeval:false});");
            Script.Write("$(\"#SageModal\").modal('show');");
        }

        public static void TellSageAuto(string s)
        {
            AppendSageDiv(s);

            Script.Write("sagecell.makeSagecell({\"inputLocation\": \"div.compute\", hide: [\"permalink\", \"evalButton\", \"fullScreen\", \"editor\"], autoeval:true});");
            Script.Write("$(\"#SageModal\").modal('show');");
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
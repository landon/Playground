using Bridge;
using Bridge.Html5;
using Graphs;
using GraphicsLayer;
using Newtonsoft.Json;
using System;
using Bridge.jQuery2;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GraphsCore;

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
            Window.OnResize += OnResize;
            Window.OnDeviceOrientation += OnDeviceOrientation;
        }

        static void OnDeviceOrientation(Event e)
        {
            OnLayoutChange();
        }

        static void OnResize(Event e)
        {
            OnLayoutChange();
        }

        static void OnLayoutChange()
        {
            var tc = CurrentTabCanvas;
            if (tc != null)
                tc.OnLayoutChange();
        }

        static void OnWindowLoad(Event eee)
        {
            Window.AddEventListener("copy", (Action<Event>)((Event e) =>
            {
                if ((e.Target is HTMLSpanElement) || (e.Target is HTMLAnchorElement))
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

            #region Sage
            _sageContainer = Document.GetElementById("SageContainer");
            jQuery.Select("#SageManual").On("click", () => CurrentTabCanvas.SageManual());
            jQuery.Select("#SageGraph6").On("click", () => CurrentTabCanvas.SageGraph6());
            jQuery.Select("#SageSparse6").On("click", () => CurrentTabCanvas.SageSparse6());
            jQuery.Select("#SageChromaticNumber").On("click", () => CurrentTabCanvas.SageChromaticNumber());
            jQuery.Select("#SageChromaticPolynomial").On("click", () => CurrentTabCanvas.SageChromaticPolynomial());
            jQuery.Select("#SageColoring").On("click", () => CurrentTabCanvas.SageColoring());
            jQuery.Select("#SageIndependentSet").On("click", () => CurrentTabCanvas.SageIndependentSet());
            jQuery.Select("#SageIsPerfect").On("click", () => CurrentTabCanvas.SageIsPerfect());
            jQuery.Select("#SageMatchingPolynomial").On("click", () => CurrentTabCanvas.SageMatchingPolynomial());
            jQuery.Select("#SagePathwidth").On("click", () => CurrentTabCanvas.SagePathwidth());
            jQuery.Select("#SageTreewidth").On("click", () => CurrentTabCanvas.SageTreewidth());
            jQuery.Select("#SageTuttePolynomial").On("click", () => CurrentTabCanvas.SageTuttePolynomial());
            jQuery.Select("#SageVertexCover").On("click", () => CurrentTabCanvas.SageVertexCover());
            jQuery.Select("#SageCliqueComplex").On("click", () => CurrentTabCanvas.SageCliqueComplex());
            jQuery.Select("#SageCliqueMaximum").On("click", () => CurrentTabCanvas.SageCliqueMaximum());
            jQuery.Select("#SageCliqueNumber").On("click", () => CurrentTabCanvas.SageCliqueNumber());
            jQuery.Select("#SageCliquePolynomial").On("click", () => CurrentTabCanvas.SageCliquePolynomial());
            jQuery.Select("#SageBridges").On("click", () => CurrentTabCanvas.SageBridges());
            jQuery.Select("#SageGomoryHuTree").On("click", () => CurrentTabCanvas.SageGomoryHuTree());
            jQuery.Select("#SageRandomSpanningTree").On("click", () => CurrentTabCanvas.SageRandomSpanningTree());
            jQuery.Select("#SageApexVertices").On("click", () => CurrentTabCanvas.SageApexVertices());
            jQuery.Select("#SageIsApex").On("click", () => CurrentTabCanvas.SageIsApex());
            jQuery.Select("#SageIsArcTransitive").On("click", () => CurrentTabCanvas.SageIsArcTransitive());
            jQuery.Select("#SageIsAsteroidalTripleFree").On("click", () => CurrentTabCanvas.SageIsAsteroidalTripleFree());
            jQuery.Select("#SageIsBiconnected").On("click", () => CurrentTabCanvas.SageIsBiconnected());
            jQuery.Select("#SageIsBipartite").On("click", () => CurrentTabCanvas.SageIsBipartite());
            jQuery.Select("#SageIsBlockGraph").On("click", () => CurrentTabCanvas.SageIsBlockGraph());
            jQuery.Select("#SageIsCartesianProduct").On("click", () => CurrentTabCanvas.SageIsCartesianProduct());
            jQuery.Select("#SageIsDistanceRegular").On("click", () => CurrentTabCanvas.SageIsDistanceRegular());
            jQuery.Select("#SageIsEdgeTransitive").On("click", () => CurrentTabCanvas.SageIsEdgeTransitive());
            jQuery.Select("#SageIsEvenHoleFree").On("click", () => CurrentTabCanvas.SageIsEvenHoleFree());
            jQuery.Select("#SageIsForest").On("click", () => CurrentTabCanvas.SageIsForest());
            jQuery.Select("#SageIsHalfTransitive").On("click", () => CurrentTabCanvas.SageIsHalfTransitive());
            jQuery.Select("#SageIsLineGraph").On("click", () => CurrentTabCanvas.SageIsLineGraph());
            jQuery.Select("#SageIsLongAntiholeFree").On("click", () => CurrentTabCanvas.SageIsLongAntiholeFree());
            jQuery.Select("#SageIsLongHoleFree").On("click", () => CurrentTabCanvas.SageIsLongHoleFree());
            jQuery.Select("#SageIsOddHoleFree").On("click", () => CurrentTabCanvas.SageIsOddHoleFree());
            jQuery.Select("#SageIsOverfull").On("click", () => CurrentTabCanvas.SageIsOverfull());
            jQuery.Select("#SageIsPartialCube").On("click", () => CurrentTabCanvas.SageIsPartialCube());
            jQuery.Select("#SageIsPrime").On("click", () => CurrentTabCanvas.SageIsPrime());
            jQuery.Select("#SageIsSemiSymmetric").On("click", () => CurrentTabCanvas.SageIsSemiSymmetric());
            jQuery.Select("#SageIsSplit").On("click", () => CurrentTabCanvas.SageIsSplit());
            jQuery.Select("#SageIsStronglyRegular").On("click", () => CurrentTabCanvas.SageIsStronglyRegular());
            jQuery.Select("#SageIsTree").On("click", () => CurrentTabCanvas.SageIsTree());
            jQuery.Select("#SageIsTriangleFree").On("click", () => CurrentTabCanvas.SageIsTriangleFree());
            jQuery.Select("#SageIsWeaklyChordal").On("click", () => CurrentTabCanvas.SageIsWeaklyChordal());
            jQuery.Select("#SageOddGirth").On("click", () => CurrentTabCanvas.SageOddGirth());
            jQuery.Select("#SageFractionalChromaticIndex").On("click", () => CurrentTabCanvas.SageFractionalChromaticIndex());
            jQuery.Select("#SageHasPerfectMatching").On("click", () => CurrentTabCanvas.SageHasPerfectMatching());
            jQuery.Select("#SageKirchhoffSymanzikPolynomial").On("click", () => CurrentTabCanvas.SageKirchhoffSymanzikPolynomial());
            jQuery.Select("#SageLovaszTheta").On("click", () => CurrentTabCanvas.SageLovaszTheta());
            jQuery.Select("#SageMatching").On("click", () => CurrentTabCanvas.SageMatching());
            jQuery.Select("#SageMaximumAverageDegree").On("click", () => CurrentTabCanvas.SageMaximumAverageDegree());
            jQuery.Select("#SageSeidelAdjacencyMatrix").On("click", () => CurrentTabCanvas.SageSeidelAdjacencyMatrix());
            jQuery.Select("#SageNetworkxGraph").On("click", () => CurrentTabCanvas.SageNetworkxGraph());
            jQuery.Select("#SageAdjacencyMatrix").On("click", () => CurrentTabCanvas.SageAdjacencyMatrix());
            jQuery.Select("#SageIncidenceMatrix").On("click", () => CurrentTabCanvas.SageIncidenceMatrix());
            jQuery.Select("#SageDistanceMatrix").On("click", () => CurrentTabCanvas.SageDistanceMatrix());
            jQuery.Select("#SageKirchhoffMatrix").On("click", () => CurrentTabCanvas.SageKirchhoffMatrix());
            jQuery.Select("#SageDensity").On("click", () => CurrentTabCanvas.SageDensity());
            jQuery.Select("#SageOrder").On("click", () => CurrentTabCanvas.SageOrder());
            jQuery.Select("#SageSize").On("click", () => CurrentTabCanvas.SageSize());
            jQuery.Select("#SageAverageDegree").On("click", () => CurrentTabCanvas.SageAverageDegree());
            jQuery.Select("#SageDegreeSequence").On("click", () => CurrentTabCanvas.SageDegreeSequence());
            jQuery.Select("#SageCycleBasis").On("click", () => CurrentTabCanvas.SageCycleBasis());
            jQuery.Select("#SageAllPaths").On("click", () => CurrentTabCanvas.SageAllPaths());
            jQuery.Select("#SageTrianglesCount").On("click", () => CurrentTabCanvas.SageTrianglesCount());
            jQuery.Select("#SageSpectrum").On("click", () => CurrentTabCanvas.SageSpectrum());
            jQuery.Select("#SageEigenvectors").On("click", () => CurrentTabCanvas.SageEigenvectors());
            jQuery.Select("#SageEigenspaces").On("click", () => CurrentTabCanvas.SageEigenspaces());
            jQuery.Select("#SageAutomorphismGroup").On("click", () => CurrentTabCanvas.SageAutomorphismGroup());
            jQuery.Select("#SageIsVertexTransitive").On("click", () => CurrentTabCanvas.SageIsVertexTransitive());
            jQuery.Select("#SageCanonicalLabel").On("click", () => CurrentTabCanvas.SageCanonicalLabel());
            jQuery.Select("#SageIsCayley").On("click", () => CurrentTabCanvas.SageIsCayley());
            jQuery.Select("#SageIsEulerian").On("click", () => CurrentTabCanvas.SageIsEulerian());
            jQuery.Select("#SageIsPlanar").On("click", () => CurrentTabCanvas.SageIsPlanar());
            jQuery.Select("#SageIsRegular").On("click", () => CurrentTabCanvas.SageIsRegular());
            jQuery.Select("#SageIsChordal").On("click", () => CurrentTabCanvas.SageIsChordal());
            jQuery.Select("#SageIsCirculant").On("click", () => CurrentTabCanvas.SageIsCirculant());
            jQuery.Select("#SageIsInterval").On("click", () => CurrentTabCanvas.SageIsInterval());
            jQuery.Select("#SageIsGallaiTree").On("click", () => CurrentTabCanvas.SageIsGallaiTree());
            jQuery.Select("#SageIsClique").On("click", () => CurrentTabCanvas.SageIsClique());
            jQuery.Select("#SageIsCycle").On("click", () => CurrentTabCanvas.SageIsCycle());
            jQuery.Select("#SageIsIndependentSet").On("click", () => CurrentTabCanvas.SageIsIndependentSet());
            jQuery.Select("#SageIsTransitivelyReduced").On("click", () => CurrentTabCanvas.SageIsTransitivelyReduced());
            jQuery.Select("#SageIsEquitable").On("click", () => CurrentTabCanvas.SageIsEquitable());
            jQuery.Select("#SageEccentricity").On("click", () => CurrentTabCanvas.SageEccentricity());
            jQuery.Select("#SageRadius").On("click", () => CurrentTabCanvas.SageRadius());
            jQuery.Select("#SageDiameter").On("click", () => CurrentTabCanvas.SageDiameter());
            jQuery.Select("#SageGirth").On("click", () => CurrentTabCanvas.SageGirth());
            jQuery.Select("#SageEdgeConnectivity").On("click", () => CurrentTabCanvas.SageEdgeConnectivity());
            jQuery.Select("#SageVertexConnectivity").On("click", () => CurrentTabCanvas.SageVertexConnectivity());
            jQuery.Select("#SageIsHamiltonian").On("click", () => CurrentTabCanvas.SageIsHamiltonian());
            jQuery.Select("#SageCharacteristicPolynomial").On("click", () => CurrentTabCanvas.SageCharacteristicPolynomial());
            jQuery.Select("#SageGenus").On("click", () => CurrentTabCanvas.SageGenus());


            #region Layout
            jQuery.Select("#SageLayoutSprings").On("click", () => CurrentTabCanvas.SageLayoutSpring());
            jQuery.Select("#SageLayoutRanked").On("click", () => CurrentTabCanvas.SageLayoutRanked());
            jQuery.Select("#SageLayoutRandom").On("click", () => CurrentTabCanvas.SageLayoutExtendRandomly());
            jQuery.Select("#SageLayoutCircular").On("click", () => CurrentTabCanvas.SageLayoutCircular());
            jQuery.Select("#SageLayoutTree").On("click", () => CurrentTabCanvas.SageLayoutTree());
            jQuery.Select("#SageLayoutGraphViz").On("click", () => CurrentTabCanvas.SageLayoutGraphviz());
            jQuery.Select("#SageLayoutPlanar").On("click", () => CurrentTabCanvas.SageLayoutPlanar());
            #endregion

            jQuery.Select(".NamedGraph").On("click", (Action<Event>)((Event ee) =>
            {
                TabCanvas.SageLoadNamedGraph(((HTMLElement)ee.CurrentTarget).TextContent);
            }));
            #endregion



            NewTab();
            AddNewSheetTab();

            jQuery.Select(".nav-tabs").On("click", "a", (Action<Event>)((Event ee) =>
            {
                ee.PreventDefault();
                if (!jQuery.This.HasClass("new-sheet-anchor"))
                {
                    var d = jQuery.This.ToDynamic();
                    _currentTabCanvas = ee.Target.As<HTMLAnchorElement>().Href;
                    var canvas = _canvasLookup[_currentTabCanvas].Canvas;
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

        public static void NewTab(string name = null, Graph G = null)
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

            if (G == null)
                G = new Graph();
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
                case "r":
                    tc.GraphCanvas.DoReverseSelectedEdges();
                    break;
                case "R":
                    tc.GraphCanvas.DoRotateSelectedEdges();
                    break;
                case "y":
                    if (e.CtrlKey)
                        tc.GraphCanvas.DoRedo();
                    break;
                case "z":
                    if (e.CtrlKey)
                        tc.GraphCanvas.DoUndo();
                    break;

            }
        }

        public static void AskSage(string s)
        {
            var div = AppendSageDiv(s);

            Script.Write("sagecell.makeSagecell({\"inputLocation\": \"div.compute\", hide: [\"permalink\", \"fullScreen\"], autoeval:false});");
            Script.Write("$(\"#SageModal\").modal('show');");
        }

        public static void AskSageAuto(string s)
        {
            AppendSageDiv(s);

            Script.Write("sagecell.makeSagecell({\"inputLocation\": \"div.compute\", hide: [\"permalink\", \"evalButton\", \"fullScreen\", \"editor\"], autoeval:true});");
            Script.Write("$(\"#SageModal\").modal('show');");
        }

        public static async Task<string> AskSageAsync(Graph G, string sageCodeAboutG)
        {
            return await App.AskSageAsync("G = Graph('" + G.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + sageCodeAboutG);
        }

        public static async Task<string> AskSageAsync(string sageCode)
        {
            var output = Document.GetElementById("SageSecretOutputHole");
            output.TextContent = "working";
            AppendSageDiv(sageCode);

            Script.Write("sagecell.makeSagecell({\"inputLocation\": \"div.compute\", hide: [\"permalink\", \"evalButton\", \"fullScreen\", \"editor\"], autoeval:true, outputLocation:\"#SageSecretOutputHole\"});");

            while (!output.TextContent.Contains("Accepted:"))
                await Task.Delay(100);

            var response = output.TextContent;
            if (string.IsNullOrEmpty(response))
                return "flibble";

            var ss = response.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(line => line.Contains("Accepted:"));
            if (string.IsNullOrEmpty(ss))
                return "flabble";

            ss = ss.Substring(ss.IndexOf("text/plain") + "text/plain".Length + 3);
            ss = ss.Substring(0, ss.IndexOf("\""));

            return ss;
        }

        static HTMLDivElement AppendSageDiv(string s)
        {
            if (_sageContainer.ChildElementCount > 0)
                _sageContainer.RemoveChild(_sageContainer.Children[0]);
            var div = new HTMLDivElement();
            div.ClassName = "compute";
            div.TextContent = s;
            _sageContainer.AppendChild(div);

            return div;
        }
    }
}
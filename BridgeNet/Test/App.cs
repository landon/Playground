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

            #region Sage
            _sageContainer = Document.GetElementById("SageContainer");
            jQuery.Select("#SageManual").On("click", () => CurrentTabCanvas.SageManual());
            jQuery.Select("#SageGraph6").On("click", () => CurrentTabCanvas.SageGraph6());
            jQuery.Select("#SageSparse6").On("click", () => CurrentTabCanvas.SageSparse6());
            jQuery.Select("#SageChromaticNumber").On("click", () => CurrentTabCanvas.SageChromaticNumber());
            jQuery.Select("#SageChromaticPolynomial").On("click", () => CurrentTabCanvas.SageChromaticPolynomial());
            jQuery.Select("#SageChromaticQuasisymmetricFunction").On("click", () => CurrentTabCanvas.SageChromaticQuasisymmetricFunction());
            jQuery.Select("#SageChromaticSymmetricFunction").On("click", () => CurrentTabCanvas.SageChromaticSymmetricFunction());
            jQuery.Select("#SageColoring").On("click", () => CurrentTabCanvas.SageColoring());
            jQuery.Select("#SageConvexityProperties").On("click", () => CurrentTabCanvas.SageConvexityProperties());
            jQuery.Select("#SageHasHomomorphismTo").On("click", () => CurrentTabCanvas.SageHasHomomorphismTo());
            jQuery.Select("#SageIndependentSet").On("click", () => CurrentTabCanvas.SageIndependentSet());
            jQuery.Select("#SageIndependentSetOfRepresentatives").On("click", () => CurrentTabCanvas.SageIndependentSetOfRepresentatives());
            jQuery.Select("#SageIsPerfect").On("click", () => CurrentTabCanvas.SageIsPerfect());
            jQuery.Select("#SageMatchingPolynomial").On("click", () => CurrentTabCanvas.SageMatchingPolynomial());
            jQuery.Select("#SageMinor").On("click", () => CurrentTabCanvas.SageMinor());
            jQuery.Select("#SagePathwidth").On("click", () => CurrentTabCanvas.SagePathwidth());
            jQuery.Select("#SageRankDecomposition").On("click", () => CurrentTabCanvas.SageRankDecomposition());
            jQuery.Select("#SageTopologicalMinor").On("click", () => CurrentTabCanvas.SageTopologicalMinor());
            jQuery.Select("#SageTreewidth").On("click", () => CurrentTabCanvas.SageTreewidth());
            jQuery.Select("#SageTuttePolynomial").On("click", () => CurrentTabCanvas.SageTuttePolynomial());
            jQuery.Select("#SageVertexCover").On("click", () => CurrentTabCanvas.SageVertexCover());
            jQuery.Select("#SageBipartiteColor").On("click", () => CurrentTabCanvas.SageBipartiteColor());
            jQuery.Select("#SageBipartiteSets").On("click", () => CurrentTabCanvas.SageBipartiteSets());
            jQuery.Select("#SageGraph6String").On("click", () => CurrentTabCanvas.SageGraph6String());
            jQuery.Select("#SageIsDirected").On("click", () => CurrentTabCanvas.SageIsDirected());
            jQuery.Select("#SageJoin").On("click", () => CurrentTabCanvas.SageJoin());
            jQuery.Select("#SageSparse6String").On("click", () => CurrentTabCanvas.SageSparse6String());
            jQuery.Select("#SageToDirected").On("click", () => CurrentTabCanvas.SageToDirected());
            jQuery.Select("#SageToUndirected").On("click", () => CurrentTabCanvas.SageToUndirected());
            jQuery.Select("#SageWriteToEps").On("click", () => CurrentTabCanvas.SageWriteToEps());
            jQuery.Select("#SageCliqueComplex").On("click", () => CurrentTabCanvas.SageCliqueComplex());
            jQuery.Select("#SageCliqueMaximum").On("click", () => CurrentTabCanvas.SageCliqueMaximum());
            jQuery.Select("#SageCliqueNumber").On("click", () => CurrentTabCanvas.SageCliqueNumber());
            jQuery.Select("#SageCliquePolynomial").On("click", () => CurrentTabCanvas.SageCliquePolynomial());
            jQuery.Select("#SageCliquesContainingVertex").On("click", () => CurrentTabCanvas.SageCliquesContainingVertex());
            jQuery.Select("#SageCliquesGetCliqueBipartite").On("click", () => CurrentTabCanvas.SageCliquesGetCliqueBipartite());
            jQuery.Select("#SageCliquesGetMaxCliqueGraph").On("click", () => CurrentTabCanvas.SageCliquesGetMaxCliqueGraph());
            jQuery.Select("#SageCliquesMaximal").On("click", () => CurrentTabCanvas.SageCliquesMaximal());
            jQuery.Select("#SageCliquesMaximum").On("click", () => CurrentTabCanvas.SageCliquesMaximum());
            jQuery.Select("#SageCliquesNumberOf").On("click", () => CurrentTabCanvas.SageCliquesNumberOf());
            jQuery.Select("#SageCliquesVertexCliqueNumber").On("click", () => CurrentTabCanvas.SageCliquesVertexCliqueNumber());
            jQuery.Select("#SageBoundedOutdegreeOrientation").On("click", () => CurrentTabCanvas.SageBoundedOutdegreeOrientation());
            jQuery.Select("#SageBridges").On("click", () => CurrentTabCanvas.SageBridges());
            jQuery.Select("#SageDegreeConstrainedSubgraph").On("click", () => CurrentTabCanvas.SageDegreeConstrainedSubgraph());
            jQuery.Select("#SageGomoryHuTree").On("click", () => CurrentTabCanvas.SageGomoryHuTree());
            jQuery.Select("#SageMinimumOutdegreeOrientation").On("click", () => CurrentTabCanvas.SageMinimumOutdegreeOrientation());
            jQuery.Select("#SageOrientations").On("click", () => CurrentTabCanvas.SageOrientations());
            jQuery.Select("#SageRandomSpanningTree").On("click", () => CurrentTabCanvas.SageRandomSpanningTree());
            jQuery.Select("#SageSpanningTrees").On("click", () => CurrentTabCanvas.SageSpanningTrees());
            jQuery.Select("#SageStrongOrientation").On("click", () => CurrentTabCanvas.SageStrongOrientation());
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
            jQuery.Select("#SageCores").On("click", () => CurrentTabCanvas.SageCores());
            jQuery.Select("#SageFractionalChromaticIndex").On("click", () => CurrentTabCanvas.SageFractionalChromaticIndex());
            jQuery.Select("#SageHasPerfectMatching").On("click", () => CurrentTabCanvas.SageHasPerfectMatching());
            jQuery.Select("#SageIharaZetaFunctionInverse").On("click", () => CurrentTabCanvas.SageIharaZetaFunctionInverse());
            jQuery.Select("#SageKirchhoffSymanzikPolynomial").On("click", () => CurrentTabCanvas.SageKirchhoffSymanzikPolynomial());
            jQuery.Select("#SageLovaszTheta").On("click", () => CurrentTabCanvas.SageLovaszTheta());
            jQuery.Select("#SageMagnitudeFunction").On("click", () => CurrentTabCanvas.SageMagnitudeFunction());
            jQuery.Select("#SageMatching").On("click", () => CurrentTabCanvas.SageMatching());
            jQuery.Select("#SageMaximumAverageDegree").On("click", () => CurrentTabCanvas.SageMaximumAverageDegree());
            jQuery.Select("#SageModularDecomposition").On("click", () => CurrentTabCanvas.SageModularDecomposition());
            jQuery.Select("#SagePerfectMatchings").On("click", () => CurrentTabCanvas.SagePerfectMatchings());
            jQuery.Select("#SageSeidelAdjacencyMatrix").On("click", () => CurrentTabCanvas.SageSeidelAdjacencyMatrix());
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
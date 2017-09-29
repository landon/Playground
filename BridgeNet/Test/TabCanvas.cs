using Bridge.Html5;
using GraphicsLayer;
using Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphsCore;
using Algorithms;

namespace Test
{
    public class TabCanvas : ICanvas
    {
        bool _ctrlDown;
        string _title;
        public HTMLCanvasElement Canvas { get; private set; }
        public GraphCanvas GraphCanvas { get; private set; }

        static List<ARGB> Colors = new List<ARGB>() {
            new ARGB(255, 0, 0), new ARGB(0, 0, 255), new ARGB(0, 255, 0),
            new ARGB(255, 255, 0), new ARGB(95, 158, 160), new ARGB(139, 69, 19),
            new ARGB(30, 144, 255), new ARGB(64, 224, 208), new ARGB(218, 112, 214) };

        public TabCanvas(HTMLCanvasElement canvas, GraphCanvas graphCanvas)
        {
            GraphCanvas = graphCanvas;
            Canvas = canvas;
            GraphCanvas.Canvas = this;

            Canvas.OnDblClick += OnMouseDoubleClick;
            Canvas.OnMouseDown += OnMouseButtonDown;
            Canvas.OnMouseUp += OnMouseButtonUp;
            Canvas.OnMouseMove += OnMouseMove;

            Canvas.OnTouchCancel += OnTouchCancel;
            Canvas.OnTouchEnd += OnTouchEnd;
            Canvas.OnTouchEnter += OnTouchEnter;
            Canvas.OnTouchLeave += OnTouchLeave;
            Canvas.OnTouchMove += OnTouchMove;
            Canvas.OnTouchStart += OnTouchStart;

            Canvas.OnLoad = OnLoad;

            GraphCanvas.GraphModified += OnGraphModified;
            GraphCanvas.NameModified += OnNameModified;
        }

        public void OnLayoutChange()
        {
            Canvas.Width = Window.InnerWidth;
            Canvas.Height = Window.InnerHeight;
            Invalidate();
        }

        void OnNameModified(string name)
        {
            Title = name;
        }

        #region touch
        void OnTouchCancel(TouchEvent<HTMLCanvasElement> e)
        {
        }
        void OnTouchEnd(TouchEvent<HTMLCanvasElement> e)
        {
            if (MouseButtonUp != null)
                MouseButtonUp(e.LayerX, e.LayerY, MouseButton.Left);
        }
        void OnTouchEnter(TouchEvent<HTMLCanvasElement> e)
        {
        }
        void OnTouchLeave(TouchEvent<HTMLCanvasElement> e)
        {
        }
        void OnTouchMove(TouchEvent<HTMLCanvasElement> e)
        {
            if (MouseMoved != null)
                MouseMoved(e.LayerX, e.LayerY);
        }
        void OnTouchStart(TouchEvent<HTMLCanvasElement> e)
        {
            if (MouseButtonDown != null)
                MouseButtonDown(e.LayerX, e.LayerY, MouseButton.Left);
        }
        #endregion

        #region Sage
        internal void SageManual()
        {
            App.AskSage("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine);
        }
        internal void SageChromaticPolynomial()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.chromatic_polynomial()");
        }
        internal void SageGraph6()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.graph6_string()");
        }
        internal void SageSparse6()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.sparse6_string()");
        }
        internal void SageChromaticNumber()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.chromatic_number()");
        }
        internal void SageChromaticQuasisymmetricFunction()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.chromatic_quasisymmetric_function()");
        }
        internal void SageChromaticSymmetricFunction()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.chromatic_symmetric_function()");
        }
        internal async void SageColoring()
        {
            var coloring = await App.AskSageAsync(GraphCanvas.Graph, "G.coloring()");
            var cc = coloring.Split(new[] { "]," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Replace("[", "").Replace("]", "").Replace(" ", "").Replace(Environment.NewLine, "").Replace("\\n", "").Split(',').Select(x => int.Parse(x)).ToList()).ToList();

            int i = 0;
            foreach(var v in GraphCanvas.Graph.Vertices)
            {
                var c = cc.FirstIndex(l => l.Contains(i));
                v.Color = Colors[c % Colors.Count];
                i++;
            }

            Invalidate();
        }
        internal void SageConvexityProperties()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.convexity_properties()");
        }
        internal void SageHasHomomorphismTo()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.has_homomorphism_to()");
        }
        internal async void SageIndependentSet()
        {
            var coloring = await App.AskSageAsync(GraphCanvas.Graph, "G.independent_set()");
            var cc = coloring.Replace("[", "").Replace("]", "").Replace(" ", "").Replace(Environment.NewLine, "").Replace("\\n", "").Split(',').Select(x => int.Parse(x)).ToList();

            foreach (var v in GraphCanvas.Graph.Vertices)
                v.Color = Vertex.DefaultFillBrushColor;

            foreach (var v in cc)
                GraphCanvas.Graph.Vertices[v].Color = Colors[1];

            Invalidate();
        }
        internal void SageIndependentSetOfRepresentatives()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.independent_set_of_representatives()");
        }
        internal void SageIsPerfect()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_perfect()");
        }
        internal void SageMatchingPolynomial()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.matching_polynomial()");
        }
        internal void SageMinor()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.minor()");
        }
        internal void SagePathwidth()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.pathwidth()");
        }
        internal void SageRankDecomposition()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.rank_decomposition()");
        }
        internal void SageTopologicalMinor()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.topological_minor()");
        }
        internal void SageTreewidth()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.treewidth()");
        }
        internal void SageTuttePolynomial()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.tutte_polynomial()");
        }
        internal async void SageVertexCover()
        {
            var coloring = await App.AskSageAsync(GraphCanvas.Graph, "G.vertex_cover()");
            var cc = coloring.Replace("[", "").Replace("]", "").Replace(" ", "").Replace(Environment.NewLine, "").Replace("\\n", "").Split(',').Select(x => int.Parse(x)).ToList();

            foreach (var v in GraphCanvas.Graph.Vertices)
                v.Color = Vertex.DefaultFillBrushColor;

            foreach (var v in cc)
                GraphCanvas.Graph.Vertices[v].Color = Colors[1];

            Invalidate();
        }
        internal void SageBipartiteColor()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.bipartite_color()");
        }
        internal void SageBipartiteSets()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.bipartite_sets()");
        }
        internal void SageGraph6String()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.graph6_string()");
        }
        internal void SageIsDirected()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_directed()");
        }
        internal void SageJoin()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.join()");
        }
        internal void SageSparse6String()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.sparse6_string()");
        }
        internal void SageToDirected()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.to_directed()");
        }
        internal void SageToUndirected()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.to_undirected()");
        }
        internal void SageWriteToEps()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.write_to_eps()");
        }
        internal void SageCliqueComplex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clique_complex()");
        }
        internal async void SageCliqueMaximum()
        {
            var coloring = await App.AskSageAsync(GraphCanvas.Graph, "G.clique_maximum()");
            var cc = coloring.Replace("[", "").Replace("]", "").Replace(" ", "").Replace(Environment.NewLine, "").Replace("\\n", "").Split(',').Select(x => int.Parse(x)).ToList();

            foreach (var v in GraphCanvas.Graph.Vertices)
                v.Color = Vertex.DefaultFillBrushColor;

            foreach (var v in cc)
                GraphCanvas.Graph.Vertices[v].Color = Colors[1];

            Invalidate();
        }
        internal void SageCliqueNumber()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clique_number()");
        }
        internal void SageCliquePolynomial()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clique_polynomial()");
        }
        internal void SageCliquesContainingVertex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_containing_vertex()");
        }
        internal void SageCliquesGetCliqueBipartite()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_get_clique_bipartite()");
        }
        internal void SageCliquesGetMaxCliqueGraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_get_max_clique_graph()");
        }
        internal void SageCliquesMaximal()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_maximal()");
        }
        internal void SageCliquesMaximum()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_maximum()");
        }
        internal void SageCliquesNumberOf()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_number_of()");
        }
        internal void SageCliquesVertexCliqueNumber()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_vertex_clique_number()");
        }
        internal void SageBoundedOutdegreeOrientation()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.bounded_outdegree_orientation()");
        }
        internal void SageBridges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.bridges()");
        }
        internal void SageDegreeConstrainedSubgraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.degree_constrained_subgraph()");
        }
        internal void SageGomoryHuTree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.gomory_hu_tree().plot()");
        }
        internal void SageMinimumOutdegreeOrientation()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.minimum_outdegree_orientation()");
        }
        internal void SageOrientations()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.orientations()");
        }
        internal async void SageRandomSpanningTree()
        {
            var edges = await App.AskSageAsync(GraphCanvas.Graph, "G.random_spanning_tree()");
            var ee = edges.Split(new[] { ")," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Replace("[", "").Replace("]", "").Replace("(","").Replace(")", "").Replace(" ", "").Replace(Environment.NewLine, "").Replace("\\n", "").Replace("\\n","").Split(',').Select(x => int.Parse(x)).ToList()).ToList();

            foreach (var v in GraphCanvas.Graph.Vertices)
                v.Color = Vertex.DefaultFillBrushColor;

            foreach(var e in GraphCanvas.Graph.Edges)
                e.Color = new GraphicsLayer.ARGB(0, 0, 0);

            foreach (var e in ee)
            {
                var v1 = GraphCanvas.Graph.Vertices[e[0]];
                var v2 = GraphCanvas.Graph.Vertices[e[1]];

                GraphCanvas.Graph.Edges.First(x => x.V1 == v1 && x.V2 == v2 || x.V1 == v2 && x.V2 == v1).Color = Colors[1];
            }

            Invalidate();
        }
        internal void SageSpanningTrees()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.spanning_trees()");
        }
        internal void SageStrongOrientation()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.strong_orientation()");
        }
        internal async void SageApexVertices()
        {
            var coloring = await App.AskSageAsync(GraphCanvas.Graph, "G.apex_vertices()");
            var cc = coloring.Replace("[", "").Replace("]", "").Replace(" ", "").Replace(Environment.NewLine, "").Replace("\\n", "").Split(',').Select(x => int.Parse(x)).ToList();

            foreach (var v in GraphCanvas.Graph.Vertices)
                v.Color = Vertex.DefaultFillBrushColor;

            foreach (var v in cc)
                GraphCanvas.Graph.Vertices[v].Color = Colors[1];

            Invalidate();
        }

        internal void SageIsApex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_apex()");
        }

        internal void SageIsArcTransitive()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_arc_transitive()");
        }
        internal void SageIsAsteroidalTripleFree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_asteroidal_triple_free()");
        }

      

        internal void SageIsBiconnected()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_biconnected()");
        }
        internal void SageIsBipartite()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_bipartite()");
        }
        internal void SageIsBlockGraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_block_graph()");
        }
        internal void SageIsCartesianProduct()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_cartesian_product()");
        }
        internal void SageIsDistanceRegular()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_distance_regular()");
        }
        internal void SageIsEdgeTransitive()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_edge_transitive()");
        }
        internal void SageIsEvenHoleFree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_even_hole_free()");
        }
        internal void SageIsForest()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_forest()");
        }
        internal void SageIsHalfTransitive()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_half_transitive()");
        }
        internal void SageIsLineGraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_line_graph()");
        }
        internal void SageIsLongAntiholeFree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_long_antihole_free()");
        }
        internal void SageIsLongHoleFree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_long_hole_free()");
        }
        internal void SageIsOddHoleFree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_odd_hole_free()");
        }
        internal void SageIsOverfull()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_overfull()");
        }
        internal void SageIsPartialCube()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_partial_cube()");
        }
        internal void SageIsPrime()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_prime()");
        }
        internal void SageIsSemiSymmetric()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_semi_symmetric()");
        }
        internal void SageIsSplit()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_split()");
        }
        internal void SageIsStronglyRegular()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_strongly_regular()");
        }
        internal void SageIsTree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_tree()");
        }
        internal void SageIsTriangleFree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_triangle_free()");
        }
        internal void SageIsWeaklyChordal()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_weakly_chordal()");
        }
        internal void SageOddGirth()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.odd_girth()");
        }
        internal void SageCores()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cores()");
        }
        internal void SageFractionalChromaticIndex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.fractional_chromatic_index()");
        }
        internal void SageHasPerfectMatching()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.has_perfect_matching()");
        }
        internal void SageIharaZetaFunctionInverse()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.ihara_zeta_function_inverse()");
        }
        internal void SageKirchhoffSymanzikPolynomial()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.kirchhoff_symanzik_polynomial()");
        }
        internal void SageLovaszTheta()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.lovasz_theta()");
        }
        internal void SageMagnitudeFunction()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.magnitude_function()");
        }
        internal void SageMatching()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.matching()");
        }
        internal void SageMaximumAverageDegree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.maximum_average_degree()");
        }
        internal void SageModularDecomposition()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.modular_decomposition()");
        }
        internal void SagePerfectMatchings()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.perfect_matchings()");
        }
        internal void SageSeidelAdjacencyMatrix()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.seidel_adjacency_matrix()");
        }
        internal void SageNetworkxGraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.networkx_graph()");
        }
        internal void SageIgraphGraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.igraph_graph()");
        }
        internal void SageToDictionary()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.to_dictionary()");
        }
        internal void SageCopy()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.copy()");
        }
        internal void SageExportToFile()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.export_to_file()");
        }
        internal void SageAdjacencyMatrix()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.adjacency_matrix()");
        }
        internal void SageIncidenceMatrix()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.incidence_matrix()");
        }
        internal void SageDistanceMatrix()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.distance_matrix()");
        }
        internal void SageWeightedAdjacencyMatrix()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.weighted_adjacency_matrix()");
        }
        internal void SageKirchhoffMatrix()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.kirchhoff_matrix()");
        }
        internal void SageHasLoops()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.has_loops()");
        }
        internal void SageAllowsLoops()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.allows_loops()");
        }
        internal void SageAllowLoops()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.allow_loops()");
        }
        internal void SageLoops()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.loops()");
        }
        internal void SageLoopEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.loop_edges()");
        }
        internal void SageNumberOfLoops()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.number_of_loops()");
        }
        internal void SageLoopVertices()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.loop_vertices()");
        }
        internal void SageRemoveLoops()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.remove_loops()");
        }
        internal void SageHasMultipleEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.has_multiple_edges()");
        }
        internal void SageAllowsMultipleEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.allows_multiple_edges()");
        }
        internal void SageAllowMultipleEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.allow_multiple_edges()");
        }
        internal void SageMultipleEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.multiple_edges()");
        }
        internal void SageName()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.name()");
        }
        internal void SageIsImmutable()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_immutable()");
        }
        internal void SageWeighted()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.weighted()");
        }
        internal void SageAntisymmetric()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.antisymmetric()");
        }
        internal void SageDensity()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.density()");
        }
        internal void SageOrder()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.order()");
        }
        internal void SageSize()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.size()");
        }
        internal void SageAddVertex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.add_vertex()");
        }
        internal void SageAddVertices()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.add_vertices()");
        }
        internal void SageDeleteVertex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.delete_vertex()");
        }
        internal void SageDeleteVertices()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.delete_vertices()");
        }
        internal void SageHasVertex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.has_vertex()");
        }
        internal void SageRandomVertex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.random_vertex()");
        }
        internal void SageRandomVertexIterator()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.random_vertex_iterator()");
        }
        internal void SageRandomEdge()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.random_edge()");
        }
        internal void SageRandomEdgeIterator()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.random_edge_iterator()");
        }
        internal void SageVertexBoundary()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.vertex_boundary()");
        }
        internal void SageSetVertices()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.set_vertices()");
        }
        internal void SageSetVertex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.set_vertex()");
        }
        internal void SageGetVertex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.get_vertex()");
        }
        internal void SageGetVertices()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.get_vertices()");
        }
        internal void SageVertexIterator()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.vertex_iterator()");
        }
        internal void SageNeighborIterator()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.neighbor_iterator()");
        }
        internal void SageVertices()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.vertices()");
        }
        internal void SageNeighbors()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.neighbors()");
        }
        internal void SageMergeVertices()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.merge_vertices()");
        }
        internal void SageAddEdge()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.add_edge()");
        }
        internal void SageAddEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.add_edges()");
        }
        internal void SageSubdivideEdge()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.subdivide_edge()");
        }
        internal void SageSubdivideEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.subdivide_edges()");
        }
        internal void SageDeleteEdge()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.delete_edge()");
        }
        internal void SageDeleteEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.delete_edges()");
        }
        internal void SageDeleteMultiedge()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.delete_multiedge()");
        }
        internal void SageSetEdgeLabel()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.set_edge_label()");
        }
        internal void SageHasEdge()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.has_edge()");
        }
        internal void SageEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edges()");
        }
        internal void SageEdgeBoundary()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edge_boundary()");
        }
        internal void SageEdgeIterator()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edge_iterator()");
        }
        internal void SageEdgesIncident()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edges_incident()");
        }
        internal void SageEdgeLabel()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edge_label()");
        }
        internal void SageEdgeLabels()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edge_labels()");
        }
        internal void SageRemoveMultipleEdges()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.remove_multiple_edges()");
        }
        internal void SageClear()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clear()");
        }
        internal void SageDegree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.degree()");
        }
        internal void SageAverageDegree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.average_degree()");
        }
        internal void SageDegreeHistogram()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.degree_histogram()");
        }
        internal void SageDegreeIterator()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.degree_iterator()");
        }
        internal void SageDegreeSequence()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.degree_sequence()");
        }
        internal void SageRandomSubgraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.random_subgraph()");
        }
        internal void SageAddClique()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.add_clique()");
        }
        internal void SageAddCycle()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.add_cycle()");
        }
        internal void SageAddPath()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.add_path()");
        }
        internal void SageComplement()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.complement()");
        }
        internal void SageLineGraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.line_graph()");
        }
        internal void SageToSimple()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.to_simple()");
        }
        internal void SageDisjointUnion()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.disjoint_union()");
        }
        internal void SageUnion()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.union()");
        }
        internal void SageRelabel()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.relabel()");
        }
        internal void SageDegreeToCell()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.degree_to_cell()");
        }
        internal void SageSubgraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.subgraph()");
        }
        internal void SageIsSubgraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_subgraph()");
        }
        internal void SageCartesianProduct()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cartesian_product()");
        }
        internal void SageTensorProduct()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.tensor_product()");
        }
        internal void SageLexicographicProduct()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.lexicographic_product()");
        }
        internal void SageStrongProduct()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.strong_product()");
        }
        internal void SageDisjunctiveProduct()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.disjunctive_product()");
        }
        internal void SageEulerianOrientation()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.eulerian_orientation()");
        }
        internal void SageEulerianCircuit()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.eulerian_circuit()");
        }
        internal void SageCycleBasis()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cycle_basis()");
        }
        internal void SageAllPaths()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.all_paths()");
        }
        internal void SageTrianglesCount()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.triangles_count()");
        }
        internal void SageSpectrum()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.spectrum()");
        }
        internal void SageEigenvectors()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.eigenvectors()");
        }
        internal void SageEigenspaces()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.eigenspaces()");
        }
        internal void SageClusterTriangles()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cluster_triangles()");
        }
        internal void SageClusteringAverage()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clustering_average()");
        }
        internal void SageClusteringCoeff()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clustering_coeff()");
        }
        internal void SageClusterTransitivity()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cluster_transitivity()");
        }
        internal void SageSzegedIndex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.szeged_index()");
        }
        internal void SageCoarsestEquitableRefinement()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.coarsest_equitable_refinement()");
        }
        internal void SageAutomorphismGroup()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.automorphism_group()");
        }
        internal void SageIsVertexTransitive()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_vertex_transitive()");
        }
        internal void SageIsIsomorphic()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_isomorphic()");
        }
        internal void SageCanonicalLabel()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.canonical_label().plot()");
        }
        internal void SageIsCayley()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_cayley()");
        }
        internal void SageIsEulerian()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_eulerian()");
        }
        internal void SageIsPlanar()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_planar()");
        }
        internal void SageIsCircularPlanar()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_circular_planar()");
        }
        internal void SageIsRegular()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_regular()");
        }
        internal void SageIsChordal()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_chordal()");
        }
        internal void SageIsCirculant()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_circulant()");
        }
        internal void SageIsInterval()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_interval()");
        }
        internal void SageIsGallaiTree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_gallai_tree()");
        }
        internal void SageIsClique()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_clique()");
        }
        internal void SageIsCycle()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_cycle()");
        }
        internal void SageIsIndependentSet()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_independent_set()");
        }
        internal void SageIsTransitivelyReduced()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_transitively_reduced()");
        }
        internal void SageIsEquitable()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_equitable()");
        }
        internal void SageBreadthFirstSearch()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.breadth_first_search()");
        }
        internal void SageDepthFirstSearch()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.depth_first_search()");
        }
        internal void SageLexBFS()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.lex_BFS()");
        }
        internal void SageCentralityBetweenness()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.centrality_betweenness()");
        }
        internal void SageCentralityCloseness()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.centrality_closeness()");
        }
        internal void SageDistance()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.distance()");
        }
        internal void SageDistanceAllPairs()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.distance_all_pairs()");
        }
        internal void SageDistancesDistribution()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.distances_distribution()");
        }
        internal void SageEccentricity()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.eccentricity()");
        }
        internal void SageRadius()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.radius()");
        }
        internal void SageCenter()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.center()");
        }
        internal void SageDiameter()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.diameter()");
        }
        internal void SageDistanceGraph()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.distance_graph()");
        }
        internal void SageGirth()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.girth()");
        }
        internal void SagePeriphery()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.periphery()");
        }
        internal void SageShortestPath()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.shortest_path()");
        }
        internal void SageShortestPathLength()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.shortest_path_length()");
        }
        internal void SageShortestPaths()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.shortest_paths()");
        }
        internal void SageShortestPathLengths()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.shortest_path_lengths()");
        }
        internal void SageShortestPathAllPairs()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.shortest_path_all_pairs()");
        }
        internal void SageWienerIndex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.wiener_index()");
        }
        internal void SageAverageDistance()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.average_distance()");
        }
        internal void SageIsConnected()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_connected()");
        }
        internal void SageConnectedComponents()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.connected_components()");
        }
        internal void SageConnectedComponentsNumber()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.connected_components_number()");
        }
        internal void SageConnectedComponentsSubgraphs()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.connected_components_subgraphs()");
        }
        internal void SageConnectedComponentContainingVertex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.connected_component_containing_vertex()");
        }
        internal void SageConnectedComponentsSizes()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.connected_components_sizes()");
        }
        internal void SageBlocksAndCutVertices()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.blocks_and_cut_vertices()");
        }
        internal void SageBlocksAndCutsTree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.blocks_and_cuts_tree()");
        }
        internal void SageIsCutEdge()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_cut_edge()");
        }
        internal void SageIsCutVertex()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_cut_vertex()");
        }
        internal void SageEdgeCut()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edge_cut()");
        }
        internal void SageVertexCut()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.vertex_cut()");
        }
        internal void SageFlow()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.flow()");
        }
        internal void SageEdgeDisjointPaths()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edge_disjoint_paths()");
        }
        internal void SageVertexDisjointPaths()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.vertex_disjoint_paths()");
        }
        internal void SageEdgeConnectivity()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edge_connectivity()");
        }
        internal void SageVertexConnectivity()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.vertex_connectivity()");
        }
        internal void SageTransitiveClosure()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.transitive_closure()");
        }
        internal void SageTransitiveReduction()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.transitive_reduction()");
        }
        internal void SageMinSpanningTree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.min_spanning_tree()");
        }
        internal void SageSpanningTreesCount()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.spanning_trees_count()");
        }
        internal void SageDominatorTree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.dominator_tree()");
        }
        internal void SageSetEmbedding()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.set_embedding()");
        }
        internal void SageGetEmbedding()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.get_embedding()");
        }
        internal void SageFaces()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.faces()");
        }
        internal void SageGetPos()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.get_pos()");
        }
        internal void SageSetPos()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.set_pos()");
        }
        internal void SageSetPlanarPositions()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.set_planar_positions()");
        }
       
        internal void SageIsDrawnFreeOfEdgeCrossings()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_drawn_free_of_edge_crossings()");
        }
        internal void SageLatexOptions()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.latex_options()");
        }
        internal void SageSetLatexOptions()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.set_latex_options()");
        }
        internal void SageLayout()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.layout()");
        }
       
        internal void SageGraphplot()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.graphplot()");
        }
        internal void SagePlot()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.plot()");
        }
        internal void SageShow()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.show()");
        }
        internal void SagePlot3d()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.plot3d()");
        }
        internal void SageShow3d()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.show3d()");
        }
        internal void SageGraphvizString()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.graphviz_string()");
        }
        internal void SageGraphvizToFileNamed()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.graphviz_to_file_named()");
        }
        internal void SageSteinerTree()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.steiner_tree()");
        }
        internal void SageEdgeDisjointSpanningTrees()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.edge_disjoint_spanning_trees()");
        }
        internal void SageFeedbackVertexSet()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.feedback_vertex_set()");
        }
        internal void SageMultiwayCut()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.multiway_cut()");
        }
        internal void SageMaxCut()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.max_cut()");
        }
        internal void SageLongestPath()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.longest_path()");
        }
        internal void SageTravelingSalesmanProblem()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.traveling_salesman_problem()");
        }
        internal void SageIsHamiltonian()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_hamiltonian()");
        }
        internal void SageHamiltonianCycle()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.hamiltonian_cycle()");
        }
        internal void SageHamiltonianPath()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.hamiltonian_path()");
        }
        internal void SageMulticommodityFlow()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.multicommodity_flow()");
        }
        internal void SageDisjointRoutedPaths()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.disjoint_routed_paths()");
        }
        internal void SageDominatingSet()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.dominating_set()");
        }
        internal void SageSubgraphSearch()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.subgraph_search()");
        }
        internal void SageSubgraphSearchCount()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.subgraph_search_count()");
        }
        internal void SageSubgraphSearchIterator()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.subgraph_search_iterator()");
        }
        internal void SageCharacteristicPolynomial()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.characteristic_polynomial()");
        }
        internal void SageGenus()
        {
            App.AskSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.genus()");
        }

        #region Layout
        internal async void SageLayoutSpring()
        {
            var layout = await App.AskSageAsync(GraphCanvas.Graph, "G.layout_spring()");
            var positions = Scale(ExtractPoints(layout));
            await LayoutGraph(positions);
        }

        internal async void SageLayoutRanked()
        {
            var layout = await App.AskSageAsync(GraphCanvas.Graph, "G.layout_ranked()");
            var positions = Scale(ExtractPoints(layout));
            await LayoutGraph(positions);
        }
        internal async void SageLayoutExtendRandomly()
        {
            var layout = await App.AskSageAsync(GraphCanvas.Graph, "G.layout_extend_randomly()");
            var positions = Scale(ExtractPoints(layout));
            await LayoutGraph(positions);
        }
        internal async void SageLayoutCircular()
        {
            var layout = await App.AskSageAsync(GraphCanvas.Graph, "G.layout_circular()");
            var positions = Scale(ExtractPoints(layout));
            await LayoutGraph(positions);
        }
        internal async void SageLayoutTree()
        {
            var layout = await App.AskSageAsync(GraphCanvas.Graph, "G.layout_tree()");
            var positions = Scale(ExtractPoints(layout));
            await LayoutGraph(positions);
        }
        internal async void SageLayoutGraphviz()
        {
            var layout = await App.AskSageAsync(GraphCanvas.Graph, "G.layout_graphviz()");
            var positions = Scale(ExtractPoints(layout));
            await LayoutGraph(positions);
        }

        internal async void SageLayoutPlanar()
        {
            var layout = await App.AskSageAsync(GraphCanvas.Graph, "G.layout_planar()");
            var positions = Scale(ExtractPoints(layout));
            await LayoutGraph(positions);
        }
        #endregion

        static internal async void SageLoadNamedGraph(string name)
        {
            var a = await App.AskSageAsync("G = graphs." + name + "()" + Environment.NewLine + "G.adjacency_matrix().str()");
            var layout = await App.AskSageAsync("G = graphs." + name + "()" + Environment.NewLine + "G.layout()");
            var positions = Scale(ExtractPoints(layout));
            var adj = ParseAdjacencyMatrix(a);

            var g = new Algorithms.Graph(adj);

            App.NewTab(name, new Graphs.Graph(g, positions, false));
        }
        #endregion

        static List<Vector> Scale(IEnumerable<Vector> positions)
        {
            var l = positions.ToList();
            var minX = l.Min(p => p.X);
            var minY = l.Min(p => p.Y);
            var maxX = l.Max(p => p.X);
            var maxY = l.Max(p => p.Y);

            var w = maxX - minX;
            var h = maxY - minY;

            return positions.Select(p => new Vector(0.1 + 0.8 * (p.X - minX) / w, 0.1 + 0.8 * (p.Y - minY) / h)).ToList();
        }

        async Task LayoutGraph(List<Vector> positions)
        {
            SnapPositionsToGrid(positions);
            var a = new LayoutAnimation(() => {
                GraphCanvas.Invalidate();
                
            }, () => GraphCanvas.Invalidate(), positions, GraphCanvas.Graph);

            GraphCanvas.SnapToGrid = false;
            await a.Animate();
            GraphCanvas.SnapToGrid = true;
        }

        void SnapPositionsToGrid(List<Vector> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                positions[i].X = (float)(GraphCanvas._gridStep * Math.Round(positions[i].X / GraphCanvas._gridStep));
                positions[i].Y = (float)(GraphCanvas._gridStep * Math.Round(positions[i].Y / GraphCanvas._gridStep));
            }
        }

        void OnGraphModified(Graphs.Graph g)
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
                MouseMoved(e.LayerX, e.LayerY);
        }

        void OnMouseButtonDown(MouseEvent<HTMLCanvasElement> e)
        {
            _ctrlDown = e.CtrlKey;
           // Canvas.SetCapture(true);  // chrome does not seem to support this, what?

            if (e.ShiftKey)
            {
                if (MouseButtonDoubleClicked != null)
                    MouseButtonDoubleClicked(e.LayerX, e.LayerY, e.Button == 0 ? MouseButton.Left : MouseButton.Right);
            }
            else
            {
                if (MouseButtonDown != null)
                    MouseButtonDown(e.LayerX, e.LayerY, e.Button == 0 ? MouseButton.Left : MouseButton.Right);
            }
        }

        void OnMouseButtonUp(MouseEvent<HTMLCanvasElement> e)
        {
            _ctrlDown = e.CtrlKey;
           // Canvas.SetCapture(false);
            if (e.ShiftKey)
                return;


            if (MouseButtonUp != null)
                MouseButtonUp(e.LayerX, e.LayerY, e.Button == 0 ? MouseButton.Left : MouseButton.Right);

            if (e.CtrlKey && e.AltKey)
            {
                App.AskSage("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')");
            }
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

        static IEnumerable<Vector> ExtractPoints(string x)
        {
            var i = 0;
            while (true)
            {
                var j = x.IndexOf("[", i);
                if (j < 0)
                {
                    j = x.IndexOf("(", i);
                    if (j < 0)
                        break;
                }
                i = j;

                j = x.IndexOf("]", i);
                if (j < 0)
                {
                    j = x.IndexOf(")", i);
                    if (j < 0)
                        break;
                }
                var p = x.Substring(i, j - i).Split(',');
                yield return new Vector(double.Parse(p[0].Trim('[', ']', '(', ')')), double.Parse(p[1].Trim('[', ']', '(', ')')));
                i = j;
            }
        }

        static bool[,] ParseAdjacencyMatrix(string a)
        {
            var bits = a.Replace("'", "").Split(new[] { ",", "\\\\n", "[", "]", " " }, StringSplitOptions.RemoveEmptyEntries);
            var n = (int)Math.Sqrt(bits.Length);
            var adj = new bool[n, n];

            var i = 0;
            foreach (var b in bits)
            {
                adj[i / n, i % n] = b == "1";
                i++;
            }

            return adj;
        }
    }
}

using Bridge.Html5;
using GraphicsLayer;
using Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphsCore;

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

        #region Sage
        internal void SageManual()
        {
            App.TellSage("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine);
        }
        internal void SageChromaticPolynomial()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.chromatic_polynomial()");
        }
        internal void SageGraph6()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.graph6_string()");
        }
        internal void SageSparse6()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.sparse6_string()");
        }
        internal void SageChromaticNumber()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.chromatic_number()");
        }
        internal void SageChromaticQuasisymmetricFunction()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.chromatic_quasisymmetric_function()");
        }
        internal void SageChromaticSymmetricFunction()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.chromatic_symmetric_function()");
        }
        internal void SageColoring()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.coloring()");
        }
        internal void SageConvexityProperties()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.convexity_properties()");
        }
        internal void SageHasHomomorphismTo()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.has_homomorphism_to()");
        }
        internal void SageIndependentSet()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.independent_set()");
        }
        internal void SageIndependentSetOfRepresentatives()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.independent_set_of_representatives()");
        }
        internal void SageIsPerfect()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_perfect()");
        }
        internal void SageMatchingPolynomial()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.matching_polynomial()");
        }
        internal void SageMinor()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.minor()");
        }
        internal void SagePathwidth()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.pathwidth()");
        }
        internal void SageRankDecomposition()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.rank_decomposition()");
        }
        internal void SageTopologicalMinor()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.topological_minor()");
        }
        internal void SageTreewidth()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.treewidth()");
        }
        internal void SageTuttePolynomial()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.tutte_polynomial()");
        }
        internal void SageVertexCover()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.vertex_cover()");
        }
        internal void SageBipartiteColor()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.bipartite_color()");
        }
        internal void SageBipartiteSets()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.bipartite_sets()");
        }
        internal void SageGraph6String()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.graph6_string()");
        }
        internal void SageIsDirected()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_directed()");
        }
        internal void SageJoin()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.join()");
        }
        internal void SageSparse6String()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.sparse6_string()");
        }
        internal void SageToDirected()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.to_directed()");
        }
        internal void SageToUndirected()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.to_undirected()");
        }
        internal void SageWriteToEps()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.write_to_eps()");
        }
        internal void SageCliqueComplex()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clique_complex()");
        }
        internal void SageCliqueMaximum()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clique_maximum()");
        }
        internal void SageCliqueNumber()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clique_number()");
        }
        internal void SageCliquePolynomial()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.clique_polynomial()");
        }
        internal void SageCliquesContainingVertex()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_containing_vertex()");
        }
        internal void SageCliquesGetCliqueBipartite()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_get_clique_bipartite()");
        }
        internal void SageCliquesGetMaxCliqueGraph()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_get_max_clique_graph()");
        }
        internal void SageCliquesMaximal()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_maximal()");
        }
        internal void SageCliquesMaximum()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_maximum()");
        }
        internal void SageCliquesNumberOf()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_number_of()");
        }
        internal void SageCliquesVertexCliqueNumber()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cliques_vertex_clique_number()");
        }
        internal void SageBoundedOutdegreeOrientation()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.bounded_outdegree_orientation()");
        }
        internal void SageBridges()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.bridges()");
        }
        internal void SageDegreeConstrainedSubgraph()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.degree_constrained_subgraph()");
        }
        internal void SageGomoryHuTree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.gomory_hu_tree()");
        }
        internal void SageMinimumOutdegreeOrientation()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.minimum_outdegree_orientation()");
        }
        internal void SageOrientations()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.orientations()");
        }
        internal void SageRandomSpanningTree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.random_spanning_tree()");
        }
        internal void SageSpanningTrees()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.spanning_trees()");
        }
        internal void SageStrongOrientation()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.strong_orientation()");
        }
        internal void SageApexVertices()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.apex_vertices()");
        }
        internal void SageIsApex()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_apex()");
        }
        internal void SageIsArcTransitive()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_arc_transitive()");
        }
        internal void SageIsAsteroidalTripleFree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_asteroidal_triple_free()");
        }
        internal void SageIsBiconnected()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_biconnected()");
        }
        internal void SageIsBipartite()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_bipartite()");
        }
        internal void SageIsBlockGraph()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_block_graph()");
        }
        internal void SageIsCartesianProduct()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_cartesian_product()");
        }
        internal void SageIsDistanceRegular()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_distance_regular()");
        }
        internal void SageIsEdgeTransitive()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_edge_transitive()");
        }
        internal void SageIsEvenHoleFree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_even_hole_free()");
        }
        internal void SageIsForest()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_forest()");
        }
        internal void SageIsHalfTransitive()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_half_transitive()");
        }
        internal void SageIsLineGraph()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_line_graph()");
        }
        internal void SageIsLongAntiholeFree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_long_antihole_free()");
        }
        internal void SageIsLongHoleFree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_long_hole_free()");
        }
        internal void SageIsOddHoleFree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_odd_hole_free()");
        }
        internal void SageIsOverfull()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_overfull()");
        }
        internal void SageIsPartialCube()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_partial_cube()");
        }
        internal void SageIsPrime()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_prime()");
        }
        internal void SageIsSemiSymmetric()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_semi_symmetric()");
        }
        internal void SageIsSplit()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_split()");
        }
        internal void SageIsStronglyRegular()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_strongly_regular()");
        }
        internal void SageIsTree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_tree()");
        }
        internal void SageIsTriangleFree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_triangle_free()");
        }
        internal void SageIsWeaklyChordal()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.is_weakly_chordal()");
        }
        internal void SageOddGirth()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.odd_girth()");
        }
        internal void SageCores()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.cores()");
        }
        internal void SageFractionalChromaticIndex()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.fractional_chromatic_index()");
        }
        internal void SageHasPerfectMatching()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.has_perfect_matching()");
        }
        internal void SageIharaZetaFunctionInverse()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.ihara_zeta_function_inverse()");
        }
        internal void SageKirchhoffSymanzikPolynomial()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.kirchhoff_symanzik_polynomial()");
        }
        internal void SageLovaszTheta()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.lovasz_theta()");
        }
        internal void SageMagnitudeFunction()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.magnitude_function()");
        }
        internal void SageMatching()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.matching()");
        }
        internal void SageMaximumAverageDegree()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.maximum_average_degree()");
        }
        internal void SageModularDecomposition()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.modular_decomposition()");
        }
        internal void SagePerfectMatchings()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.perfect_matchings()");
        }
        internal void SageSeidelAdjacencyMatrix()
        {
            App.TellSageAuto("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')" + Environment.NewLine + "G.seidel_adjacency_matrix()");
        } 
        #endregion

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
                App.TellSage("G = Graph('" + GraphCanvas.Graph.GetEdgeWeights().ToGraph6() + "')");
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
    }
}

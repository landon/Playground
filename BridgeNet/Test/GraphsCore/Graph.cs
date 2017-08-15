using System;
using System.Collections.Generic;
using System.Linq;

namespace Graphs
{
    public class Graph : GraphicsLayer.IPaintable
    {
        public const string GraphExtension = ".graph";
        public const string EdgeWeightExtension = ".edge weight";

        List<Vertex> _vertices = new List<Vertex>();
        List<Edge> _edges = new List<Edge>();
        object _ModifyListsToken = new object();

        string _FileName = null;

        public bool IsDirected
        {
            get { return _edges.Any(e => e.Orientation != Edge.Orientations.None); }
        }

        public List<IHittable> SelectedItems
        {
            get
            {
                lock (_ModifyListsToken)
                {
                    var selectedItems = new List<IHittable>();
                    foreach (var i in SelectedVertices)
                        selectedItems.Add(i);
                    foreach (var i in SelectedEdges)
                        selectedItems.Add(i);

                    return selectedItems;
                }
            }
        }

        public List<Vertex> SelectedVertices
        {
            get
            {
                lock (_ModifyListsToken)
                    return _vertices.Where(v => v.IsSelected).ToList();
            }
        }

        public List<Edge> SelectedEdges
        {
            get
            {
                lock (_ModifyListsToken)
                    return _edges.Where(e => e.IsSelected).ToList();
            }
        }

        public GraphicsLayer.Box BoundingRectangle
        {
            get
            {
                lock (_ModifyListsToken)
                {
                    var left = double.MaxValue;
                    var right = double.MinValue;
                    var top = double.MaxValue;
                    var bottom = double.MinValue;

                    foreach (var v in _vertices)
                    {
                        left = Math.Min(left, v.LocalBounds.Left);
                        right = Math.Max(right, v.LocalBounds.Right);
                        top = Math.Min(top, v.LocalBounds.Top);
                        bottom = Math.Max(bottom, v.LocalBounds.Bottom);
                    }

                    return new GraphicsLayer.Box(left, top, right - left, bottom - top);
                }
            }
        }

        public GraphicsLayer.Box SelectedBoundingRectangle
        {
            get
            {
                var sv = SelectedVertices.ToList();
                if (sv.Count <= 0)
                    return BoundingRectangle;

                lock (_ModifyListsToken)
                {
                    var left = double.MaxValue;
                    var right = double.MinValue;
                    var top = double.MaxValue;
                    var bottom = double.MinValue;

                    foreach (var v in sv)
                    {
                        left = Math.Min(left, v.LocalBounds.Left);
                        right = Math.Max(right, v.LocalBounds.Right);
                        top = Math.Min(top, v.LocalBounds.Top);
                        bottom = Math.Max(bottom, v.LocalBounds.Bottom);
                    }

                    return new GraphicsLayer.Box(left, top, right - left, bottom - top);
                }
            }
        }


        public string Name
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }

        public List<Vertex> Vertices
        {
            get
            {
                return _vertices;
            }
        }

        public List<Edge> Edges
        {
            get
            {
                return _edges;
            }
        }

        public bool ParametersDirty { get; set; }

        public Graph()
        {
            ParametersDirty = true;
        }

        public Graph(SerializationGraph g)
            : this()
        {
            _vertices = g.Vertices.Select(v => new Vertex(v)).ToList();
            _edges = g.Edges.Select(e => new Edge(e, _vertices)).ToList();
            Name = g.Name;
        }

        public Graph(IEnumerable<Vertex> vertices, IEnumerable<Edge> edges)
        {
            _vertices = vertices.ToList();
            _edges = edges.ToList();
        }

        public Graph(Algorithms.Graph g, List<Vector> position, bool directed = true)
            : this()
        {
            if (g.VertexWeight != null && g.VertexWeight.Count == g.N)
            {
                _vertices = g.Vertices.Select(v => new Vertex(position[v].X, position[v].Y, g.VertexWeight[v].ToString())).ToList();
            }
            else
            {
                _vertices = g.Vertices.Select(v => new Vertex(position[v].X, position[v].Y)).ToList();
            }
            _edges = new List<Edge>();

            for (int i = 0; i < g.N; i++)
            {
                for (int j = i + 1; j < g.N; j++)
                {
                    if (directed)
                    {
                        if (g.Directed[i, j])
                            _edges.Add(new Edge(_vertices[i], _vertices[j], Edge.Orientations.Forward));
                        else if (g.Adjacent[i, j])
                            _edges.Add(new Edge(_vertices[j], _vertices[i], Edge.Orientations.Forward));
                    }
                    else if (g.Adjacent[i, j])
                    {
                        _edges.Add(new Edge(_vertices[i], _vertices[j], Edge.Orientations.None));
                    }
                }
            }
        }

        public void Paint(GraphicsLayer.IGraphics g, int width, int height)
        {
            lock (_ModifyListsToken)
            {
                if (ParametersDirty)
                {
                    var graph = new Algorithms.Graph(GetEdgeWeights());
                    var n = Vertices.Count;

                    for (int i = 0; i < Vertices.Count; i++)
                        Vertices[i].IsUniversal = graph.Degree(i) == n - 1;

                    ParametersDirty = false;
                }

                var jj = 0;
                foreach (var v in _vertices)
                {
                    v.ParentIndex = jj++;
                    v.Paint(g, width, height);
                }

                jj = 0;
                foreach (var e in _edges)
                {
                    e.ParentIndex = jj++;
                    e.Paint(g, width, height);
                }
            }
        }

        public List<int> GetEdgeWeights()
        {
            var weights = new List<int>();

            for (int i = 0; i < _vertices.Count; i++)
            {
                for (int j = i + 1; j < _vertices.Count; j++)
                {
                    Edge e;
                    if (EdgeExists(_vertices[i], _vertices[j], out e))
                        weights.Add(1);
                    else if (EdgeExists(_vertices[j], _vertices[i], out e))
                    {
                        if (e.Orientation == Edge.Orientations.None)
                            weights.Add(1);
                        else
                            weights.Add(-1);
                    }
                    else
                        weights.Add(0);
                }
            }

            return weights;
        }

        public void ModifyOrientation(List<int> edgeWeights)
        {
            int k = 0;
            for (int i = 0; i < _vertices.Count; i++)
            {
                for (int j = i + 1; j < _vertices.Count; j++)
                {
                    var w = edgeWeights[k];

                    if (w != 0)
                    {
                        var e = GetEdge(_vertices[i], _vertices[j]);

                        if (e.V1 == _vertices[i] && e.V2 == _vertices[j])
                        {
                            if (w < 0)
                                e.Orientation = Edge.Orientations.Backward;
                            else
                                e.Orientation = Edge.Orientations.Forward;
                        }
                        else
                        {
                            if (w < 0)
                                e.Orientation = Edge.Orientations.Forward;
                            else
                                e.Orientation = Edge.Orientations.Backward;
                        }
                    }

                    k++;
                }
            }

            ParametersDirty = true;
        }
        
        public Graph Clone()
        {
            var s = GraphsCore.CompactSerializer.Serialize(this);
            var g = GraphsCore.CompactSerializer.Deserialize(s);
            if (g != null)
                g.ParametersDirty = true;
            else
                Console.WriteLine("clone is null");
            return g;
        }

        public IHittable HitTest(double x, double y)
        {
            foreach (var v in _vertices)
            {
                if (v.Hit(x, y))
                    return v;
            }

            foreach (var e in _edges)
            {
                if (e.Hit(x, y))
                    return e;
            }

            return null;
        }

        public void UnselectAll()
        {
            UnselectAllVertices();
            UnselectAllEdges();
        }

        public void UnselectAllVertices()
        {
            foreach (var v in _vertices)
                v.IsSelected = false;
        }

        public void UnselectAllEdges()
        {
            foreach (var e in _edges)
                e.IsSelected = false;
        }

        public void SelectEdges(IEnumerable<Edge> edges, bool symmetricDifference)
        {
            UnselectAllVertices();
            if (!symmetricDifference)
                UnselectAllEdges();

            foreach (var e in edges)
                e.IsSelected = symmetricDifference ? !e.IsSelected : true;
        }

        public void SelectVertices(IEnumerable<Vertex> vertices, bool symmetricDifference)
        {
            UnselectAllEdges();
            if (!symmetricDifference)
                UnselectAllVertices();

            foreach (var v in vertices)
                v.IsSelected = symmetricDifference ? !v.IsSelected : true;
        }
        public void SelectObjects(List<GraphicsLayer.Box> selectionPoints, bool symmetricDifference)
        {
            try
            {
                var bounds = new PolygonContainer(selectionPoints.ToArray());

                var verticesInPolygon = _vertices.Where(v => bounds.Contains(new GraphicsLayer.Box(v.X, v.Y))).ToList();

                if (verticesInPolygon.Count > 0)
                {
                    SelectVertices(verticesInPolygon, symmetricDifference);
                }
                else
                {
                    var edgesHittingPolygon = _edges.Where(e =>
                    {
                        for (int i = 0; i < selectionPoints.Count; i++)
                        {
                            var start = selectionPoints[i];
                            var end = selectionPoints[(i + 1) % selectionPoints.Count];

                            if (Utility.HaveIntersection(start, end, e.V1.Location, e.V2.Location))
                                return true;
                        }

                        return false;
                    }).ToList();

                    SelectEdges(edgesHittingPolygon, symmetricDifference);
                }
            }
            catch { }
        }
        public bool AddVertex(Vertex v)
        {
            if (_showVertexIndices)
                v.ToggleIndex();
            lock (_ModifyListsToken)
                _vertices.Add(v);

            ParametersDirty = true;
            return true;
        }
        public bool RemoveVertex(Vertex v)
        {
            var neighbors = FindNeighbors(v);
            foreach (var x in neighbors)
            {
                RemoveEdge(v, x);
                RemoveEdge(x, v);
            }

            lock (_ModifyListsToken)
                _vertices.Remove(v);

            ParametersDirty = neighbors.Count > 0;

            return neighbors.Count > 0;
        }
        public bool AddEdge(Vertex v1, Vertex v2, int multiplicity = 1)
        {
            return AddEdge(v1, v2, Edge.Orientations.None, multiplicity);
        }
        public bool AddEdge(Vertex v1, Vertex v2, Edge.Orientations orientation, int multiplicity = 1, float thickness = 3, string style = "", string label = "")
        {
            if (v1 == v2)
            {
                // Not allowing self-loops for now.
                return false;
            }

            if (!EdgeExists(v1, v2) && !EdgeExists(v2, v1))
            {
                lock (_ModifyListsToken)
                {
                    var edge = new Edge(v1, v2, orientation);
                    edge.Multiplicity = multiplicity;
                    edge.Thickness = thickness;
                    edge.Style = style;
                    edge.Label = label;
                    if (_showEdgeIndices)
                        edge.ToggleIndex();
                    _edges.Add(edge);
                    ParametersDirty = true;

                    return true;
                }
            }

            return false;
        }

        public bool RemoveEdge(Vertex v1, Vertex v2)
        {
            List<Edge> toRemove;

            lock (_ModifyListsToken)
            {
                toRemove = _edges.Where(e => e.V1 == v1 && e.V2 == v2).ToList();
            }

            foreach (var e in toRemove)
                _edges.Remove(e);

            ParametersDirty = toRemove.Count > 0;
            return toRemove.Count > 0;
        }
        public bool EdgeExists(Vertex v1, Vertex v2)
        {
            Edge edge;
            return EdgeExists(v1, v2, out edge);
        }
        public bool EdgeExists(Vertex v1, Vertex v2, out Edge edge)
        {
            edge = null;

            lock (_ModifyListsToken)
            {
                foreach (var e in _edges)
                {
                    edge = e;
                    if (e.V1 == v1 && e.V2 == v2 && (e.Orientation == Edge.Orientations.None || e.Orientation == Edge.Orientations.Forward))
                        return true;

                    if (e.V1 == v2 && e.V2 == v1 && e.Orientation == Edge.Orientations.Backward)
                        return true;
                }
            }

            return false;
        }

        public Edge GetEdge(Vertex v1, Vertex v2)
        {
            lock (_ModifyListsToken)
            {
                foreach (var e in _edges)
                {
                    if (e.V1 == v1 && e.V2 == v2 ||
                        e.V1 == v2 && e.V2 == v1)
                    {
                        return e;
                    }
                }
            }

            return null;
        }

        public Graph InducedSubgraph(List<Vertex> vertices)
        {
            var induced = new Graph();
            foreach (var v in vertices)
                induced.AddVertex(v);

            foreach (var v1 in induced.Vertices)
            {
                foreach (var v2 in induced.Vertices)
                {
                    Edge e;
                    if (EdgeExists(v1, v2, out e))
                        induced.AddEdge(v1, v2, e.Orientation != Edge.Orientations.None ? Edge.Orientations.Forward : Edge.Orientations.None, e.Multiplicity, e.Thickness, e.Style, e.Label);
                }
            }

            return induced.Clone();
        }

        public void DisjointUnion(Graph g)
        {
            foreach (var v in g.Vertices)
                AddVertex(v);

            foreach (var e in g.Edges)
                AddEdge(e);
        }

        void AddEdge(Edge e)
        {
            AddEdge(e.V1, e.V2, e.Orientation, Math.Max(1, e.Multiplicity), e.Thickness, e.Style, e.Label);
        }

        public List<Vertex> FindNeighbors(Vertex v)
        {
            var neighbors = new List<Vertex>();

            lock (_ModifyListsToken)
            {
                foreach (var e in _edges)
                {
                    if (e.V1 == v)
                        neighbors.Add(e.V2);
                    if (e.V2 == v)
                        neighbors.Add(e.V1);
                }
            }

            return neighbors;
        }

        public void Translate(Vector offset)
        {
            lock (_ModifyListsToken)
            {
                foreach (var v in _vertices)
                {
                    v.X += offset.X;
                    v.Y += offset.Y;
                }
            }
        }

        bool _showVertexIndices;
        bool _showEdgeIndices;
        public void ToggleEdgeIndices()
        {
            _showEdgeIndices = !_showEdgeIndices;
            foreach (var a in Edges)
                a.ToggleIndex();
        }

        public void ToggleVertexIndices()
        {
            _showVertexIndices = !_showVertexIndices;
            foreach (var a in Vertices)
                a.ToggleIndex();
        }
    }
}

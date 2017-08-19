using GraphicsLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GraphsCore;

namespace Graphs
{
    public class GraphCanvas : GraphicsLayer.IPaintable
    {
        public event Action<Graph> GraphModified;
        public event Action<string> NameModified;

        const int DefaultBaseViewScale = 800;
        public int BaseViewScale = DefaultBaseViewScale;
        const int MaxZoomScale = 6000;
        double ZoomDelta = 0.15;
        double MinZoom = 0.15;

        public void SetZoomDelta(double d)
        {
            ZoomDelta = d;
            MinZoom = d;
        }

        bool _zoomFitNextPaint;
        bool _snapToGrid = true;
        public static double _gridStep = 0.04;
        bool _drawGrid = true;
        public bool SnapToGrid { get { return _snapToGrid; } set { var needRedraw = value && !_snapToGrid; _snapToGrid = value; if (needRedraw) Invalidate(); } }
        public double GridStep { get { return _gridStep; } set { var needRedraw = SnapToGrid && value != _gridStep; _gridStep = value; if (needRedraw) Invalidate(); } }
        public bool DrawGrid { get { return _drawGrid; } set { var needRedraw = _drawGrid != value; _drawGrid = value; if (needRedraw) Invalidate(); } }
        public bool HasPainted { get; private set; }

        Graph _graph;

        int Width { get; set; }
        int Height { get; set; }

        #region dragging vertex variables
        Vertex _DraggedVertex = null;
        #endregion

        #region dragging selection variables
        List<Box> _selectionPoints = new List<Box>();
        object _SelectionPointsToken = new object();
        bool _controlWasDown;
        #endregion

        #region children items
        public Graph Graph { get { return _graph; } }
        ICanvas _canvas;
        public ICanvas Canvas 
        {
            get { return _canvas; }
            set
            {
                _canvas = value;

                _canvas.MouseMoved += OnMouseMove;
                _canvas.MouseButtonDown += OnMouseDown;
                _canvas.MouseButtonUp += OnMouseUp;
                _canvas.MouseButtonDoubleClicked += OnMouseDoubleClick;
            }
        }
        #endregion

        double Zoom
        {
            set
            {
                if (value == _Zoom)
                    return;

                _Zoom = value;
                var scale = (int)(_Zoom * BaseViewScale);
                if (scale > MaxZoomScale)
                {
                    scale = MaxZoomScale;
                    _Zoom = scale / BaseViewScale;
                }

                ViewScale = scale;
            }
        }
        public int ViewScale
        {
            get
            {
                return _viewScale;
            }
            protected set
            {
                if (value == _viewScale)
                    return;

                _viewScale = value;
            }
        }
        public bool IsEmpty { get { return _graph.Vertices.Count <= 0; } }

        States _state = States.Idle;
        int _viewScale = DefaultBaseViewScale;
        double _Zoom = 1.0;
        Func<string, Graph> _secondaryConverter;

        List<HistoricalGraph> _history = new List<HistoricalGraph>();
        int _historyIndex = 0;

        static readonly ARGB SelectionPenColor = new ARGB(0, 0, 255);
        static readonly ARGB GridPenColor = new ARGB(75, 0, 10, 0);

        enum States
        {
            Idle,
            DraggingVertex,
            DraggingSelectionRegion,
            DraggingSelectedVertices
        }

        static readonly ARGB BackgroundColor = new ARGB(0xf7, 0xf7, 0xf7);

        public GraphCanvas(Graph graph,  Func<string, Graph> secondaryConverter = null)
        {
            _graph = graph != null ? graph : new Graph();
            _secondaryConverter = secondaryConverter;
            _history.Add(new HistoricalGraph() { Graph = _graph.Clone(), Zoom = _Zoom, ViewScale = _viewScale });
        }

        public void Invalidate()
        {
            if (Canvas != null)
                Canvas.Invalidate();
        }

        public void GraphChanged()
        {
            if (_history.Count - _historyIndex - 1 > 0)
                _history.RemoveRange(_historyIndex + 1, _history.Count - _historyIndex - 1);

            _history.Add(new HistoricalGraph() { Graph = _graph.Clone(), Zoom = _Zoom, ViewScale = _viewScale });
            _historyIndex = _history.Count - 1;

            if (GraphModified != null)
                GraphModified(Graph);
        }

        public bool DoUndo()
        {
            if (_historyIndex <= 0)
                return false;

            _historyIndex--;
            ObeyHistory();

            return _historyIndex > 0;
        }
        public bool DoRedo()
        {
            if (_historyIndex >= _history.Count - 1)
                return false;

            _historyIndex++;
            ObeyHistory();

            return _historyIndex < _history.Count - 1;
        }

        void ObeyHistory()
        {
            _graph = _history[_historyIndex].Graph.Clone();
            ViewScale = _history[_historyIndex].ViewScale;
            Zoom = _history[_historyIndex].Zoom;
            _graph.ParametersDirty = true;

            if (GraphModified != null)
                GraphModified(Graph);

            Invalidate();
        }

        #region actions

        public string Save(string name)
        {
            _graph.Name = name;
            return "";// _graph.Serialize();
        }
     
        public void DoZoom(int amount, Box location)
        {
            int width = Width;
            int height = Height;

            var oldCenter = new Vector((double)width / (double)_viewScale * location.X, (double)height / (double)_viewScale * location.Y);

            Zoom = Math.Max(MinZoom, _Zoom + ZoomDelta * amount);

            var center = new Vector((double)width / (double)_viewScale * location.X, (double)height / (double)_viewScale * location.Y);

            _graph.Translate(center - oldCenter);

            Invalidate();
        }

        public void DoZoomFit()
        {
            if (_graph.Vertices.Count <= 0)
            {
                Zoom = 1;
                Invalidate();
                return;
            }

            int width = Width;
            int height = Height;
            var bounds = _graph.SelectedBoundingRectangle;
            var c = new Vector(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);

            var actual = Math.Min(width, height);
            Zoom = 0.9 * actual / Math.Max(bounds.Width, bounds.Height) / BaseViewScale;

            var center = new Vector((double)width / (double)_viewScale * 0.5, (double)height / (double)_viewScale * 0.5);
            _graph.Translate(center - c);

            Invalidate();
        }

        public void DoDelete()
        {
            if (_graph.SelectedVertices.Count > 0)
            {
                foreach (var v in _graph.SelectedVertices)
                    _graph.RemoveVertex(v);
            }

            if (_graph.SelectedEdges.Count > 0)
            {
                foreach (var e in _graph.SelectedEdges)
                    _graph.RemoveEdge(e.V1, e.V2);
            }

            Invalidate();
            GraphChanged();
        }

        public void DoReverseSelectedEdges()
        {
            if (_graph.SelectedEdges.Count > 0)
            {
                foreach (var e in _graph.SelectedEdges)
                {
                    var oo = e.Orientation;
                    if (oo == Edge.Orientations.Forward)
                        oo = Edge.Orientations.Backward;
                    else if (oo == Edge.Orientations.Backward)
                        oo = Edge.Orientations.Forward;

                    e.Orientation = oo;
                }

                Invalidate();
                GraphChanged();
            }
        }

        public void DoRotateSelectedEdges()
        {
            if (_graph.SelectedEdges.Count > 0)
            {
                foreach (var e in _graph.SelectedEdges)
                {
                    var oo = e.Orientation;
                    if (oo == Edge.Orientations.Forward)
                        oo = Edge.Orientations.Backward;
                    else if (oo == Edge.Orientations.Backward)
                        oo = Edge.Orientations.None;
                    else if (oo == Edge.Orientations.None)
                        oo = Edge.Orientations.Forward;

                    e.Orientation = oo;
                }

                Invalidate();
                GraphChanged();
            }
        }

        public void DoCut()
        {
            DoCopy();
            DoDelete();
        }

        public string DoCopy(bool verbose = false)
        {
            var h =  _graph.InducedSubgraph(_graph.SelectedVertices);
            return CompactSerializer.Serialize(h);
        }

        
        public void DoPaste(string s)
        {
            Graph g = null;
            if (s.Contains("tikzpicture"))
            {
                //g = TeXConverter.FromTikz(s);
            }
            else if (CompactSerializer.LooksLikeASerializedGraph(s))
            {
                g = CompactSerializer.Deserialize(s);
            }
            else
            {
                //g = Graph.Deserialize(s);
            }

            if (g != null)
            {
                _graph.DisjointUnion(g);

                Invalidate();
                GraphChanged();
            }
            else if (s.Trim().Split(' ').All(x =>
                {
                    int d;
                    return x.StartsWith("[") || int.TryParse(x, out d);
                }))
            {
                try
                {
                    //g = GraphIO.GraphFromEdgeWeightString(s);
                    if (g != null)
                    {
                        _graph.DisjointUnion(g);

                        Invalidate();
                        GraphChanged();
                    }

                }
                catch { }
            }
            else
            {
                try
                {
                    //g = GraphIO.GraphFromGraph6(s.Trim());

                    if (g != null)
                    {
                        var empty = _graph.Vertices.Count <= 0;
                        _graph.DisjointUnion(g);

                        if (empty)
                            NameModified(g.Name);

                        Invalidate();
                        GraphChanged();
                    }
                }
                catch { }
            }
        }

        public void DoComplement()
        {
            for (int i = 0; i < _graph.SelectedVertices.Count; i++)
            {
                for (int j = i + 1; j < _graph.SelectedVertices.Count; j++)
                {
                    var v1 = _graph.SelectedVertices[i];
                    var v2 = _graph.SelectedVertices[j];

                    var edgeExists = _graph.EdgeExists(v1, v2) || _graph.EdgeExists(v2, v1);

                    if (edgeExists)
                    {
                        _graph.RemoveEdge(v1, v2);
                        _graph.RemoveEdge(v2, v1);
                    }
                    else
                    {
                        _graph.AddEdge(v1, v2);
                    }
                }
            }

            Invalidate();

            GraphChanged();
        }

        public Graph DoSquare()
        {
            var g = new Graph();

            var vertexMap = new Dictionary<Vertex, Vertex>();
            foreach (var v in _graph.Vertices)
            {
                 var w = new Vertex(v.X, v.Y, v.Label);
                 g.AddVertex(w);

                vertexMap[v] = w;
            }

            foreach (var v in _graph.Vertices)
            {
                var q = _graph.FindNeighbors(v).Union(new[] { v }).ToList();

                for (int i = 0; i < q.Count; i++)
                {
                    for (int j = i + 1; j < q.Count; j++)
                    {
                        g.AddEdge(vertexMap[q[i]], vertexMap[q[j]]);
                    }
                }
            }
           

            return g;
        }

        public Graph DoLineGraph()
        {
            var g = new Graph();

            var edgeToCliqueMap = new Dictionary<Edge, List<Vertex>>();
            foreach (var e in _graph.Edges)
            {
                var clique = new List<Vertex>();

                for (int i = 0; i < e.Multiplicity; i++)
                {
                    var v = new Vertex((e.V1.X + e.V2.X) / 2, (e.V1.Y + e.V2.Y) / 2);
                    g.AddVertex(v);
                    v.Label = e.Label;

                    foreach (var other in clique)
                        g.AddEdge(other, v);

                    clique.Add(v);
                }

                edgeToCliqueMap[e] = clique;
            }

            for (int i = 0; i < _graph.Edges.Count; i++)
            {
                for (int j = i + 1; j < _graph.Edges.Count; j++)
                {
                    if (_graph.Edges[i].Meets(_graph.Edges[j]))
                    {
                        foreach (var v1 in edgeToCliqueMap[_graph.Edges[i]])
                        {
                            foreach (var v2 in edgeToCliqueMap[_graph.Edges[j]])
                            {
                                g.AddEdge(v1, v2);
                            }
                        }
                    }
                }
            }

            return g;
        }

        public void DoContractSelectedSubgraph()
        {
            var selected = _graph.SelectedVertices;
            if (selected.Count <= 0)
                return;

            var x = selected.Average(z => z.X);
            var y = selected.Average(z => z.Y);

            var contractedVertex = new Vertex(x, y);
            _graph.AddVertex(contractedVertex);

            var neighbors = selected.SelectMany(v => _graph.FindNeighbors(v)).Distinct();

            foreach (var neighbor in neighbors)
                _graph.AddEdge(contractedVertex, neighbor);

            foreach (var v in selected)
                _graph.RemoveVertex(v);

            Invalidate();

            GraphChanged();
        }
      
        public void DoIndexLabeling()
        {
            var g = new Algorithms.Graph(_graph.GetEdgeWeights());

            for (int i = 0; i < _graph.Vertices.Count; i++)
                _graph.Vertices[i].Label = i.ToString();

            Invalidate();
            GraphChanged();
        }
        public void DoDegreeLabeling()
        {
            var g = new Algorithms.Graph(_graph.GetEdgeWeights());

            for (int i = 0; i < _graph.Vertices.Count; i++)
                _graph.Vertices[i].Label = g.Degree(i).ToString();

            Invalidate();
            GraphChanged();
        }
        public void DoInDegreeLabeling()
        {
            var g = new Algorithms.Graph(_graph.GetEdgeWeights());

            for (int i = 0; i < _graph.Vertices.Count; i++)
                _graph.Vertices[i].Label = g.InDegree(i).ToString();

            Invalidate();
            GraphChanged();
        }
        public void DoOutDegreeLabeling()
        {
            var g = new Algorithms.Graph(_graph.GetEdgeWeights());

            for (int i = 0; i < _graph.Vertices.Count; i++)
                _graph.Vertices[i].Label = g.OutDegree(i).ToString();

            Invalidate();
            GraphChanged();
        }
        public void DoOutDegreePlusOneLabeling()
        {
            var g = new Algorithms.Graph(_graph.GetEdgeWeights());

            for (int i = 0; i < _graph.Vertices.Count; i++)
                _graph.Vertices[i].Label = (g.OutDegree(i) + 1).ToString();

            Invalidate();
            GraphChanged();
        }
        public void DoClearLabels(string s = "", IEnumerable<int> vertices = null)
        {
            if (vertices == null || vertices.Count() <= 0)
                vertices = Enumerable.Range(0, _graph.Vertices.Count);

            foreach (var v in vertices)
                _graph.Vertices[v].Label = s;

            Invalidate();
            GraphChanged();
        }

        public void DoSnapToGrid()
        {
            if (GridStep <= 0)
                return;

            foreach (var v in _graph.Vertices)
            {
                v.X = (float)(_gridStep * Math.Round(v.X / _gridStep));
                v.Y = (float)(_gridStep * Math.Round(v.Y / _gridStep));
            }
        }
        #endregion

        void PaintGrid(IGraphics g)
        {
            if (!DrawGrid || GridStep <= 0)
                return;

            var x = 0.0;
            while (x <= Width)
            {
                g.DrawLine(GridPenColor, x, 0, x, Height);
                x += GridStep * _viewScale;
            }

            var y = 0.0;
            while (y <= Height)
            {
                g.DrawLine(GridPenColor, 0, y, Width, y);
                y += GridStep * _viewScale;
            }
        }

        IHittable GetHit(double x, double y)
        {
            return _graph.HitTest(x, y);
        }

        class HistoricalGraph
        {
            public Graph Graph { get; set; }
            public int ViewScale { get; set; }
            public double Zoom { get; set; }
        }

        public void Paint(GraphicsLayer.IGraphics g, int width, int height)
        {
            if (width == 0 || height == 0)
                return;

            try
            {
                Width = width;
                Height = height;

                if (SnapToGrid)
                    DoSnapToGrid();

                g.Clear(BackgroundColor);
                PaintGrid(g);

                _graph.Paint(g, _viewScale, _viewScale);

                switch (_state)
                {
                    case States.Idle:
                        break;
                    case States.DraggingVertex:
                        break;
                    case States.DraggingSelectionRegion:
                        {
                            List<Box> selectionPoints;
                            lock (_SelectionPointsToken)
                            {
                                selectionPoints = new List<Box>(_selectionPoints.Count);
                                foreach (var p in _selectionPoints)
                                    selectionPoints.Add(new Box(p.X * _viewScale, p.Y * _viewScale));
                            }

                            if (selectionPoints.Count > 1)
                                g.DrawLines(SelectionPenColor, selectionPoints);

                            break;
                        }
                }
            }
            catch { }

            HasPainted = true;

            if (_zoomFitNextPaint)
            {
                _zoomFitNextPaint = false;
                DoZoomFit();
                DoZoom(4, new GraphicsLayer.Box(0.5, 0.5));
            }
        }

        public void OnMouseMove(double X, double Y)
        {
            var x = X / _viewScale;
            var y = Y / _viewScale;

            switch (_state)
            {
                case States.Idle:
                    break;
                case States.DraggingVertex:
                    {
                        _DraggedVertex.X = _DraggedVertex.DragOffset.X + x;
                        _DraggedVertex.Y = _DraggedVertex.DragOffset.Y + y;

                        Invalidate();
                        break;
                    }
                case States.DraggingSelectionRegion:
                    {
                        lock (_SelectionPointsToken)
                        {
                            _selectionPoints.Add(new Box(x, y));

                            Invalidate();
                        }
                        break;
                    }
                case States.DraggingSelectedVertices:
                    {
                        foreach (var v in _graph.SelectedVertices)
                        {
                            v.X = v.DragOffset.X + x;
                            v.Y = v.DragOffset.Y + y;
                        }

                        Invalidate();
                        break;
                    }
            }
        }

        public void ToggleEdgeIndices()
        {
            _graph.ToggleEdgeIndices();

            Invalidate();
        }

        public void RotateVertexIndices()
        {
            var x = _graph.SelectedVertices;
            if (x.Count <= 0)
                x = _graph.Vertices;

            foreach (var a in x)
                a.RotateIndex();

            Invalidate();
        }

        public void RotateEdgeIndices()
        {
            var x = _graph.SelectedEdges;
            if (x.Count <= 0)
                x = _graph.Edges;

            foreach (var a in x)
                a.RotateIndex();

            Invalidate();
        }

        public void SetIndex(int n)
        {
            var x = _graph.SelectedVertices;
            if (x.Count == 1)
            {
                var v = x[0];
                var i = _graph.Vertices.IndexOf(v);
                if (i != n)
                {
                    _graph.Vertices.Remove(v);
                    if (n >= _graph.Vertices.Count)
                    {
                        _graph.Vertices.Add(v);
                    }
                    else if (n < i)
                    {
                        _graph.Vertices.Insert(n, v);
                    }
                    else
                    {
                        _graph.Vertices.Insert(n, v);
                    }
                }
            }
            else
            {
                var y = _graph.SelectedEdges;
                if (y.Count == 1)
                {
                    var v = y[0];
                    var i = _graph.Edges.IndexOf(v);
                    if (i != n)
                    {
                        _graph.Edges.Remove(v);
                        if (n >= _graph.Edges.Count)
                        {
                            _graph.Edges.Add(v);
                        }
                        else if (n < i)
                        {
                            _graph.Edges.Insert(n, v);
                        }
                        else
                        {
                            _graph.Edges.Insert(n, v);
                        }
                    }
                }
            }

            Invalidate();
        }

        public void ToggleVertexIndices()
        {
            _graph.ToggleVertexIndices();

            Invalidate();
        }

        public void OnMouseDown(double X, double Y, GraphicsLayer.MouseButton button)
        {
            _controlWasDown = Canvas.IsControlKeyDown;

            var x = X / _viewScale;
            var y = Y / _viewScale;

            var o = GetHit(x, y);

            switch (_state)
            {
                case States.Idle:
                    {
                        if (button == MouseButton.Left)
                        {
                            if (o == null)
                            {
                                _selectionPoints.Clear();
                                _selectionPoints.Add(new Box(x, y));
                                _state = States.DraggingSelectionRegion;
                            }
                            else if (o is Vertex)
                            {
                                if (((Vertex)o).IsSelected)
                                {
                                    foreach (Vertex v in _graph.SelectedVertices)
                                    {
                                        v.DragOffset = new Box(v.X - x, v.Y - y);
                                    }

                                    _state = States.DraggingSelectedVertices;
                                }
                                else
                                {
                                    _DraggedVertex = (Vertex)o;
                                    _DraggedVertex.DragOffset = new Box(_DraggedVertex.X - x, _DraggedVertex.Y - y);
                                    _state = States.DraggingVertex;
                                }
                            }
                            else if (o is Edge)
                            {
                            }
                        }
                        else if (button == MouseButton.Right)
                        {
                            if (o == null)
                            {
                            }
                            else if (o is Vertex)
                            {
                            }
                            else if (o is Edge)
                            {
                            }
                        }

                        break;
                    }
                case States.DraggingVertex:
                    break;
                case States.DraggingSelectionRegion:
                    break;
            }
        }

        public void OnMouseUp(double X, double Y, GraphicsLayer.MouseButton button)
        {
            var x = X / _viewScale;
            var y = Y / _viewScale;

            var o = GetHit(x, y);

            switch (_state)
            {
                case States.Idle:
                    {
                        break;
                    }
                case States.DraggingVertex:
                    {
                        if (button == MouseButton.Left)
                        {
                            _state = States.Idle;

                            if (SnapToGrid)
                                DoSnapToGrid();

                            GraphChanged();
                        }
                        break;
                    }
                case States.DraggingSelectionRegion:
                    {
                        if (button == MouseButton.Left)
                        {
                            _state = States.Idle;

                            List<Box> selectionPoints;
                            lock (_SelectionPointsToken)
                            {
                                selectionPoints = new List<Box>(_selectionPoints);

                                if (selectionPoints.Count > 2)
                                {
                                    _graph.SelectObjects(selectionPoints, _controlWasDown);
                                }

                                Canvas.SelectedObjects = _graph.SelectedItems.Cast<object>();
                                Invalidate();
                            }
                        }

                        break;
                    }
                case States.DraggingSelectedVertices:
                    {
                        if (button == MouseButton.Left)
                        {
                            _state = States.Idle;
                            GraphChanged();
                        }
                        break;
                    }
            }

            Invalidate();
        }

        public void OnMouseDoubleClick(double X, double Y, GraphicsLayer.MouseButton button)
        {
            var x = X / _viewScale;
            var y = Y / _viewScale;

            var o = GetHit(x, y);

            var graphChanged = false;
            if (_state == States.Idle && !Canvas.IsControlKeyDown)
            {
                if (button == MouseButton.Left)
                {
                    if (o == null)
                    {
                        var v = new Vertex(x, y);
                        if (_graph.AddVertex(v))
                            graphChanged = true;
                    }
                    else if (o is Vertex)
                    {
                        var endVertex = (Vertex)o;
                        foreach (var v in _graph.SelectedVertices)
                        {
                            if (_graph.AddEdge(v, endVertex))
                                graphChanged = true;
                        }
                    }
                }
                else if (button == MouseButton.Right)
                {
                    if (o is Vertex)
                    {
                        if (_graph.RemoveVertex((Vertex)o))
                            graphChanged = true;
                    }
                }
            }

            Invalidate();

            if (graphChanged)
                GraphChanged();
        }

        public void ZoomFitNextPaint()
        {
            _zoomFitNextPaint = true;
            Invalidate();
        }
    }
}

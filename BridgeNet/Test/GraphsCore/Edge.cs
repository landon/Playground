using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Graphs
{
    public class Edge : GraphicsLayer.IPaintable, IHittable
    {
        static readonly GraphicsLayer.Font LabelFont = new GraphicsLayer.Font("Times New Roman", 18);
        static readonly GraphicsLayer.ARGB LabelBrushColor = new GraphicsLayer.ARGB(120, 0, 0, 255);
        static readonly GraphicsLayer.Font IndexFont = new GraphicsLayer.Font("System", 8);
        static readonly GraphicsLayer.ARGB IndexBrushColor = new GraphicsLayer.ARGB(200, 200, 0, 0);

        public int ParentIndex { get; set; }
        public double IndexAngle { get; set; }

        public enum Orientations
        {
            None,
            Forward,
            Backward
        }
        public Edge(Vertex v1, Vertex v2)
            : this(v1, v2, Orientations.None)
        {
        }

        public Edge(Vertex v1, Vertex v2, Orientations orientation)
        {
            _V1 = v1;
            _V2 = v2;
            _Orientation = orientation;
            _Multiplicity = 1;
        }

        public Edge(SerializationEdge e, List<Vertex> vertices)
        {
            V1 = vertices[e.IndexV1];
            V2 = vertices[e.IndexV2];
            Orientation = e.Orientation;
            Thickness = e.Thickness;
            Multiplicity = e.Multiplicity;
            Style = e.Style;
            Label = e.Label;
        }

        public void Paint(GraphicsLayer.IGraphics g, int width, int height)
        {
            if (_V1 == _V2)
            {
                // Draw self loop.
            }
            else
            {
                var maxRadius1 = Math.Max(_V1.LocalBounds.Width, _V1.LocalBounds.Height) / 2;
                var maxRadius2 = Math.Max(_V2.LocalBounds.Width, _V2.LocalBounds.Height) / 2;

                var cos = (_V2.X - _V1.X) / Math.Sqrt((_V2.X - _V1.X) * (_V2.X - _V1.X) + (_V2.Y - _V1.Y) * (_V2.Y - _V1.Y));
                cos = Math.Min(Math.Max(cos, -1), 1);

                var sin = (_V2.Y - _V1.Y) / Math.Sqrt((_V2.X - _V1.X) * (_V2.X - _V1.X) + (_V2.Y - _V1.Y) * (_V2.Y - _V1.Y));
                sin = Math.Min(Math.Max(sin, -1), 1);

                var angle = Math.Acos(cos);
                if (_V1.Y > _V2.Y)
                    angle = -angle;

                if (double.IsNaN(angle))
                    return;

                var angleStep = Math.PI / 2.0 / _Multiplicity;
                var evenModifier = (_Multiplicity % 2 == 0) ? 0.5 : 0;

                GraphicsLayer.Box topStart = new GraphicsLayer.Box(0, 0);
                GraphicsLayer.Box topFinish = new GraphicsLayer.Box(0, 0);
                var first = true;
                for (int i = -(_Multiplicity - 1) / 2; i < Math.Ceiling((_Multiplicity + 1) / 2.0); i++)
                {
                    var startAngle = angle + angleStep * (i - evenModifier);
                    var finishAngle = angle - angleStep * (i - evenModifier);

                    var start = LocalToGlobal(_V1.Location + Utility.PolarToRectangular(maxRadius1 + 0.01, startAngle), width, height);
                    var finish = LocalToGlobal(_V2.Location - Utility.PolarToRectangular(maxRadius2 + 0.01, finishAngle), width, height);


                    if (first)
                    {
                        first = false;
                        topStart = start;
                        topFinish = finish;
                    }

                    g.DrawLine(_penColor, start, finish, _thickness);
                }

                if (_Orientation != Orientations.None)
                {
                    var points = new GraphicsLayer.Box[3];
                    if (_Orientation == Orientations.Forward)
                    {
                        var sweep = _V2.Location - Utility.PolarToRectangular(maxRadius2 + 0.025f, angle);

                        points[0] = LocalToGlobal(Utility.RotateAroundPoint(sweep, _V2.Location, Math.PI / 180.0 * 10.0), width, height);
                        points[1] = LocalToGlobal(_V2.Location - Utility.PolarToRectangular(maxRadius2 + 0.005f, angle), width, height);
                        points[2] = LocalToGlobal(Utility.RotateAroundPoint(sweep, _V2.Location, -Math.PI / 180.0 * 10.0), width, height);
                    }
                    else if (_Orientation == Orientations.Backward)
                    {
                        var sweep = _V1.Location + Utility.PolarToRectangular(maxRadius1 + 0.025f, angle);

                        points[0] = LocalToGlobal(Utility.RotateAroundPoint(sweep, _V1.Location, Math.PI / 180.0 * 10.0), width, height);
                        points[1] = LocalToGlobal(_V1.Location + Utility.PolarToRectangular(maxRadius1 + 0.005f, angle), width, height);
                        points[2] = LocalToGlobal(Utility.RotateAroundPoint(sweep, _V1.Location, -Math.PI / 180.0 * 10.0), width, height);
                    }

                    g.FillPolygon(new GraphicsLayer.ARGB(0, 0, 0), points);
                }

                var label = Label;
                if (!string.IsNullOrEmpty(label))
                {
                    var box = g.MeasureString(label, LabelFont);
                    var bounds = new GraphicsLayer.Box(topStart.X + (topFinish.X - topStart.X) / 2 - box.Width / 2 - 0.75 * box.Width * sin, topStart.Y + (topFinish.Y - topStart.Y) / 2 - box.Height / 2 + 0.75 * box.Height * cos, box.Width, box.Height);
                    g.DrawString(label, LabelFont, LabelBrushColor, bounds);
                }

                if (_showIndex)
                {
                    var bounds = new GraphicsLayer.Box(topStart.X + (topFinish.X - topStart.X) / 2, topStart.Y + (topFinish.Y - topStart.Y) / 2, 10, 10);
                    var cx = (bounds.Left + bounds.Right) / 2;
                    var cy = (bounds.Bottom + bounds.Top) / 2;
                    var r = Math.Max(bounds.Width, bounds.Height) / 2 + 5;
                    var bb = new GraphicsLayer.Box(cx + r * Math.Cos(IndexAngle) - 5, cy + r * Math.Sin(IndexAngle) - 5, 10, 10);
                    g.DrawString(ParentIndex.ToString(), IndexFont, IndexBrushColor, bb);
                }
            }
        }

        GraphicsLayer.Box LocalToGlobal(Vector local, int width, int height)
        {
            return new GraphicsLayer.Box(width * local.X, height * local.Y);
        }

        public bool Hit(double x, double y)
        {
            return false;
        }

        public bool Meets(Edge other)
        {
            return _V1 == other.V1 ||
                   _V2 == other.V1 ||
                   _V1 == other.V2 ||
                   _V2 == other.V2;
        }

        
        public Vertex V1
        {
            get
            {
                return _V1;
            }
            set
            {
                _V1 = value;
            }
        }

        
        public Vertex V2
        {
            get
            {
                return _V2;
            }
            set
            {
                _V2 = value;
            }
        }

        
        public Orientations Orientation
        {
            get
            {
                return _Orientation;
            }
            set
            {
                _Orientation = value;

                UpdateStyle();
            }
        }

        void UpdateStyle()
        {
            if (Style == null)
                Style = "";

            var sb = new StringBuilder();
            Style = sb.ToString();
        }

        
        public float Thickness
        {
            get
            {
                return _thickness;
            }
            set
            {
                _thickness = value;
            }
        }

        
        public GraphicsLayer.ARGB Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
                _penColor = _Color;
            }
        }

        
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (value == _IsSelected)
                    return;

                _IsSelected = value;

                Color = _IsSelected ? new GraphicsLayer.ARGB(50, 205, 50) : new GraphicsLayer.ARGB(0, 0, 0);
            }
        }

        
        public int Multiplicity
        {
            get
            {
                return _Multiplicity;
            }
            set
            {
                _Multiplicity = Math.Max(1, value);
            }
        }

        
        public string Style
        {
            get
            {
                return _style;
            }
            set
            {
                if (value == null)
                    return;

                var style = value;
                if (style.Contains("~~|~|~~"))
                {
                    var parts = style.Split(new string[] { "~~|~|~~" }, StringSplitOptions.None);
                    if (parts.Length >= 2)
                    {
                        style = parts[0];
                        Label = parts[1];
                    }
                }

                if (style.StartsWith("+") && !string.IsNullOrEmpty(_style))
                    _style += ", " + style.TrimStart('+');
                else
                    _style = style.TrimStart('+');
            }
        }

        
        public string Label { get; set; }
   
        Vertex _V1;
        Vertex _V2;
        Orientations _Orientation;
        float _thickness = 4;
        GraphicsLayer.ARGB _Color = new GraphicsLayer.ARGB(0, 0, 0);
        GraphicsLayer.ARGB _penColor = new GraphicsLayer.ARGB(0, 0, 0);
        bool _IsSelected;
        int _Multiplicity;
        string _style;
        bool _showIndex = false;

        internal void ToggleIndex()
        {
            _showIndex = !_showIndex;
        }

        internal void RotateIndex()
        {
            IndexAngle += Math.PI / 16;
            if (IndexAngle >= 2 * Math.PI)
                IndexAngle = 0;
        }
    }
}


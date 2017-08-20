using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Graphs
{
    public class Vertex : GraphicsLayer.IPaintable, IHittable
    {
        static readonly GraphicsLayer.Font LabelFont = new GraphicsLayer.Font("Times New Roman", 12);
        static readonly GraphicsLayer.Font IndexFont = new GraphicsLayer.Font("System", 8);
        static readonly GraphicsLayer.ARGB LabelBrushColor = new GraphicsLayer.ARGB(0, 0, 0);
        static readonly GraphicsLayer.ARGB IndexBrushColor = new GraphicsLayer.ARGB(200, 200, 0, 0);
        static readonly GraphicsLayer.ARGB BoundaryPenColor = new GraphicsLayer.ARGB(0, 0, 0);
        static readonly GraphicsLayer.ARGB BoundarySelectedPenColor = new GraphicsLayer.ARGB(255, 0, 255, 0);
        static readonly int BoundarySelectedPenWidth = 2;
        public static readonly GraphicsLayer.ARGB DefaultFillBrushColor = new GraphicsLayer.ARGB(120, 0, 0, 0);
        static readonly GraphicsLayer.ARGB UniversalVertexFillBrushColor = new GraphicsLayer.ARGB(120, 0, 0, 255);
        static readonly GraphicsLayer.ARGB SelectedFillBrushColor = new GraphicsLayer.ARGB(255, 0, 255, 127);

        public int ParentIndex { get; set; }
        public double IndexAngle { get; set; }

        public Vertex(double x, double y)
            : this(x, y, "")
        {
        }

        public Vertex(Vector v)
            : this(v, "")
        {
        }

        public Vertex(Vector v, string label)
            : this(v.X, v.Y, label)
        {
        }

        public Vertex(double x, double y, string label)
        {
            _Location = new Vector(x, y);
            _label = label;
        }

        public Vertex(SerializationVertex v)
        {
            Location = new Vector(v.Location.X, v.Location.Y);
            Label = v.Label;
            Padding = v.Padding;
            Style = v.Style;
        }

        public virtual void Paint(GraphicsLayer.IGraphics g, int width, int height)
        {
            var bounds = ComputeBounds(g, width, height);

            if (!string.IsNullOrEmpty(_label))
            {
                if (Color.Equals(default(GraphicsLayer.ARGB)))
                {
                    if (IsUniversal)
                        g.FillEllipse(UniversalVertexFillBrushColor, bounds);
                }
                else
                {
                    g.FillEllipse(Color, bounds);
                }

                g.DrawEllipse(_IsSelected ? BoundarySelectedPenColor : BoundaryPenColor, bounds, _IsSelected ? BoundarySelectedPenWidth : 1);
                g.DrawString(_label, LabelFont, LabelBrushColor, bounds);
            }
            else
            {
                if (Color.Equals(default(GraphicsLayer.ARGB)))
                    g.FillEllipse(IsUniversal ? UniversalVertexFillBrushColor : DefaultFillBrushColor, bounds);
                else
                    g.FillEllipse(Color, bounds);

                
                g.DrawEllipse(_IsSelected ? BoundarySelectedPenColor : BoundaryPenColor, bounds, _IsSelected ? BoundarySelectedPenWidth : 1);
            }

            if (_showIndex)
            {
                var cx = (bounds.Left + bounds.Right) / 2;
                var cy = (bounds.Bottom + bounds.Top) / 2;
                var r = Math.Max(bounds.Width, bounds.Height) / 2 + 5;
                var bb = new GraphicsLayer.Box(cx + r * Math.Cos(IndexAngle) - 5, cy + r * Math.Sin(IndexAngle) - 5, 10, 10);
                g.DrawString(ParentIndex.ToString(), IndexFont, IndexBrushColor, bb);
            }
        }

        public bool Hit(double x, double y)
        {
            var hit = _LocalBounds.Contains(x, y);
            return hit;
        }

        public GraphicsLayer.Box ComputeBounds(GraphicsLayer.IGraphics g, int width, int height)
        {
            GraphicsLayer.Box bounds;

            if (!string.IsNullOrEmpty(_label))
            {
                var size = g.MeasureString(_label, LabelFont);

                bounds = new GraphicsLayer.Box(X * width - size.Width / 2, Y * height - size.Height / 2, size.Width, size.Height);
            }
            else
            {
                bounds = new GraphicsLayer.Box(X * width, Y * height, 0, 0);
            }

            bounds.Inflate(_padding * width, _padding * height);

            _LocalBounds = new GraphicsLayer.Box(bounds.X / width, bounds.Y / height, bounds.Width / width, bounds.Height / height);

            return bounds;
        }

        public GraphicsLayer.ARGB Color { get; set; }

        public int Modifier { get; set; }

        public double X
        {
            get
            {
                return _Location.X;
            }
            set
            {
                _Location.X = value;
            }
        }

        public double Y
        {
            get
            {
                return _Location.Y;
            }
            set
            {
                _Location.Y = value;
            }
        }

        public Vector Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
            }
        }

        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                if (value == _label)
                    return;

                _label = value;
            }
        }

        public float Padding
        {
            get
            {
                return _padding;
            }
            set
            {
                if (value == _padding)
                    return;

                _padding = value;
            }
        }

        public GraphicsLayer.Box LocalBounds
        {
            get
            {
                return _LocalBounds;
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
            }
        }

        public GraphicsLayer.Box DragOffset
        {
            get
            {
                return _DragOffset;
            }
            set
            {
                _DragOffset = value;
            }
        }

        public string Style
        {
            get
            {
                return _Style;
            }
            set
            {
                if (value.StartsWith("+") && !string.IsNullOrEmpty(_Style))
                    _Style += ", " + value.TrimStart('+');
                else
                    _Style = value.TrimStart('+');
            }
        }

        public bool IsUniversal { get; set; }

        Vector _Location;
        GraphicsLayer.Box _DragOffset = GraphicsLayer.Box.Empty;
        string _label;
        string _Style;
        bool _IsSelected;

        float _padding = 2.0f / 100.0f;
        GraphicsLayer.Box _LocalBounds = GraphicsLayer.Box.Empty;
        bool _showIndex = false;

        internal void RotateIndex()
        {
            IndexAngle += Math.PI / 16;
            if (IndexAngle >= 2 * Math.PI)
                IndexAngle = 0;
        }

        internal void ToggleIndex()
        {
            _showIndex = !_showIndex;
        }
    }
}

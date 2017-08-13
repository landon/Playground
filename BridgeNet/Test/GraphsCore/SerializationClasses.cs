using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Graphs
{
    [DataContract(Name = "Vector")]
    public class SerializationVector
    {
        [DataMember]
        public double X;
        [DataMember]
        public double Y;

        public SerializationVector() { }
        public SerializationVector(Vector v)
        {
            X = v.X;
            Y = v.Y;
        }
    }
    [DataContract(Name = "Vertex")]
    public class SerializationVertex
    {
        [DataMember]
        public SerializationVector Location;
        [DataMember]
        public string Label;
        [DataMember]
        public float Padding;
        [DataMember]
        public string Style;

        public SerializationVertex() { }
        public SerializationVertex(Vertex v)
        {
            Location = new SerializationVector(v.Location);
            Label = v.Label;
            Padding = v.Padding;
            Style = v.Style ?? "";
        }
    }
    [DataContract(Name = "Edge")]
    public class SerializationEdge
    {
        [DataMember]
        public int IndexV1;
        [DataMember]
        public int IndexV2;
        [DataMember]
        public Edge.Orientations Orientation;
        [DataMember]
        public float Thickness;
        [DataMember]
        public int Multiplicity;
        [DataMember]
        public string Style;
        [DataMember]
        public string Label;

        public SerializationEdge() { }
        public SerializationEdge(Edge e, List<Vertex> vertices)
        {
            IndexV1 = vertices.IndexOf(e.V1);
            IndexV2 = vertices.IndexOf(e.V2);
            Orientation = e.Orientation;
            Thickness = e.Thickness;
            Multiplicity = e.Multiplicity;
            Style = e.Style ?? "";
            Label = e.Label ?? "";

            if (Style == "")
            {
                if (Orientation == Edge.Orientations.Forward)
                    Style = e.Style = "post";
                else if (Orientation == Edge.Orientations.Backward)
                    Style = e.Style = "pre";
            }
        }
    }
    [DataContract(Name = "Graph")]
    public class SerializationGraph
    {
        [DataMember]
        public List<SerializationVertex> Vertices;
        [DataMember]
        public List<SerializationEdge> Edges;
        [DataMember]
        public string Name;

        public SerializationGraph() { }
        public SerializationGraph(Graph g)
        {
            Vertices = g.Vertices.Select(v => new SerializationVertex(v)).ToList();
            Edges = g.Edges.Select(e => new SerializationEdge(e, g.Vertices)).ToList();
            Name = g.Name;
        }
    }
}


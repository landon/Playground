using BitLevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SuperSlimColoringAnalyzer
    {
        Graph _lineGraph;
        Func<SuperSlimBoard, int, long> _getEdgeColorList;

        public SuperSlimColoringAnalyzer(Graph lineGraph, Func<SuperSlimBoard, int, long> getEdgeColorList)
        {
            _lineGraph = lineGraph;
            _getEdgeColorList = getEdgeColorList;
        }

        public bool Analyze(SuperSlimBoard b)
        {
            return _lineGraph.IsChoosable(Enumerable.Range(0, _lineGraph.N).Select(e => _getEdgeColorList(b, e)).ToList());
        }
      
        public bool ColorableWithoutEdge(SuperSlimBoard b, int edgeIndex)
        {
            return _lineGraph.IsChoosable(Enumerable.Range(0, _lineGraph.N).Select(e =>
            {
                if (e == edgeIndex)
                    return -1;

                return _getEdgeColorList(b, e);
            }).ToList());
        }


        public bool Analyze(SuperSlimBoard b, out Dictionary<int, long> coloring)
        {
            return IsChoosable(Enumerable.Range(0, _lineGraph.N).Select(e => _getEdgeColorList(b, e)).ToList(), out coloring);
        }

        public bool AnalyzeWithoutEdge(SuperSlimBoard b, out Dictionary<int, long> coloring, int edgeIndex)
        {
            return IsChoosable(Enumerable.Range(0, _lineGraph.N).Select(e =>
            {
                if (e == edgeIndex)
                    return -1;

                return _getEdgeColorList(b, e);
            }).ToList(), out coloring);
        }

        public bool IsChoosable(List<long> assignment, out Dictionary<int, long> coloring)
        {
            coloring = new Dictionary<int, long>();
            return IsChoosable(assignment, 0, coloring);
        }
        bool IsChoosable(List<long> assignment, int v, Dictionary<int, long> coloring)
        {
            if (v >= _lineGraph.N)
            {
                return true;
            }

            var colors = assignment[v];
            while (colors != 0)
            {
                var color = colors & -colors;

                var assignmentCopy = new List<long>(assignment);
                foreach (var neighbor in _lineGraph._laterNeighbors.Value[v])
                    assignmentCopy[neighbor] &= ~color;

                if (IsChoosable(assignmentCopy, v + 1, coloring))
                {
                    coloring[v] = color;
                    return true;
                }

                colors ^= color;
            }

            return false;
        }
    }
}

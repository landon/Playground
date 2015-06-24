using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability
{
    public class Mind<TColorSet, TVertexSet, TGraph>
        where TGraph : IGraph<TColorSet, TVertexSet>, new()
    {
        IGraph<TColorSet, TVertexSet> _graph;
        IGraph<TColorSet, TVertexSet> _lineGraph;
        List<Tuple<int, int>> _edges;
        DynamicAnalyzer<TColorSet, TVertexSet> _dynamicAnalyzer;
        StaticAnalyzer<TColorSet, TVertexSet> _staticAnalyzer;
        IAssignmentGenerator<TColorSet, TVertexSet> _assignmentGenerator;

        public List<IAssignment<TColorSet, TVertexSet>> Assignments { get; private set; }
        public List<IAssignment<TColorSet, TVertexSet>> ColorableAssignments { get; private set; }
        public List<IAssignment<TColorSet, TVertexSet>> NearlyColorableAssignments { get; private set; }
        public List<IAssignment<TColorSet, TVertexSet>> NonColorableAssignments { get; private set; }
        public List<IAssignment<TColorSet, TVertexSet>> NonSuperabundantAssignments { get; private set; }

        public Mind(TGraph graph, DynamicAnalyzer<TColorSet, TVertexSet> dynamicAnalyzer, StaticAnalyzer<TColorSet, TVertexSet> staticAnalyzer, IAssignmentGenerator<TColorSet, TVertexSet> assignmentGenerator)
        {
            _graph = graph;
            _dynamicAnalyzer = dynamicAnalyzer;
            _staticAnalyzer = staticAnalyzer;
            _assignmentGenerator = assignmentGenerator;

            BuildLineGraph();

            _dynamicAnalyzer.Initialize(_graph, _lineGraph, _edges);
            _staticAnalyzer.Initialize(_graph, _lineGraph, _edges);
        }

        public AnalysisResult Analyze(int[] sizes, int maxPot = int.MaxValue)
        {
            if (sizes.Length != _graph.N)
                throw new Exception("Wrong number of list sizes.");

            var minPot = sizes.Max();
            maxPot = Math.Min(maxPot, sizes.Sum());

            Assignments = _assignmentGenerator.Generate(sizes, minPot, maxPot);
           
            DoStaticAnalysis();
            var remainingAssignments = DoDynamicAnalysis();

            if (remainingAssignments.Count <= 0)
                return AnalysisResult.FixableForAllAssignments;

            var result = AnalysisResult.NotFixable;
            if (remainingAssignments.All(a => !NearlyColorableAssignments.Contains(a)))
                result |= AnalysisResult.FixableForNearlyColorableAssignments;
            if (remainingAssignments.All(a => NonSuperabundantAssignments.Contains(a)))
                result |= AnalysisResult.FixableForSuperabundantAssignments;
            if (result == AnalysisResult.NotFixable && remainingAssignments.All(a => !NearlyColorableAssignments.Contains(a) || NonSuperabundantAssignments.Contains(a)))
                result |= AnalysisResult.FixableForNearlyColorableSuperabundantAssignments;

            return result;
        }

        List<IAssignment<TColorSet, TVertexSet>> DoDynamicAnalysis()
        {
            var targetAssignments = new HashSet<IAssignment<TColorSet, TVertexSet>>(ColorableAssignments);
            var remainingAssignments = NonColorableAssignments.ToList();

            while (true)
            {
                var wonAssignments = new List<IAssignment<TColorSet, TVertexSet>>();

                for (int i = remainingAssignments.Count - 1; i >= 0; i--)
                {
                    var assignment = remainingAssignments[i];
                    if (_dynamicAnalyzer.Analyze(assignment, targetAssignments))
                    {
                        remainingAssignments.RemoveAt(i);
                        wonAssignments.Add(assignment);
                    }
                }

                if (wonAssignments.Count <= 0)
                    break;

                foreach (var assignment in wonAssignments)
                    targetAssignments.Add(assignment);
            }

            return remainingAssignments;
        }

        void DoStaticAnalysis()
        {
            ColorableAssignments = new List<IAssignment<TColorSet, TVertexSet>>();
            NonColorableAssignments = new List<IAssignment<TColorSet, TVertexSet>>();
            NonSuperabundantAssignments = new List<IAssignment<TColorSet, TVertexSet>>();
            NearlyColorableAssignments = new List<IAssignment<TColorSet, TVertexSet>>();

            foreach (var assignment in Assignments)
            {
                if (_staticAnalyzer.IsEdgeColorable(assignment))
                    ColorableAssignments.Add(assignment);
                else
                {
                    NonColorableAssignments.Add(assignment);
                    if (!_staticAnalyzer.IsSuperabundant(assignment))
                        NonSuperabundantAssignments.Add(assignment);
                    if (_staticAnalyzer.IsNearlyEdgeColorable(assignment))
                        NearlyColorableAssignments.Add(assignment);
                }
            }
        }

        void BuildLineGraph()
        {
            int n = _graph.N;

            _edges = new List<Tuple<int, int>>();
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (_graph.AreAdjacent(i, j))
                        _edges.Add(new Tuple<int, int>(i, j));
                }
            }

            var meets = new bool[_edges.Count, _edges.Count];
            for (int i = 0; i < _edges.Count; i++)
            {
                for (int j = i + 1; j < _edges.Count; j++)
                {
                    if (_edges[i].Item1 == _edges[j].Item1 ||
                        _edges[i].Item1 == _edges[j].Item2 ||
                        _edges[i].Item2 == _edges[j].Item1 ||
                        _edges[i].Item2 == _edges[j].Item2)
                    {
                        meets[i, j] = meets[j, i] = true;
                    }
                }
            }

            _lineGraph = new TGraph();
            _lineGraph.Initialize(meets);
        }
    }
}

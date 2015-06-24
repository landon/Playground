using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability
{
    public class Mind
    {
        IGraph _graph;
        IDynamicAnalyzer _dynamicAnalyzer;
        IStaticAnalyzer _staticAnalyzer;
        IAssignmentGenerator _assignmentGenerator;
        IListHelper _listHelper;

        public List<IAssignment> Assignments { get; private set; }
        public List<IAssignment> ColorableAssignments { get; private set; }
        public List<IAssignment> NearlyColorableAssignments { get; private set; }
        public List<IAssignment> NonColorableAssignments { get; private set; }
        public List<IAssignment> NonSuperabundantAssignments { get; private set; }

        public Mind(IGraph graph, IDynamicAnalyzer dynamicAnalyzer, IStaticAnalyzer staticAnalyzer, IAssignmentGenerator assignmentGenerator, IListHelper listHelper)
        {
            _graph = graph;
            _dynamicAnalyzer = dynamicAnalyzer;
            _staticAnalyzer = staticAnalyzer;
            _assignmentGenerator = assignmentGenerator;
            _listHelper = listHelper;
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

            return AnalysisResult.NotFixable;
        }

        List<IAssignment> DoDynamicAnalysis()
        {
            var targetAssignments = new HashSet<IAssignment>(ColorableAssignments);
            var remainingAssignments = NonColorableAssignments.ToList();

            while (true)
            {
                var wonAssignments = new List<IAssignment>();

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
            ColorableAssignments = new List<IAssignment>();
            NonColorableAssignments = new List<IAssignment>();
            NonSuperabundantAssignments = new List<IAssignment>();
            NearlyColorableAssignments = new List<IAssignment>();

            foreach (var assignment in Assignments)
            {
                if (_staticAnalyzer.IsColorable(assignment))
                    ColorableAssignments.Add(assignment);
                else
                {
                    NonColorableAssignments.Add(assignment);
                    if (!_staticAnalyzer.IsSuperabundant(assignment))
                        NonSuperabundantAssignments.Add(assignment);
                    if (_staticAnalyzer.IsNearlyColorable(assignment))
                        NearlyColorableAssignments.Add(assignment);
                }
            }
        }
    }
}

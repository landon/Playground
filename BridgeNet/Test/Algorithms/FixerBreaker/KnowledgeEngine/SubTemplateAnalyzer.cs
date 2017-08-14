using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class SubTemplateAnalyzer : IBoardAnalyzer
    {
        List<Template> SubTemplates { get; set; }

        public string Reason { get { return "sub template"; } }

        public bool IsKnowledgeDependent
        {
            get { return false; }
        }

        public SubTemplateAnalyzer(params Template[] subTemplates)
        {
            SubTemplates = subTemplates.ToList();
        }

        public bool Analyze(Knowledge knowledge, Board board)
        {
            foreach (var template in SubTemplates)
            {
                int color;
                int x;
                if (ContainsSuberabundantSubStacksAboveTemplate(knowledge, board, template, out color, out x))
                {
                    knowledge[board.Template.Value][board.ColorCount].AddWin(board, string.Format("set L({0}) = L({0}) - {1}", x + 1, color));
                    return true;
                }
            }

            return false;
        }

        bool ContainsSuberabundantSubStacksAboveTemplate(Knowledge knowledge, Board board, Template template, out int color, out int x)
        {
            color = -1;
            x = -1;
            var g = knowledge.GraphKnowledge.Graph;
            var stackSets = board.Stacks.Select(s => s.ToSet()).ToList();
            var bigs = Enumerable.Range(0, stackSets.Count).Count(i => stackSets[i].Count > template.Sizes[i]);
            if (bigs <= 0 || bigs > 1)
                return false;

            var shrinkable = Enumerable.Range(0, stackSets.Count).Where(i => stackSets[i].Count == template.Sizes[i] + 1).ToList();
            if (shrinkable.Count <= 0)
                return false;

            x = shrinkable[0];
            foreach (var c in stackSets[x].ToList())
            {
                stackSets[x].Remove(c);

                if (g.DegreeCondition(stackSets.Select(s => s.ToInt64()).ToList(), board.Pot))
                {
                    color = c;
                    return true;
                }

                stackSets[x].Add(c);
            }

            return false;
        }
    }
}

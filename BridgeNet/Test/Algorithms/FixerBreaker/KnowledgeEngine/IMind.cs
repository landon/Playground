using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public interface IMind
    {
        bool Analyze(Template template, Action<Tuple<string, int>> progress);
        bool FixerWonAllNearlyColorableBoards { get; }
        int TotalPositions { get; }
        int MaxPot { set; }
    }
}

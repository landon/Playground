using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public interface IBoardAnalyzer
    {
        string Reason { get; }
        bool IsKnowledgeDependent { get; }
        bool Analyze(Knowledge knowledge, Board board);
    }
}

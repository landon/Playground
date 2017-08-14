using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class ProofCase
    {
        public SuperSlimMind Mind { get; private set; }
        public List<SuperSlimBoard> Boards { get; private set; }
        public bool BreakerWin { get; set; }
        public int CaseNumber { get; private set; }
        public bool Superabundant { get; set; }

        public ProofCase(SuperSlimMind mind, int caseNumber, List<SuperSlimBoard> boards = null)
        {
            Mind = mind;
            CaseNumber = caseNumber;
            Boards = boards;
            if (Boards == null)
                Boards = new List<SuperSlimBoard>();
        }

        public void AddBoard(SuperSlimBoard board)
        {
            Boards.Add(board);
        }
    }
}

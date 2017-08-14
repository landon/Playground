using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public abstract class ProofBuilder
    {
        public SuperSlimMind Mind { get; private set; }
        public List<ProofCase> Cases { get; protected set; }
        public bool UsePermutations { get; set; }

        public ProofBuilder(SuperSlimMind mind, bool usePermutations = false)
        {
            Mind = mind;
            UsePermutations = usePermutations;
            ExtractCases();
        }

        protected virtual void ExtractCases()
        {
            Cases = new List<ProofCase>();

            var caseNumber = 0;

            var colorableCase = new ProofCase(Mind, 0, Mind.ColorableBoards);
            Cases.Add(colorableCase);
            caseNumber++;

            var remainingBoards = Mind.NonColorableBoards.ToList();
            var wonBoards = Mind.ColorableBoards.ToList();
            while (remainingBoards.Count > 0)
            {
                var proofCase = new ProofCase(Mind, caseNumber);
                Cases.Add(proofCase);

                var addedBoards = new List<SuperSlimBoard>();
                foreach (var board in remainingBoards)
                {
                    var treeInfo = Mind.GetWinTreeInfo(board);

                    if (treeInfo.All(bc => wonBoards.Contains(new SuperSlimBoard(board._trace, bc.Alpha, bc.Beta, bc.Response, board._stackCount))))
                        addedBoards.Add(board);
                }

                foreach (var board in addedBoards)
                {
                    proofCase.AddBoard(board);
                    wonBoards.Add(board);
                    remainingBoards.Remove(board);
                }

                caseNumber++;
            }
        }

        protected virtual int GetHandledCaseNumber(SuperSlimBoard b, BreakerChoiceInfo bc)
        {
            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
            return Cases.IndicesWhere(cc => cc.Boards.Contains(childBoard)).First() + 1;
        }

        public abstract string WriteProof();
    }
}

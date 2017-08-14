using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorithms.Utility;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public abstract class PermutationAwareProofBuilder : ProofBuilder
    {
        protected Dictionary<SuperSlimBoard, List<Tuple<Permutation, SuperSlimBoard>>> _permutationLinked;
        

        public PermutationAwareProofBuilder(SuperSlimMind mind, bool usePermutations = false)
            : base(mind, usePermutations)
        {
            
        }

        protected override void ExtractCases()
        {
            Cases = new List<ProofCase>();

            _permutationLinked = new Dictionary<SuperSlimBoard, List<Tuple<Permutation, SuperSlimBoard>>>();

            var indices = Enumerable.Range(0, Mind.ColorableBoards[0].Stacks.Value.Length).ToList();
            var permutations = Permutation.EnumerateAll(indices.Count).ToList();

            var caseNumber = 0;

            var colorableCase = new ProofCase(Mind, 0, Mind.ColorableBoards);
            Cases.Add(colorableCase);
            caseNumber++;

            var remainingBoards = Mind.NonColorableBoards.Except(Mind.BreakerWonBoards).ToList();
            var wonBoards = Mind.ColorableBoards.ToList();
            while (remainingBoards.Count > 0)
            {
                var proofCase = new ProofCase(Mind, caseNumber);
                Cases.Add(proofCase);

                var addedRootBoards = new List<SuperSlimBoard>();
                var addedBoards = new List<SuperSlimBoard>();
                foreach (var board in remainingBoards)
                {
                    if (addedBoards.Contains(board))
                        continue;

                    var treeInfo = Mind.GetWinTreeInfo(board);
                    var childBoards = treeInfo.Select(bc => new SuperSlimBoard(board._trace, bc.Alpha, bc.Beta, bc.Response, board._stackCount)).Distinct().ToList();

                    if (childBoards.SubsetEqual(wonBoards))
                    {
                        addedRootBoards.Add(board);
                        addedBoards.Add(board);
                        _permutationLinked[board] = new List<Tuple<Permutation, SuperSlimBoard>>();

                        if (UsePermutations)
                        {
                            foreach (var p in permutations)
                            {
                                var pb = board.Permute(p, indices);
                                if (wonBoards.Contains(pb) || addedBoards.Contains(pb))
                                    continue;

                                var closed = true;
                                foreach (var cb in childBoards)
                                {
                                    if (!wonBoards.Contains(cb.Permute(p, indices)))
                                    {
                                        closed = false;
                                        break;
                                    }
                                }

                                if (closed)
                                {
                                    _permutationLinked[board].Add(new Tuple<Permutation, SuperSlimBoard>(p, pb));
                                    addedBoards.Add(pb);
                                }
                            }
                        }
                    }
                }

                foreach (var board in addedRootBoards)
                {
                    proofCase.AddBoard(board);
                }

                foreach (var board in addedBoards)
                {
                    wonBoards.Add(board);
                    remainingBoards.Remove(board);
                }

                caseNumber++;
            }

            if (Mind.BreakerWonBoards.Count > 0)
            {
                foreach (var group in Mind.BreakerWonBoards.GroupBy(b => Mind.IsSuperabundant(b)))
                {
                    var lostCase = new ProofCase(Mind, caseNumber, group.ToList()) { BreakerWin = true, Superabundant = group.Key };
                    Cases.Add(lostCase);
                }
            }
        }

        protected override int GetHandledCaseNumber(SuperSlimBoard b, BreakerChoiceInfo bc)
        {
            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
            if (Cases[0].Boards.Contains(childBoard))
                return 1;

            return Cases.Skip(1).IndicesWhere(cc => cc.Boards.SelectMany(bb => new[] { bb }.Union(_permutationLinked[bb].Select(tup => tup.Item2))).Contains(childBoard)).First() + 2;
        }
    }
}

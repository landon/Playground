using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs.ArbitraryMaxDegree;
using Algorithms.Utility;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class ArbitraryDegreeProofBuilder : PermutationAwareProofBuilder
    {
        public bool UseWildCards { get; set; }

        
        string _figureTikz;
        int _maxPot;
        List<int> _activeIndices;
        List<int> _activeListSizes;
        List<int> _possibleListIndices;
        List<List<int>> _possibleLists;
        List<SuperSlimBoard> _allBoards;
        SequenceGeneralizer<int>.VectorComparer _sequenceComparer;
        SequenceGeneralizer<int> _sequenceGeneralizer;
        Dictionary<string, string> _orderFilter = new Dictionary<string, string>();
        bool _isWin;

        public ArbitraryDegreeProofBuilder(SuperSlimMind mind, string figureTikz = "", bool usePermutations = false)
            : base(mind, usePermutations)
        {
            _figureTikz = figureTikz;
            _maxPot = mind.MaxPot;
            UseWildCards = true;
        }

        public override string WriteProof()
        {
            var sb = new StringBuilder();

            _isWin = Cases.All(c => !c.BreakerWin);
       
            AddFigure(sb);
            BeginProof(sb);
            GeneratePossibleLists();
            GeneralizeAllBoards(sb);

            var wonBoards = new List<SuperSlimBoard>();
            for (int caseNumber = 1; caseNumber <= Cases.Count; caseNumber++)
            {
                var c = Cases[caseNumber - 1];
                var boards = c.Boards.OrderBy(b => b.ToListStringInLexOrder(_maxPot)).ToList();
                
                List<SuperSlimBoard> thisClaimBoards;
                if (caseNumber > 1 && !c.BreakerWin)
                    thisClaimBoards = boards.SelectMany(b => new[] { b }.Union(_permutationLinked[b].Select(tup => tup.Item2))).ToList();
                else
                    thisClaimBoards = boards;

                if (!c.BreakerWin)
                    wonBoards.AddRange(thisClaimBoards);

                var thisClaimBoardsTex = GeneralizeBoards(thisClaimBoards);

                sb.AppendLine();
                sb.AppendLine("\\bigskip");
                sb.AppendLine(string.Format("\\case{{{0}}}{{$B$ is one of the {1} following boards:\n " + thisClaimBoardsTex + ".}}",caseNumber, thisClaimBoards.Count));
                sb.AppendLine();
                sb.AppendLine("\\bigskip");

                if (c.BreakerWin)
                {
                    sb.AppendLine();

                    if (c.Superabundant)
                        sb.AppendLine("No single Kempe exchange gets from these boards to a previous case.");
                    else
                        sb.AppendLine("These boards are not superabundant.");
                }
                else if (caseNumber == 1)
                {
                    sb.AppendLine();
                    sb.AppendLine("In all these cases, $H$ is immediately colorable from the lists.");
                }
                else
                {
                    sb.AppendLine();

                    var swapCountGroups = boards.GroupBy(b => Mind.GetWinTreeInfo(b).Max(ti => ti.SwapVertices.Count)).ToList();
                    foreach (var swapCountGroup in swapCountGroups)
                    {
                        if (swapCountGroup.Key == 1)
                        {
                            sb.AppendLine("Each of the following boards can be handled by a single Kempe change that has an endpoint at infinity.");

                            foreach (var b in swapCountGroup)
                            {
                                Permutation pp;
                                var listString = b.ToListStringInLexOrder(out pp);

                                var treeInfo = Mind.GetWinTreeInfo(b);
                                var alpha = Math.Min(pp[treeInfo.First().Alpha], pp[treeInfo.First().Beta]);
                                var beta = Math.Max(pp[treeInfo.First().Alpha], pp[treeInfo.First().Beta]);
                                var groups = treeInfo.GroupBy(ss => ss.SwapVertices[0]);

                                if (!CheckPermutationGoodNess(alpha, beta, listString, treeInfo.Select(ss => ss.SwapVertices[0]).Distinct().ToList()))
                                {
                                    sb.AppendLine();
                                    sb.AppendLine("there is badness here");
                                    sb.AppendLine();
                                }

                                sb.Append("$\\K_{" + alpha + "" + beta + ",\\infty}(" + listString + "," + groups.OrderBy(gg => gg.Key).Select(gg => gg.Key.GetActiveListIndex(b, _maxPot) + 1).Listify(null) + ")");
                                sb.AppendLine("\\Rightarrow $ " + groups.OrderBy(gg => gg.Key).Select(gg => "$" + GetChildBoardName(b, gg.First()) + "$ (Case " + GetHandledCaseNumber(b, gg.First()) + ")").Listify(null) + ".");
                                sb.AppendLine();

                                if (_permutationLinked[b].Count > 0)
                                {
                                    sb.AppendLine();
                                    sb.AppendLine();
                                    sb.AppendLine("Free by vertex permutation: " + _permutationLinked[b].Select(ppp => "$" + ppp.Item1 + "\\Rightarrow " + ppp.Item2.ToListStringInLexOrder(_maxPot) + "$").Listify());
                                    sb.AppendLine();
                                    sb.AppendLine();
                                }

                                sb.AppendLine("\\bigskip");
                                sb.AppendLine();
                            }
                        }
                        else if (swapCountGroup.Key == 2)
                        {
                            sb.AppendLine("\\bigskip");
                            sb.AppendLine();
                            sb.AppendLine("Each of the following boards can be handled by a single Kempe change.");
                            foreach (var b in swapCountGroup)
                            {
                                Permutation pp;
                                var listString = b.ToListStringInLexOrder(out pp);

                                var treeInfo = Mind.GetWinTreeInfo(b);
                                var alpha = Math.Min(pp[treeInfo.First().Alpha], pp[treeInfo.First().Beta]);
                                var beta = Math.Max(pp[treeInfo.First().Alpha], pp[treeInfo.First().Beta]);
                                var leftover = treeInfo.ToList();

                                if (!CheckPermutationGoodNess(alpha, beta, listString, treeInfo.SelectMany(ss => ss.SwapVertices).Distinct().ToList()))
                                {
                                    sb.AppendLine();
                                    sb.AppendLine("there is badness here");
                                    sb.AppendLine();
                                }

                                while (leftover.Count > 0)
                                {
                                    var commonestSwapper = Enumerable.Range(0, b._stackCount).MaxIndex(v => leftover.Count(bc => bc.SwapVertices.Contains(v)));
                                    var handledAll = leftover.Where(bc => bc.SwapVertices.Contains(commonestSwapper)).ToList();
                                    var handled = handledAll.Distinct(bc => bc.SwapVertices.Count == 1 ? -1 : bc.SwapVertices.Except(commonestSwapper).First()).ToList();

                                    sb.Append("$\\K_{" + alpha + "" + beta + "," + (commonestSwapper.GetActiveListIndex(b, _maxPot) + 1) + "}(" + listString);

                                    var single = handled.FirstOrDefault(bc => bc.SwapVertices.Count == 1);
                                    if (single != null)
                                        sb.Append(",\\infty");

                                    if (handled.Where(bc => bc.SwapVertices.Count > 1).Count() > 0)
                                        sb.Append("," + handled.Where(bc => bc.SwapVertices.Count > 1).OrderBy(bc => bc.SwapVertices.Except(commonestSwapper).First()).Select(bc => bc.SwapVertices.Except(commonestSwapper).First().GetActiveListIndex(b, _maxPot) + 1).Listify(null));
                                    sb.Append(")");

                                    sb.AppendLine("\\Rightarrow $ " + handled.OrderBy(bc => bc.SwapVertices.Count == 1 ? -1 : bc.SwapVertices.Except(commonestSwapper).First()).Select(bc => "$" + GetChildBoardName(b, bc) + "$ (Case " + GetHandledCaseNumber(b, bc) + ")").Listify(null) + ".");
                                    sb.AppendLine();

                                    foreach (var bc in handledAll)
                                        leftover.Remove(bc);
                                }

                                if (_permutationLinked[b].Count > 0)
                                {
                                    sb.AppendLine();
                                    sb.AppendLine();
                                    sb.AppendLine("Free by vertex permutation: " + _permutationLinked[b].Select(ppp => "$" + ppp.Item1 + "\\Rightarrow " + ppp.Item2.ToListStringInLexOrder(_maxPot) + "$").Listify());
                                    sb.AppendLine();
                                    sb.AppendLine();
                                }

                                sb.AppendLine();
                                sb.AppendLine("\\bigskip");
                                sb.AppendLine();
                            }
                        }
                    }
                }
            }

            EndProof(sb);

            return sb.ToString();
        }

        bool CheckPermutationGoodNess(int alpha, int beta, string stacksString, List<int> swapVertices)
        {
            var stacks = stacksString.Split('|').Select(s => s.ToCharArray().Select(c => int.Parse(c.ToString())).ToList()).ToList();

            var colors = new List<int>() { alpha, beta };
            foreach (var v in swapVertices)
            {
                if (colors.IntersectionCount(stacks[v]) != 1)
                {
                    System.Diagnostics.Debugger.Break();
                    return false;
                }
            }

            return true;
        }

        string GetChildBoardName(SuperSlimBoard b, BreakerChoiceInfo bc)
        {
            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
            return childBoard.ToListStringInLexOrder(_maxPot);
        }

        void GeneralizeAllBoards(StringBuilder sb)
        {
            _allBoards = Mind.ColorableBoards.Union(Mind.NonColorableBoards).Union(Mind.BreakerWonBoards).ToList();
            _sequenceComparer = new SequenceGeneralizer<int>.VectorComparer();
            _sequenceGeneralizer = new SequenceGeneralizer<int>(_activeIndices.Count, _possibleListIndices);

            var allBoardsTex = GeneralizeBoards(_allBoards);

            if (Mind.OnlyConsiderNearlyColorableBoards)
                sb.AppendLine("We need to handle all boards that are nearly colorable for edge $e$ up to permutation of colors, so it will suffice to handle the following " + _allBoards.Count + " boards: " + allBoardsTex + ".");
            else
                sb.AppendLine("We need to handle all boards up to permutation of colors, so it will suffice to handle the following " + _allBoards.Count + " boards: " + allBoardsTex + ".");

            if (Mind.BreakerWonBoards.Count > 0)
            {
                sb.AppendLine("Unfortunately, the following " + Mind.BreakerWonBoards.Count +  " boards cannot be handled: " + GeneralizeBoards(Mind.BreakerWonBoards) + ".");
            }

            sb.AppendLine();
        }

        string GeneralizeBoards(List<SuperSlimBoard> boards)
        {
            if (UseWildCards)
            {
                var examples = boards.Select(b => ToListIndices(b)).ToList();
                var nonExamples = Enumerable.Repeat(_possibleListIndices, _activeIndices.Count).CartesianProduct().Select(ll => ll.ToList()).Except(examples.Distinct(_sequenceComparer), _sequenceComparer).ToList();

                var generalized = _sequenceGeneralizer.Generalize(examples, nonExamples, false);
                return generalized.Select(gg => "$" + string.Join("|", gg.Select((_, i) => _.ToTex(_possibleLists, _activeListSizes[i]))) + "$").Listify("and");
            }

            return boards.Select(b => b.ToListStringInLexOrder(_maxPot)).OrderBy(x => x).Select(s => "$" + s + "$").Listify("and");
        }

        List<int> ToListIndices(SuperSlimBoard b)
        {
            var stacks = b.Stacks.Value.Select(l => l.ToSet()).Where(s => s.Count < _maxPot).ToList();
            return stacks.Select(s => _possibleLists.FirstIndex(ss => ss.SequenceEqual(s))).ToList();
        }

        void GeneratePossibleLists()
        {
            var stacks = Mind.ColorableBoards[0].Stacks.Value.Select(l => l.ToSet()).ToList();
            _activeIndices = stacks.IndicesWhere(s => s.Count < _maxPot).ToList();
            _activeListSizes = stacks.Select(s => s.Count).Where(c => c < _maxPot).ToList();

            var pot = Enumerable.Range(0, _maxPot).ToList();
            _possibleLists = _activeListSizes.Distinct().OrderBy(c => c).ToList().SelectMany(c => pot.EnumerateSublists(c)).ToList();
            _possibleListIndices = Enumerable.Range(0, _possibleLists.Count).ToList();
        }

        protected static void BeginProof(StringBuilder sb)
        {
            sb.AppendLine("\\begin{proof}");
        }

        protected static void EndProof(StringBuilder sb)
        {
            sb.AppendLine("\\end{proof}");
        }

        void AddFigure(StringBuilder sb)
        {
            var figureID = "fig:" + Guid.NewGuid().ToString();

            sb.AppendLine("\\begin{figure}");
            sb.AppendLine("\\centering");
            sb.AppendLine(_figureTikz);
            sb.AppendLine("\\caption{Vertices are ordered as labeled.}\\label{" + figureID + "}");
            sb.AppendLine("\\end{figure}");

            sb.AppendLine("\\begin{lem}");
            if (_isWin)
                sb.AppendLine("The graph in Figure \\ref{" + figureID + "} is reducible.");
            else
                sb.AppendLine("The graph in Figure \\ref{" + figureID + "} is not reducible.");
            sb.AppendLine("\\end{lem}");
        }
    }
}

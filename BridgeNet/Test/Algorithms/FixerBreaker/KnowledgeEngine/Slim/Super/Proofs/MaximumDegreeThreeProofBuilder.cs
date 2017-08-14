using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorithms.Utility;
using Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs.MaxDegreeThree;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class MaximumDegreeThreeProofBuilder : PermutationAwareProofBuilder
    {
        string _figureTikz;
        public MaximumDegreeThreeProofBuilder(SuperSlimMind mind, string figureTikz = "")
            : base(mind)
        {
            _figureTikz = figureTikz;
        }

        public override string WriteProof()
        {
            var length = Mind.ColorableBoards[0].ToXYZ().Length;
            var allBoards = Mind.ColorableBoards.Union(Mind.NonColorableBoards).ToList();

            var comparer = new SequenceGeneralizer<int>.VectorComparer();
            var sg = new SequenceGeneralizer<int>(length, new List<int> { 0, 1, 2 });

            var zot2 = allBoards.Select(b => b.To012()).ToList();
            var examples2 = allBoards.Select(b => b.To012()).ToList();

            var nonExamples2 = Enumerable.Repeat(Enumerable.Range(0, 3), length).CartesianProduct().Select(ll => ll.ToList()).Except(zot2.Distinct(comparer), comparer).ToList();

            var generalized2 = sg.Generalize(examples2, nonExamples2, false);
            var allBoardsXYZ = generalized2.Select(gg => "$" + string.Join("", gg.Select(_ => _.ToTex())) + "$").Listify("or");

            var sb = new StringBuilder();
            var figureID = "fig:" + Guid.NewGuid().ToString();

            sb.AppendLine("\\begin{figure}");
            sb.AppendLine("\\centering");
            sb.AppendLine(_figureTikz);
            sb.AppendLine("\\caption{Solid vertices have lists of size 3 and the labeled vertices have lists of size 2.}\\label{" + figureID + "}");
            sb.AppendLine("\\end{figure}");

            sb.AppendLine("\\begin{lem}");
            sb.AppendLine("The graph in Figure \\ref{" + figureID + "} is reducible.");
            sb.AppendLine("\\end{lem}");

            var letters = new List<string>() { "X", "Y", "Z" };
            var stringLength = Mind.ColorableBoards[0].Stacks.Value.Count(ss => ss.PopulationCount() == 2);
            var rng = new Random(DateTime.Now.Millisecond);
            var randomString = "";
            for (int i = 0; i < stringLength; i++)
                randomString += letters[rng.Next(3)];

            var randomString2 = "";
            for (int i = 0; i < stringLength; i++)
                randomString2 += letters[rng.Next(3)];

            var randomString3 = "";
            for (int i = 0; i < stringLength; i++)
                randomString3 += letters[rng.Next(3)];

            sb.Append("\\begin{proof}");
            sb.AppendLine("Let $X = \\{0,1\\}$, $Y = \\{0,2\\}$ and $Z = \\{1,2\\}$. Then with the vertex ordering in Figure \\ref{" + figureID + "}, a string such as " + randomString + ", ");
            sb.AppendLine("represents a possible list assignment on $V(H)$ arising from a $3$-edge-coloring of $G-E(H)$.");
            sb.AppendLine("By an $X$-Kempe change, we mean flipping colors $0$ and $1$ on a two-colored path in $G-E(H)$.  We call such a path an $X$-path. ");
            sb.AppendLine("Any endpoint of an $X$-path in $H$ must end at a $Y$ or $Z$ vertex.  The meanings of $Y$-Kempe change, $Z$-Kempe change, $Y$-path and $Z$-path are analogous.");
            sb.AppendLine("Note that if there are an odd number of $Y$'s and $Z$'s, then at least one $X$-path has only one endpoint in $H$.");
            sb.AppendLine("We use shorthand notation like $\\K_{X, 2}(" + randomString + ",5,6) \\Rightarrow " + randomString2 + "," + randomString3 + "$ (Case 1).");
            sb.AppendLine("This means the $X$-Kempe change on " + randomString + " starting at the second vertex and ending at the fifth and sixth result in boards " + randomString2 + " and " + randomString3 + " respectively and these are handled by Case 1.");
            sb.AppendLine("The $\\infty$ symbol means starting (or ending) outside $H$.");
            sb.AppendLine();

            if (Mind.OnlyConsiderNearlyColorableBoards)
            {
                sb.AppendLine("We need to handle all boards that are nearly colorable for edge $e$ up to permutations of $\\{X,Y,Z\\}$, so it will suffice to handle all boards of the form " + allBoardsXYZ + ".");
            }
            else
            {
                sb.AppendLine("We need to handle all boards up to permutations of $\\{X,Y,Z\\}$, so it will suffice to handle all boards of the form " + allBoardsXYZ + ".");
            }

            sb.AppendLine();
            var wonBoards = new List<SuperSlimBoard>();
            for (int caseNumber = 1; caseNumber <= Cases.Count; caseNumber++)
            {
                var c = Cases[caseNumber - 1];

                var boards = c.Boards;
                List<SuperSlimBoard> thisClaimBoards;
                if (caseNumber > 1)
                    thisClaimBoards = boards.SelectMany(b => new[] { b }.Union(_permutationLinked[b].Select(tup => tup.Item2))).ToList();
                else
                    thisClaimBoards = boards;

                wonBoards.AddRange(thisClaimBoards);

                var zot = thisClaimBoards.Select(b => b.To012()).ToList();
                var examples = thisClaimBoards.Select(b => b.To012()).ToList();
                var nonExamples = Enumerable.Repeat(Enumerable.Range(0, 3), length).CartesianProduct().Select(ll => ll.ToList()).Except(zot.Distinct(comparer), comparer).ToList();

                var generalized = sg.Generalize(examples, nonExamples);
                var boardsXYZ = generalized.Select(gg => "$" + string.Join("", gg.Select(_ => _.ToTex())) + "$").Listify("or");

                sb.AppendLine();
                sb.AppendLine("\\bigskip");
                sb.AppendLine(string.Format("\\case{{{0}}}{{$B$ is one of " + boardsXYZ + ".}}", caseNumber));
                sb.AppendLine();
                sb.AppendLine("\\bigskip");
                if (caseNumber == 1)
                {
                    sb.AppendLine();
                    sb.AppendLine("In all these cases, $H$ is immediately colorable from the lists.");
                }
                else
                {
                    sb.AppendLine();
                    var fixGroups = boards.GroupBy(b =>
                    {
                        var treeInfo = Mind.GetWinTreeInfo(b);
                        var fixLetter = ((long)((1 << treeInfo.First().Alpha) | (1 << treeInfo.First().Beta))).ToXYZ();

                        return fixLetter;
                    });

                    foreach (var fixGroup in fixGroups)
                    {
                        var others = letters.ToList();
                        others.Remove(fixGroup.Key);

                        var swapCountGroups = fixGroup.GroupBy(b => Mind.GetWinTreeInfo(b).Max(ti => ti.SwapVertices.Count)).ToList();
                        foreach (var swapCountGroup in swapCountGroups)
                        {
                            if (swapCountGroup.Key == 1)
                            {
                                foreach (var b in swapCountGroup)
                                {
                                    var treeInfo = Mind.GetWinTreeInfo(b);
                                    var groups = treeInfo.GroupBy(ss => ss.SwapVertices[0]);

                                    sb.Append("$\\K_{" + fixGroup.Key + ",\\infty}(" + b.ToXYZ() + "," + groups.OrderBy(gg => gg.Key).Select(gg => gg.Key.GetXYZIndex(b) + 1).Listify(null) + ")");
                                    sb.AppendLine("\\Rightarrow $" + groups.OrderBy(gg => gg.Key).Select(gg => "$" + GetChildBoardName(b, gg.First()) + "$").Listify(null) + "( Case " + treeInfo.Select(bc => GetHandledCaseNumber(b, bc)).Distinct().OrderBy(xx => xx).Listify() + ").");
                                    sb.AppendLine();

                                    if (_permutationLinked[b].Count > 0)
                                    {
                                        sb.AppendLine();
                                        sb.AppendLine();
                                        sb.AppendLine(_permutationLinked[b].Select(ppp => "$" + ppp.Item1 + "\\Rightarrow " + ppp.Item2.ToXYZ() + "$").Listify());
                                        sb.AppendLine();
                                        sb.AppendLine();
                                    }
                                }
                            }
                            else if (swapCountGroup.Key == 2)
                            {
                                foreach (var b in swapCountGroup)
                                {
                                    var treeInfo = Mind.GetWinTreeInfo(b);
                                    var leftover = treeInfo.ToList();

                                    while (leftover.Count > 0)
                                    {
                                        var commonestSwapper = Enumerable.Range(0, b._stackCount).MaxIndex(v => leftover.Count(bc => bc.SwapVertices.Contains(v)));
                                        var handledAll = leftover.Where(bc => bc.SwapVertices.Contains(commonestSwapper)).ToList();
                                        var handled = handledAll.Distinct(bc => bc.SwapVertices.Count == 1 ? -1 : bc.SwapVertices.Except(commonestSwapper).First()).ToList();

                                        sb.Append("$\\K_{" + fixGroup.Key + "," + (commonestSwapper.GetXYZIndex(b) + 1) + "}(" + b.ToXYZ());

                                        var single = handled.FirstOrDefault(bc => bc.SwapVertices.Count == 1);
                                        if (single != null)
                                            sb.Append(",\\infty");
                                        
                                        if (handled.Where(bc => bc.SwapVertices.Count > 1).Count() > 0)
                                            sb.Append("," + handled.Where(bc => bc.SwapVertices.Count > 1).OrderBy(bc => bc.SwapVertices.Except(commonestSwapper).First()).Select(bc => bc.SwapVertices.Except(commonestSwapper).First().GetXYZIndex(b) + 1).Listify(null));
                                        sb.Append(")");

                                        sb.AppendLine("\\Rightarrow $" + handled.OrderBy(bc => bc.SwapVertices.Count == 1 ? -1 : bc.SwapVertices.Except(commonestSwapper).First()).Select(bc => "$" + GetChildBoardName(b, bc) + "$").Listify(null) + "(Case " + handled.Select(bc => GetHandledCaseNumber(b, bc)).Distinct().OrderBy(xx => xx).Listify() + ").");
                                        sb.AppendLine();

                                        foreach (var bc in handledAll)
                                            leftover.Remove(bc);
                                    }

                                    if (_permutationLinked[b].Count > 0)
                                    {
                                        sb.AppendLine();
                                        sb.AppendLine();
                                        sb.AppendLine(_permutationLinked[b].Select(ppp => "$" + ppp.Item1 + "\\Rightarrow " + ppp.Item2.ToXYZ() + "$").Listify());
                                        sb.AppendLine();
                                        sb.AppendLine();
                                    }
                                }
                            }
                        }
                    }
                }

                sb.AppendLine();
            }

            sb.AppendLine("\\end{proof}");
            return sb.ToString();
        }

        string GetChildBoardName(SuperSlimBoard b, BreakerChoiceInfo bc)
        {
            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
            return childBoard.ToXYZ();
        }
    }
}

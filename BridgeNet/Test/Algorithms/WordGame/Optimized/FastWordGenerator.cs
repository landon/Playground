using BitLevelGeneration;
using Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.WordGame.Optimized
{
    public class FastWordGenerator
    {
        public List<FastWord> GenerateWords(int length)
        {
            var words = TheEnumerationMachine.Enumerate(Enumerable.Repeat(1, length), 3).Select(trace => new FastWord(trace, length)).ToList();
            return words;
        }

        public static class TheEnumerationMachine
        {
            public static IEnumerable<ulong[]> Enumerate(IEnumerable<int> sizes, int potSize)
            {
                var sizesCount = sizes.Count();
                var sizeVector = BitVectors_ulong.ToBitVector(sizes);

                var assignment = new ulong[potSize];

                var r = new List<ulong>[potSize];
                for (int i = 0; i < potSize; i++)
                    r[i] = BitVectors_ulong.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

                ulong c = sizeVector.GreaterThan(r[0]);
                ulong dc = (ulong)(~(c | sizeVector.Zeroes()));
                return Enumerate(sizeVector, assignment, r, 0, 0, c, dc);
            }

            static IEnumerable<ulong[]> Enumerate(List<ulong> sizes, ulong[] assignment, List<ulong>[] r, int i, ulong last, ulong care, ulong dontCare)
            {
                ulong x;

                var g = (ulong)(care & ~last);
                var q = (ulong)(~care & ~dontCare & last);

                if (g > q)
                {
                    var f = g.RightFillToMSB();
                    x = (ulong)((care & f) | (last & ~f));
                }
                else if (q > g)
                {
                    var f = q.RightFillToMSB();
                    var t = (ulong)(~f & last);
                    var y = (ulong)(dontCare & ~f & ~t);
                    if (y == 0)
                        yield break;

                    var y2 = (ulong)(dontCare & t & (y | (y - 1)));

                    x = (ulong)((care & (f >> 1)) | (t & ~y2) | (y & (0 - y)));
                }
                else
                {
                    x = last;
                }

                var end = (ulong)(care | dontCare);
                if (i >= assignment.Length - 1)
                {
                    while (true)
                    {
                        assignment[i] = x;

                        var assignmentCopy = new ulong[assignment.Length];
                        Array.Copy(assignment, assignmentCopy, assignmentCopy.Length);

                        yield return assignmentCopy;

                        if (x == end)
                            break;

                        x = (ulong)(((x - (care + dontCare)) & dontCare) + care);
                    }
                }
                else
                {
                    while (true)
                    {
                        assignment[i] = x;

                        sizes.Decrement(x);
                        var c = sizes.GreaterThan(r[i + 1]);
                        var z = sizes.Zeroes();

                        if ((c & z) == 0)
                        {
                            var dc = (ulong)(~(c | z));
                            foreach (var a in Enumerate(sizes, assignment, r, i + 1, x, c, dc))
                                yield return a;
                        }

                        sizes.Increment(x);

                        if (x == end)
                            break;

                        x = (ulong)(((x - (care + dontCare)) & dontCare) + care);
                    }
                }
            }
        }
    }
}

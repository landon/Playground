using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitLevelGeneration
{
    public class MaximalndependentSetSearching
    {
        public static List<long> GenerateMaximalIndependentSubsets(BitGraph_long g, long set)
        {
            var d = new int[64];

            var n = set.PopulationCountDense();
            var D = new long[n];

            var q = set;
            while (q != 0)
            {
                var bit = q & -q;
                var v = bit.Extract();

                d[v] = g.DegreeInSet(v, set);
                D[d[v]] |= bit;

                q ^= bit;
            }

            var bits = new long[n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (D[j] != 0)
                    {
                        var bit = D[j] & -D[j];
                        bits[n - 1 - i] = bit;

                        var v = bit.Extract();
                        q = g.NeighborsInSet(v, set);

                        while (q != 0)
                        {
                            var b = q & -q;
                            var x = b.Extract();

                            D[d[x]] ^= b;
                            d[x]--;
                            D[d[x]] |= b;

                            q ^= b;
                        }

                        set ^= bit;
                        D[j] ^= bit;
                        break;
                    }
                }
            }

            return GenerateMaximalIndependentSubsets(g, bits);
        }

        static List<long> GenerateMaximalIndependentSubsets(BitGraph_long g, long[] bits)
        {
            var mis = new List<long>(8);
            mis.Add(0L);

            var earlier = 0L;
            for (int j = 0; j < bits.Length; j++)
            {
                var bit = bits[j];
                var v = bit.Extract();

                var count = mis.Count;
                for (int i = 0; i < count; i++)
                {
                    var m = mis[i];
                    var N = g.NeighborsInSet(v, m);
                    if (N == 0)
                    {
                        mis[i] |= bit;
                    }
                    else
                    {
                        var mp = m & ~N;
                        var mpp = mp | bit;
                        var q = N;
                        var l = earlier & ~mpp;

                        while (q != 0)
                        {
                            var b = q & -q;
                            var w = b.Extract();

                            var qq = g.NeighborsInSet(w, l);
                            while (qq != 0)
                            {
                                var bb = qq & -qq;
                                var z = bb.Extract();

                                if (g.NeighborsInSet(z, mpp) == 0)
                                    goto skip;

                                qq ^= bb;
                            }

                            q ^= b;
                        }

                        var X = g.NeighborsInSet(v, earlier);
                        q = X & ~N;

                        while (q != 0)
                        {
                            var b = q & -q;
                            var w = b.Extract();

                            if (g.NeighborsInSet(w, mp) != 0)
                                X ^= b;
                            q ^= b;
                        }

                        if (X != N)
                        {
                            var Z = 0L;
                            while (Z < N)
                            {
                                Z = (Z - X) & X;

                                if ((Z & N) == Z)
                                    continue;

                                if (!g.IsIndependent(Z))
                                    continue;

                                var mm = mp | Z;
                                l = earlier & ~mm;
                                q = N & ~Z;

                                while (q != 0)
                                {
                                    var b = q & -q;
                                    var w = b.Extract();

                                    var qq = g.NeighborsInSet(w, l);
                                    while (qq != 0)
                                    {
                                        var bb = qq & -qq;
                                        var z = bb.Extract();

                                        if (g.NeighborsInSet(z, mm) == 0)
                                            goto notMaximal;

                                        qq ^= bb;
                                    }

                                    q ^= b;
                                }

                                goto skip;
notMaximal: ;
                            }
                        }

                        mis.Add((m | bit) & ~N);
skip: ;
                    }
                }

                earlier |= bit;
            }

            return mis;
        }


        public static long LexicographicallyFirstContaining(BitGraph_long g, long set, long S)
        {
            set ^= S;
            while (set != 0)
            {
                var bit = set & -set;
                var v = bit.Extract();

                if (g.NeighborsInSet(v, S) == 0)
                    S |= bit;

                set ^= bit;
            }

            return S;
        }

        public static long Parent(BitGraph_long g, long set, long LFMIS, long S)
        {
            var a = LFMIS & ~S;
            var bit = a & -a;
            var v = bit.Extract();
            var N = g.NeighborsInSet(v, S);

            return LexicographicallyFirstContaining(g, set, (S | bit) ^ N);
        }

        public static long Later(BitGraph_long g, long set, long LFMIS, int v)
        {
            var q = g.NeighborsInSet(v, set ^ LFMIS);

            var earlier = ((1L << v) - 1) & LFMIS;
            var later = 0L;
            while (set != 0)
            {
                var bit = q & -q;
                var w = bit.Extract();

                if (g.NeighborsInSet(w, earlier) == 0)
                    later |= bit;

                q ^= bit;
            }

            return later;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BitLevelGeneration
{
    public static class Assignments_ulong
    {
        public static List<ulong[]> Generate(IEnumerable<int> sizes, int potSize, int countEstimate = 2048)
        {
            var sizesCount = sizes.Count();
            var sizeVector = BitVectors_ulong.ToBitVector(sizes);

            var assignments = new List<ulong[]>(countEstimate);
            var assignment = new ulong[potSize];

            var r = new List<ulong>[potSize];
            for (int i = 0; i < potSize; i++)
                r[i] = BitVectors_ulong.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

            ulong c = sizeVector.GreaterThan(r[0]);
            ulong dc = (ulong)(~(c | sizeVector.Zeroes()));
            Generate(sizeVector, assignments, assignment, r, 0, 1, c, dc);

            return assignments;
        }

        static void Generate(List<ulong> sizes, List<ulong[]> assignments, ulong[] assignment, List<ulong>[] r, int i, ulong last, ulong care, ulong dontCare)
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
                    return;

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
                    assignments.Add(assignmentCopy);

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
                        Generate(sizes, assignments, assignment, r, i + 1, x, c, dc);
                    }

                    sizes.Increment(x);

                    if (x == end)
                        break;

                    x = (ulong)(((x - (care + dontCare)) & dontCare) + care);
                }
            }
        }

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
            return Enumerate(sizeVector, assignment, r, 0, 1, c, dc);
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
                        foreach(var a in Enumerate(sizes, assignment, r, i + 1, x, c, dc))
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


namespace BitLevelGeneration
{
    public static class Assignments_long
    {
        public static List<long[]> Generate(IEnumerable<int> sizes, int potSize, int countEstimate = 2048)
        {
            var sizesCount = sizes.Count();
            var sizeVector = BitVectors_long.ToBitVector(sizes);

            var assignments = new List<long[]>(countEstimate);
            var assignment = new long[potSize];

            var r = new List<long>[potSize];
            for (int i = 0; i < potSize; i++)
                r[i] = BitVectors_long.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

            long c = sizeVector.GreaterThan(r[0]);
            long dc = (long)(~(c | sizeVector.Zeroes()));
            Generate(sizeVector, assignments, assignment, r, 0, 1, c, dc);

            return assignments;
        }

        static void Generate(List<long> sizes, List<long[]> assignments, long[] assignment, List<long>[] r, int i, long last, long care, long dontCare)
        {
            long x;

            var g = (long)(care & ~last);
            var q = (long)(~care & ~dontCare & last);

            if (g > q)
            {
                var f = g.RightFillToMSB();
                x = (long)((care & f) | (last & ~f));
            }
            else if (q > g)
            {
                var f = q.RightFillToMSB();
                var t = (long)(~f & last);
                var y = (long)(dontCare & ~f & ~t);
                if (y == 0)
                    return;

                var y2 = (long)(dontCare & t & (y | (y - 1)));

                x = (long)((care & (f >> 1)) | (t & ~y2) | (y & (0 - y)));
            }
            else
            {
                x = last;
            }

            var end = (long)(care | dontCare);
            if (i >= assignment.Length - 1)
            {
                while (true)
                {
                    assignment[i] = x;

                    var assignmentCopy = new long[assignment.Length];
					Array.Copy(assignment, assignmentCopy, assignmentCopy.Length);
                    assignments.Add(assignmentCopy);

                    if (x == end)
                        break;

                    x = (long)(((x - (care + dontCare)) & dontCare) + care);
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
                        var dc = (long)(~(c | z));
                        Generate(sizes, assignments, assignment, r, i + 1, x, c, dc);
                    }

                    sizes.Increment(x);

                    if (x == end)
                        break;

                    x = (long)(((x - (care + dontCare)) & dontCare) + care);
                }
            }
        }

		public static IEnumerable<long[]> Enumerate(IEnumerable<int> sizes, int potSize)
        {
            var sizesCount = sizes.Count();
            var sizeVector = BitVectors_long.ToBitVector(sizes);

            var assignment = new long[potSize];

            var r = new List<long>[potSize];
            for (int i = 0; i < potSize; i++)
                r[i] = BitVectors_long.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

            long c = sizeVector.GreaterThan(r[0]);
            long dc = (long)(~(c | sizeVector.Zeroes()));
            return Enumerate(sizeVector, assignment, r, 0, 1, c, dc);
        }

        static IEnumerable<long[]> Enumerate(List<long> sizes, long[] assignment, List<long>[] r, int i, long last, long care, long dontCare)
        {
            long x;

            var g = (long)(care & ~last);
            var q = (long)(~care & ~dontCare & last);

            if (g > q)
            {
                var f = g.RightFillToMSB();
                x = (long)((care & f) | (last & ~f));
            }
            else if (q > g)
            {
                var f = q.RightFillToMSB();
                var t = (long)(~f & last);
                var y = (long)(dontCare & ~f & ~t);
                if (y == 0)
                    yield break;

                var y2 = (long)(dontCare & t & (y | (y - 1)));

                x = (long)((care & (f >> 1)) | (t & ~y2) | (y & (0 - y)));
            }
            else
            {
                x = last;
            }

            var end = (long)(care | dontCare);
            if (i >= assignment.Length - 1)
            {
                while (true)
                {
                    assignment[i] = x;

                    var assignmentCopy = new long[assignment.Length];
					Array.Copy(assignment, assignmentCopy, assignmentCopy.Length);
                    
					yield return assignmentCopy;

                    if (x == end)
                        break;

                    x = (long)(((x - (care + dontCare)) & dontCare) + care);
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
                        var dc = (long)(~(c | z));
                        foreach(var a in Enumerate(sizes, assignment, r, i + 1, x, c, dc))
							yield return a;
                    }

                    sizes.Increment(x);

                    if (x == end)
                        break;

                    x = (long)(((x - (care + dontCare)) & dontCare) + care);
                }
            }
        }
    }
}


namespace BitLevelGeneration
{
    public static class Assignments_uint
    {
        public static List<uint[]> Generate(IEnumerable<int> sizes, int potSize, int countEstimate = 2048)
        {
            var sizesCount = sizes.Count();
            var sizeVector = BitVectors_uint.ToBitVector(sizes);

            var assignments = new List<uint[]>(countEstimate);
            var assignment = new uint[potSize];

            var r = new List<uint>[potSize];
            for (int i = 0; i < potSize; i++)
                r[i] = BitVectors_uint.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

            uint c = sizeVector.GreaterThan(r[0]);
            uint dc = (uint)(~(c | sizeVector.Zeroes()));
            Generate(sizeVector, assignments, assignment, r, 0, 1, c, dc);

            return assignments;
        }

        static void Generate(List<uint> sizes, List<uint[]> assignments, uint[] assignment, List<uint>[] r, int i, uint last, uint care, uint dontCare)
        {
            uint x;

            var g = (uint)(care & ~last);
            var q = (uint)(~care & ~dontCare & last);

            if (g > q)
            {
                var f = g.RightFillToMSB();
                x = (uint)((care & f) | (last & ~f));
            }
            else if (q > g)
            {
                var f = q.RightFillToMSB();
                var t = (uint)(~f & last);
                var y = (uint)(dontCare & ~f & ~t);
                if (y == 0)
                    return;

                var y2 = (uint)(dontCare & t & (y | (y - 1)));

                x = (uint)((care & (f >> 1)) | (t & ~y2) | (y & (0 - y)));
            }
            else
            {
                x = last;
            }

            var end = (uint)(care | dontCare);
            if (i >= assignment.Length - 1)
            {
                while (true)
                {
                    assignment[i] = x;

                    var assignmentCopy = new uint[assignment.Length];
					Array.Copy(assignment, assignmentCopy, assignmentCopy.Length);
                    assignments.Add(assignmentCopy);

                    if (x == end)
                        break;

                    x = (uint)(((x - (care + dontCare)) & dontCare) + care);
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
                        var dc = (uint)(~(c | z));
                        Generate(sizes, assignments, assignment, r, i + 1, x, c, dc);
                    }

                    sizes.Increment(x);

                    if (x == end)
                        break;

                    x = (uint)(((x - (care + dontCare)) & dontCare) + care);
                }
            }
        }

		public static IEnumerable<uint[]> Enumerate(IEnumerable<int> sizes, int potSize)
        {
            var sizesCount = sizes.Count();
            var sizeVector = BitVectors_uint.ToBitVector(sizes);

            var assignment = new uint[potSize];

            var r = new List<uint>[potSize];
            for (int i = 0; i < potSize; i++)
                r[i] = BitVectors_uint.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

            uint c = sizeVector.GreaterThan(r[0]);
            uint dc = (uint)(~(c | sizeVector.Zeroes()));
            return Enumerate(sizeVector, assignment, r, 0, 1, c, dc);
        }

        static IEnumerable<uint[]> Enumerate(List<uint> sizes, uint[] assignment, List<uint>[] r, int i, uint last, uint care, uint dontCare)
        {
            uint x;

            var g = (uint)(care & ~last);
            var q = (uint)(~care & ~dontCare & last);

            if (g > q)
            {
                var f = g.RightFillToMSB();
                x = (uint)((care & f) | (last & ~f));
            }
            else if (q > g)
            {
                var f = q.RightFillToMSB();
                var t = (uint)(~f & last);
                var y = (uint)(dontCare & ~f & ~t);
                if (y == 0)
                    yield break;

                var y2 = (uint)(dontCare & t & (y | (y - 1)));

                x = (uint)((care & (f >> 1)) | (t & ~y2) | (y & (0 - y)));
            }
            else
            {
                x = last;
            }

            var end = (uint)(care | dontCare);
            if (i >= assignment.Length - 1)
            {
                while (true)
                {
                    assignment[i] = x;

                    var assignmentCopy = new uint[assignment.Length];
					Array.Copy(assignment, assignmentCopy, assignmentCopy.Length);
                    
					yield return assignmentCopy;

                    if (x == end)
                        break;

                    x = (uint)(((x - (care + dontCare)) & dontCare) + care);
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
                        var dc = (uint)(~(c | z));
                        foreach(var a in Enumerate(sizes, assignment, r, i + 1, x, c, dc))
							yield return a;
                    }

                    sizes.Increment(x);

                    if (x == end)
                        break;

                    x = (uint)(((x - (care + dontCare)) & dontCare) + care);
                }
            }
        }
    }
}


namespace BitLevelGeneration
{
    public static class Assignments_ushort
    {
        public static List<ushort[]> Generate(IEnumerable<int> sizes, int potSize, int countEstimate = 2048)
        {
            var sizesCount = sizes.Count();
            var sizeVector = BitVectors_ushort.ToBitVector(sizes);

            var assignments = new List<ushort[]>(countEstimate);
            var assignment = new ushort[potSize];

            var r = new List<ushort>[potSize];
            for (int i = 0; i < potSize; i++)
                r[i] = BitVectors_ushort.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

            ushort c = sizeVector.GreaterThan(r[0]);
            ushort dc = (ushort)(~(c | sizeVector.Zeroes()));
            Generate(sizeVector, assignments, assignment, r, 0, 1, c, dc);

            return assignments;
        }

        static void Generate(List<ushort> sizes, List<ushort[]> assignments, ushort[] assignment, List<ushort>[] r, int i, ushort last, ushort care, ushort dontCare)
        {
            ushort x;

            var g = (ushort)(care & ~last);
            var q = (ushort)(~care & ~dontCare & last);

            if (g > q)
            {
                var f = g.RightFillToMSB();
                x = (ushort)((care & f) | (last & ~f));
            }
            else if (q > g)
            {
                var f = q.RightFillToMSB();
                var t = (ushort)(~f & last);
                var y = (ushort)(dontCare & ~f & ~t);
                if (y == 0)
                    return;

                var y2 = (ushort)(dontCare & t & (y | (y - 1)));

                x = (ushort)((care & (f >> 1)) | (t & ~y2) | (y & (0 - y)));
            }
            else
            {
                x = last;
            }

            var end = (ushort)(care | dontCare);
            if (i >= assignment.Length - 1)
            {
                while (true)
                {
                    assignment[i] = x;

                    var assignmentCopy = new ushort[assignment.Length];
					Array.Copy(assignment, assignmentCopy, assignmentCopy.Length);
                    assignments.Add(assignmentCopy);

                    if (x == end)
                        break;

                    x = (ushort)(((x - (care + dontCare)) & dontCare) + care);
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
                        var dc = (ushort)(~(c | z));
                        Generate(sizes, assignments, assignment, r, i + 1, x, c, dc);
                    }

                    sizes.Increment(x);

                    if (x == end)
                        break;

                    x = (ushort)(((x - (care + dontCare)) & dontCare) + care);
                }
            }
        }

		public static IEnumerable<ushort[]> Enumerate(IEnumerable<int> sizes, int potSize)
        {
            var sizesCount = sizes.Count();
            var sizeVector = BitVectors_ushort.ToBitVector(sizes);

            var assignment = new ushort[potSize];

            var r = new List<ushort>[potSize];
            for (int i = 0; i < potSize; i++)
                r[i] = BitVectors_ushort.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

            ushort c = sizeVector.GreaterThan(r[0]);
            ushort dc = (ushort)(~(c | sizeVector.Zeroes()));
            return Enumerate(sizeVector, assignment, r, 0, 1, c, dc);
        }

        static IEnumerable<ushort[]> Enumerate(List<ushort> sizes, ushort[] assignment, List<ushort>[] r, int i, ushort last, ushort care, ushort dontCare)
        {
            ushort x;

            var g = (ushort)(care & ~last);
            var q = (ushort)(~care & ~dontCare & last);

            if (g > q)
            {
                var f = g.RightFillToMSB();
                x = (ushort)((care & f) | (last & ~f));
            }
            else if (q > g)
            {
                var f = q.RightFillToMSB();
                var t = (ushort)(~f & last);
                var y = (ushort)(dontCare & ~f & ~t);
                if (y == 0)
                    yield break;

                var y2 = (ushort)(dontCare & t & (y | (y - 1)));

                x = (ushort)((care & (f >> 1)) | (t & ~y2) | (y & (0 - y)));
            }
            else
            {
                x = last;
            }

            var end = (ushort)(care | dontCare);
            if (i >= assignment.Length - 1)
            {
                while (true)
                {
                    assignment[i] = x;

                    var assignmentCopy = new ushort[assignment.Length];
					Array.Copy(assignment, assignmentCopy, assignmentCopy.Length);
                    
					yield return assignmentCopy;

                    if (x == end)
                        break;

                    x = (ushort)(((x - (care + dontCare)) & dontCare) + care);
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
                        var dc = (ushort)(~(c | z));
                        foreach(var a in Enumerate(sizes, assignment, r, i + 1, x, c, dc))
							yield return a;
                    }

                    sizes.Increment(x);

                    if (x == end)
                        break;

                    x = (ushort)(((x - (care + dontCare)) & dontCare) + care);
                }
            }
        }
    }
}


namespace BitLevelGeneration
{
    public static class Assignments_byte
    {
        public static List<byte[]> Generate(IEnumerable<int> sizes, int potSize, int countEstimate = 2048)
        {
            var sizesCount = sizes.Count();
            var sizeVector = BitVectors_byte.ToBitVector(sizes);

            var assignments = new List<byte[]>(countEstimate);
            var assignment = new byte[potSize];

            var r = new List<byte>[potSize];
            for (int i = 0; i < potSize; i++)
                r[i] = BitVectors_byte.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

            byte c = sizeVector.GreaterThan(r[0]);
            byte dc = (byte)(~(c | sizeVector.Zeroes()));
            Generate(sizeVector, assignments, assignment, r, 0, 1, c, dc);

            return assignments;
        }

        static void Generate(List<byte> sizes, List<byte[]> assignments, byte[] assignment, List<byte>[] r, int i, byte last, byte care, byte dontCare)
        {
            byte x;

            var g = (byte)(care & ~last);
            var q = (byte)(~care & ~dontCare & last);

            if (g > q)
            {
                var f = g.RightFillToMSB();
                x = (byte)((care & f) | (last & ~f));
            }
            else if (q > g)
            {
                var f = q.RightFillToMSB();
                var t = (byte)(~f & last);
                var y = (byte)(dontCare & ~f & ~t);
                if (y == 0)
                    return;

                var y2 = (byte)(dontCare & t & (y | (y - 1)));

                x = (byte)((care & (f >> 1)) | (t & ~y2) | (y & (0 - y)));
            }
            else
            {
                x = last;
            }

            var end = (byte)(care | dontCare);
            if (i >= assignment.Length - 1)
            {
                while (true)
                {
                    assignment[i] = x;

                    var assignmentCopy = new byte[assignment.Length];
					Array.Copy(assignment, assignmentCopy, assignmentCopy.Length);
                    assignments.Add(assignmentCopy);

                    if (x == end)
                        break;

                    x = (byte)(((x - (care + dontCare)) & dontCare) + care);
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
                        var dc = (byte)(~(c | z));
                        Generate(sizes, assignments, assignment, r, i + 1, x, c, dc);
                    }

                    sizes.Increment(x);

                    if (x == end)
                        break;

                    x = (byte)(((x - (care + dontCare)) & dontCare) + care);
                }
            }
        }

		public static IEnumerable<byte[]> Enumerate(IEnumerable<int> sizes, int potSize)
        {
            var sizesCount = sizes.Count();
            var sizeVector = BitVectors_byte.ToBitVector(sizes);

            var assignment = new byte[potSize];

            var r = new List<byte>[potSize];
            for (int i = 0; i < potSize; i++)
                r[i] = BitVectors_byte.ToBitVector(Enumerable.Repeat(potSize - 1 - i, sizesCount));

            byte c = sizeVector.GreaterThan(r[0]);
            byte dc = (byte)(~(c | sizeVector.Zeroes()));
            return Enumerate(sizeVector, assignment, r, 0, 1, c, dc);
        }

        static IEnumerable<byte[]> Enumerate(List<byte> sizes, byte[] assignment, List<byte>[] r, int i, byte last, byte care, byte dontCare)
        {
            byte x;

            var g = (byte)(care & ~last);
            var q = (byte)(~care & ~dontCare & last);

            if (g > q)
            {
                var f = g.RightFillToMSB();
                x = (byte)((care & f) | (last & ~f));
            }
            else if (q > g)
            {
                var f = q.RightFillToMSB();
                var t = (byte)(~f & last);
                var y = (byte)(dontCare & ~f & ~t);
                if (y == 0)
                    yield break;

                var y2 = (byte)(dontCare & t & (y | (y - 1)));

                x = (byte)((care & (f >> 1)) | (t & ~y2) | (y & (0 - y)));
            }
            else
            {
                x = last;
            }

            var end = (byte)(care | dontCare);
            if (i >= assignment.Length - 1)
            {
                while (true)
                {
                    assignment[i] = x;

                    var assignmentCopy = new byte[assignment.Length];
					Array.Copy(assignment, assignmentCopy, assignmentCopy.Length);
                    
					yield return assignmentCopy;

                    if (x == end)
                        break;

                    x = (byte)(((x - (care + dontCare)) & dontCare) + care);
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
                        var dc = (byte)(~(c | z));
                        foreach(var a in Enumerate(sizes, assignment, r, i + 1, x, c, dc))
							yield return a;
                    }

                    sizes.Increment(x);

                    if (x == end)
                        break;

                    x = (byte)(((x - (care + dontCare)) & dontCare) + care);
                }
            }
        }
    }
}



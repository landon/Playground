using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitLevelGeneration
{
    public interface IGraph_uint
    {
        int N { get; }
        IEnumerable<int> Vertices { get; }
        bool IsIndependent(uint set);
        int DegreeInSet(int v, uint set);
        uint NeighborsInSet(int v, uint set);
        int Degree(int v);
        IEnumerable<uint> MaximalIndependentSubsets(uint set);
    }

    public interface IGraph_long
    {
        int N { get; }
        IEnumerable<int> Vertices { get; }
        bool IsIndependent(long set);
        int DegreeInSet(int v, long set);
        long NeighborsInSet(int v, long set);
        int Degree(int v);
        IEnumerable<long> MaximalIndependentSubsets(long set);
    }
}

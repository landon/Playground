using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability
{
    public interface IGraph<TList>
    {
        int N { get; }
        bool AreAdjacent(int v, int w);
        void Initialize(bool[,] adjacent);
        bool IsChoosable(Func<int, TList> getList);
    }
}

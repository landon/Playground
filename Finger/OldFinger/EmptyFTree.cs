using System;
using System.Collections.Generic;
using System.Text;

namespace FingerStandard
{
    public class EmptyFTree<T> : FTree<T>
    {
        public EmptyFTree() { }

        public override FTree<T> Push_Front(T t)
        {
            return new SingleFTree<T>(t);
        }

        public override FTree<T> Push_Back(T t)
        {
            return new SingleFTree<T>(t);
        }

        public override IEnumerable<T> ToSequence()
        {
            return new List<T>();
        }

        public override IEnumerable<T> ToSequenceR()
        {
            return new List<T>();
        }

        public override ViewL<T> LeftView()
        {
            return null;
        }

        public override ViewR<T> RightView()
        {
            return null;
        }

        public override FTree<T> App2(List<T> ts, FTree<T> rightFT)
        {
            var resultFT = rightFT;

            for (int i = ts.Count - 1; i >= 0; i--)
            {
                resultFT = resultFT.Push_Front(ts[i]);
            }

            return resultFT;
        }

        public override FTree<T> Merge(FTree<T> rightFT)
        {
            return rightFT;
        }
    }
}

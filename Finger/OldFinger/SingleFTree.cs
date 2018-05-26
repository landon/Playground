using System;
using System.Collections.Generic;
using System.Text;

namespace FingerStandard
{
    public class SingleFTree<T> : FTree<T>
    {
        protected T theSingle;
        public SingleFTree(T t)
        {
            theSingle = t;
        }

        public override FTree<T> Push_Front(T t)
        {
            return new DeepFTree<T>(new Digit<T>(t), new EmptyFTree<Node<T>>(), new Digit<T>(theSingle));
        }

        public override FTree<T> Push_Back(T t)
        {
            return new DeepFTree<T>(new Digit<T>(theSingle), new EmptyFTree<Node<T>>(), new Digit<T>(t));
        }

        public override IEnumerable<T> ToSequence()
        {
            var newL = new List<T>();
            newL.Add(theSingle);
            return newL;
        }

        public override IEnumerable<T> ToSequenceR()
        {
            var newR = new List<T>();
            newR.Add(theSingle);
            return newR;
        }

        public override ViewL<T> LeftView()
        {
            return new ViewL<T>(theSingle, new EmptyFTree<T>());
        }

        public override ViewR<T> RightView()
        {
            return new ViewR<T>(new EmptyFTree<T>(), theSingle);
        }

        public override FTree<T> App2(List<T> ts, FTree<T> rightFT)
        {
            var resultFT = rightFT;

            for (int i = ts.Count - 1; i >= 0; i--)
            {
                resultFT = resultFT.Push_Front(ts[i]);
            }

            return resultFT.Push_Front(theSingle);
        }

        public override FTree<T> Merge(FTree<T> rightFT)
        {
            return rightFT.Push_Front(theSingle);
        }
    }
}

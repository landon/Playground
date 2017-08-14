using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class GameTree : Tree<GameTree>
    {
        public SuperSlimBoard Board { get; set; }
        public bool IsColorable { get; set; }
        public bool IsSuperabundant { get; set; }
        public bool IsFixerWin { get; set; }
        public int GameTreeIndex { get; set; }
        public int SameAsIndex { get; set; }
        public BreakerChoiceInfo Info {get; set;}

        public void AddChild(GameTree tree, BreakerChoiceInfo info)
        {
            tree.Info = info;
            AddChild(tree);
        }

        public override bool Equals(object obj)
        {
            var gg = obj as GameTree;
            if (gg == null)
                return false;

            return gg.Board.Equals(Board);
        }

        public override int GetHashCode()
        {
            return Board.GetHashCode();
        }
    }
}

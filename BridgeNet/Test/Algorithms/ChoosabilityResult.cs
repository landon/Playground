using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class ChoosabilityResult
    {
        public List<List<int>> BadAssignment
        {
            get;
            private set;
        }

        public bool Canceled
        {
            get;
            private set;
        }

        public bool IsChoosable
        {
            get { return BadAssignment == null; }
        }

        public ChoosabilityResult(List<List<int>> badAssignment, bool canceled = false)
        {
            BadAssignment = badAssignment;
            Canceled = canceled;
        }
    }

    public class OrientationResult
    {
        public Graph Graph { get; set; }
        public int Even { get; set; }
        public int Odd { get; set; }
    }
}

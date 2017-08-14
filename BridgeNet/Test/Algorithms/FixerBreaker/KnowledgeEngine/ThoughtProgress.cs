using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class ThoughtProgress
    {
        public bool IsInitialThought { get; set; }
        public List<Board> BoardsAdded { get; set; }
        public List<Board> BoardsRemoved { get; set; }
        public int WinLength { get; set; }
    }
}


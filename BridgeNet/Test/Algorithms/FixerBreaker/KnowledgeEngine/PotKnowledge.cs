using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class PotKnowledge
    {
        Dictionary<Board, BoardKnowledge> _boardLookup = new Dictionary<Board, BoardKnowledge>();
        Dictionary<string, Dictionary<Board, BoardKnowledge>> _improvementLookup = new Dictionary<string, Dictionary<Board, BoardKnowledge>>();
        HashSet<Board> _lostBoards = new HashSet<Board>();

        public int ColorCount { get; set; }
        public List<Tuple<int, int>> ColorPairs { get; private set; }
        public IEnumerable<Board> LostBoards { get { return _lostBoards; } }

        public BoardKnowledge this[Board board] 
        { 
            get 
            {
                BoardKnowledge b;
                _boardLookup.TryGetValue(board, out b);
                return b;
            } 
        }

        Dictionary<Board, BoardKnowledge> ImprovementLookup(string reason)
        {
            Dictionary<Board, BoardKnowledge> lookup;
            if (!_improvementLookup.TryGetValue(reason, out lookup))
            {
                lookup = new Dictionary<Board, BoardKnowledge>();
                _improvementLookup[reason] = lookup;
            }

            return lookup;
        }
        public BoardKnowledge this[string reason, Board board]
        {
            get
            {
                BoardKnowledge b;
                ImprovementLookup(reason).TryGetValue(board, out b);
                return b;
            }
            set
            {
                ImprovementLookup(reason)[board] = value;
            }
        }

        public PotKnowledge(int colorCount)
        {
            ColorCount = colorCount;
            SetupColorPairs();
        }

        public BoardKnowledge AddWin(Board board, string reason)
        {
            var knowledge = new BoardKnowledge(board, reason);
            _boardLookup[board] = knowledge;

            return knowledge;
        }

        public BoardKnowledge AddWin(Board board, string reason, int winDepth, Tuple<int, int> colorPair, List<Tuple<List<List<int>>, List<Move>, string>> goodSwaps)
        {
            var knowledge = new BoardKnowledge(board, reason) { Depth = winDepth, ColorPair = colorPair, Swaps = goodSwaps };
            _boardLookup[board] = knowledge;

            return knowledge;
        }

        public BoardKnowledge AddImprovement(Board board, string reason, int winDepth, Tuple<int, int> colorPair, List<Tuple<List<List<int>>, List<Move>, string>> goodSwaps)
        {
            var knowledge = new BoardKnowledge(board, reason) { Depth = winDepth, ColorPair = colorPair, Swaps = goodSwaps };
            this[reason, board] = knowledge;

            return knowledge;
        }

        public void AddLoss(Board b)
        {
            _lostBoards.Add(b);
        }

        void SetupColorPairs()
        {
            ColorPairs = new List<Tuple<int, int>>();
            var colors = Enumerable.Range(0, ColorCount).ToList();

            for (int i = 0; i < colors.Count; i++)
                for (int j = i + 1; j < colors.Count; j++)
                    ColorPairs.Add(new Tuple<int, int>(colors[i], colors[j]));
        }

        public void CopyInto(PotKnowledge potKnowledge)
        {
            foreach (var kvp in _boardLookup)
                potKnowledge.AddWin(kvp.Key, kvp.Value.Reason, kvp.Value.Depth, kvp.Value.ColorPair, kvp.Value.Swaps);
        }

        public IEnumerable<KeyValuePair<Board, BoardKnowledge>> EnumerateBoardKnowledge()
        { 
            return _boardLookup.OrderByDescending(x => x.Value.Depth);
        }
    }
}

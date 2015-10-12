using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsOnALineGame
{
    class DynamicProgramming
    {
        const Int64 WinPattern = 45;

        int _n;
        uint _mask;
        HashSet<State> _wonStates = new HashSet<State>();
        List<State> _remainingStates = new List<State>();

        public DynamicProgramming(int n)
        {
            _n = n;
            _mask = (uint)((1 << _n) - 1);
        }

        public bool Solve()
        {
            GenerateStates();

            while (true)
            {
                var count = _remainingStates.Count;

                for (int i = _remainingStates.Count - 1; i >= 0; i--)
                {
                    var s = _remainingStates[i];
                    if (IsOneStepFromWin(s))
                    {
                        _wonStates.Add(s);
                        _remainingStates.RemoveAt(i);
                    }
                }

                if (_remainingStates.Count <= 0)
                    return true;

                if (count == _remainingStates.Count)
                    return false;
            }
        }

        bool IsOneStepFromWin(State s)
        {
            var moves = ~s.Red & ~s.Green & _mask;
            while (moves != 0)
            {
                var b = (uint)(moves & (0 - moves));

                var red = s.Red | b;
                var responses = ~red & ~s.Green & _mask;
                var winsAll = true;
                while (responses != 0)
                {
                    var rb = (uint)(responses & (0 - responses));

                    var ss = new State(red, s.Green | rb);
                    if (!IsWin(ss))
                    {
                        winsAll = false;
                        break;
                    }

                    responses ^= rb;
                }

                if (winsAll)
                    return true;

                moves ^= b;
            }

            return false;
        }

        void GenerateStates()
        {
            _remainingStates.Add(new State(0, 0));

            var s = new List<uint>();
            for (int i = 1; i <= _n / 2; i++)
            {
                s.Clear();
                SubsetsOfSize(i, s);

                foreach (var a in s)
                {
                    foreach (var b in s)
                    {
                        if ((a & b) != 0)
                            continue;

                        var ss = new State(a, b);
                        

                        if (IsWin(ss))
                            _wonStates.Add(ss);
                        else
                            _remainingStates.Add(ss);
                    }
                }
            }
        }

        void SubsetsOfSize(int k, List<uint> s)
        {
            var u = 1 << _n;

            var c = (1 << k) - 1;
            while (c < u)
            {
                s.Add((uint)c);

                var a = c & -c;
                var b = c + a;
                c = (((c ^ b) >> 2) / a) | b;
            }
        }

        static bool IsWin(State ss)
        {
            var red = ss.Red;
            while (red != 0)
            {
                var lsb = red.LeastSignificantBit();
                red = red >> lsb;

                if ((red & WinPattern) == WinPattern)
                    return true;

                red = red >> 1;
            }

            return false;
        }

        struct State : IEquatable<State>
        {
            public uint Red;
            public uint Green;

            public State(uint red, uint green)
            {
                Red = red;
                Green = green;
            }
        
            public bool Equals(State other)
            {
                return Red == other.Red && Green == other.Green;
            }
        }
    }
}

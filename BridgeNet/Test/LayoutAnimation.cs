using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class LayoutAnimation
    {
        const int Steps = 100;

        Action _update;
        Action _finalUpdate;
        List<Graphs.Vector> _layout;
        double[] _xstep;
        double[] _ystep;
        int _step = 0;
        Graphs.Graph _G;

        public LayoutAnimation(Action update, Action finalUpdate, List<Graphs.Vector> layout, Graphs.Graph G)
        {
            _G = G;
            _update = update;
            _finalUpdate = finalUpdate;
            _layout = layout;

            try
            {
                _xstep = new double[_layout.Count];
                _ystep = new double[_layout.Count];
                for (int i = 0; i < _layout.Count; i++)
                {
                    _xstep[i] = (_layout[i].X - _G.Vertices[i].X) / Steps;
                    _ystep[i] = (_layout[i].Y - _G.Vertices[i].Y) / Steps;
                }
            }
            catch { }
        }

        public async Task Animate()
        {
            while(_step < Steps)
            {
                DoStep();
                await Task.Delay(10);
            }
        }

        void DoStep()
        {
            for (int i = 0; i < _layout.Count; i++)
            {
                _G.Vertices[i].X += _xstep[i];
                _G.Vertices[i].Y += _ystep[i];
            }

            _update();
            _step++;
            if (_step >= Steps)
                OnFinish();
        }

        void OnFinish()
        {
            for (int i = 0; i < _layout.Count; i++)
            {
                _G.Vertices[i].X = _layout[i].X;
                _G.Vertices[i].Y = _layout[i].Y;
            }
            _finalUpdate();
        }
    }
}

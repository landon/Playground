using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearFractionalProgram
{
    class Matrix
    {
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public double this[int row, int column] { get { return _data[row, column]; } }

        double[,] _data;

        public Matrix(int columns, params double[] entries)
        {
            Columns = columns;
            Rows = entries.Length / Columns;
            _data = new double[Rows, Columns];

            for (int i = 0; i < entries.Length; i++)
                _data[i / Columns, i % Columns] = entries[i];
        }
    }
}

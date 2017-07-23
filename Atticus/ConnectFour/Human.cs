using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectFour
{
    class Human : Player
    {
        static string Decimals = "0123456789";
        public override int SelectMove(Board b, int lastColumn, int color)
        {
            while (true)
            {
                var d = Console.ReadKey(true);
                if (char.IsDigit(d.KeyChar))
                {
                    var column = Decimals.IndexOf(d.KeyChar);
                    if (column < 7)
                    {
                        var m = new Move(column, color);
                        if (b.IsLegalMove(m))
                        {
                            return m.Column;
                        }
                    }
                }
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangleTiling
{
    class Program
    {
        static void Main(string[] args)
        {
            var coordinator = new Coordinator(10, 10);
           
            foreach(var x in "ABCDEF")
            {
                for(int i = 1; i <= 12; i++)
                {
                    var label = x.ToString() + i;
                    var triangleCoordinates = coordinator.LabelToSpaceCoordinates(label);
                    var computedLabel = coordinator.SpaceCoordinatesToLabel(triangleCoordinates);

                    if (label != computedLabel)
                        throw new Exception("the coordinator is broken");

                    Console.WriteLine(label + " ==> " + string.Join(",", triangleCoordinates.Select(p => WritePoint(p))) + " ==> " + computedLabel);
                }
            }

            Console.ReadKey();
        }

        static string WritePoint(Tuple<double, double> p)
        {
            return string.Format("({0},{1})", p.Item1, p.Item2);
        }
    }

// output:
//A1 ==> (0,0),(10,10),(0,10) ==> A1
//A2 ==> (0,0),(10,10),(10,0) ==> A2
//A3 ==> (10,0),(20,10),(10,10) ==> A3
//A4 ==> (10,0),(20,10),(20,0) ==> A4
//A5 ==> (20,0),(30,10),(20,10) ==> A5
//A6 ==> (20,0),(30,10),(30,0) ==> A6
//A7 ==> (30,0),(40,10),(30,10) ==> A7
//A8 ==> (30,0),(40,10),(40,0) ==> A8
//A9 ==> (40,0),(50,10),(40,10) ==> A9
//A10 ==> (40,0),(50,10),(50,0) ==> A10
//A11 ==> (50,0),(60,10),(50,10) ==> A11
//A12 ==> (50,0),(60,10),(60,0) ==> A12
//B1 ==> (0,10),(10,20),(0,20) ==> B1
//B2 ==> (0,10),(10,20),(10,10) ==> B2
//B3 ==> (10,10),(20,20),(10,20) ==> B3
//B4 ==> (10,10),(20,20),(20,10) ==> B4
//B5 ==> (20,10),(30,20),(20,20) ==> B5
//B6 ==> (20,10),(30,20),(30,10) ==> B6
//B7 ==> (30,10),(40,20),(30,20) ==> B7
//B8 ==> (30,10),(40,20),(40,10) ==> B8
//B9 ==> (40,10),(50,20),(40,20) ==> B9
//B10 ==> (40,10),(50,20),(50,10) ==> B10
//B11 ==> (50,10),(60,20),(50,20) ==> B11
//B12 ==> (50,10),(60,20),(60,10) ==> B12
//C1 ==> (0,20),(10,30),(0,30) ==> C1
//C2 ==> (0,20),(10,30),(10,20) ==> C2
//C3 ==> (10,20),(20,30),(10,30) ==> C3
//C4 ==> (10,20),(20,30),(20,20) ==> C4
//C5 ==> (20,20),(30,30),(20,30) ==> C5
//C6 ==> (20,20),(30,30),(30,20) ==> C6
//C7 ==> (30,20),(40,30),(30,30) ==> C7
//C8 ==> (30,20),(40,30),(40,20) ==> C8
//C9 ==> (40,20),(50,30),(40,30) ==> C9
//C10 ==> (40,20),(50,30),(50,20) ==> C10
//C11 ==> (50,20),(60,30),(50,30) ==> C11
//C12 ==> (50,20),(60,30),(60,20) ==> C12
//D1 ==> (0,30),(10,40),(0,40) ==> D1
//D2 ==> (0,30),(10,40),(10,30) ==> D2
//D3 ==> (10,30),(20,40),(10,40) ==> D3
//D4 ==> (10,30),(20,40),(20,30) ==> D4
//D5 ==> (20,30),(30,40),(20,40) ==> D5
//D6 ==> (20,30),(30,40),(30,30) ==> D6
//D7 ==> (30,30),(40,40),(30,40) ==> D7
//D8 ==> (30,30),(40,40),(40,30) ==> D8
//D9 ==> (40,30),(50,40),(40,40) ==> D9
//D10 ==> (40,30),(50,40),(50,30) ==> D10
//D11 ==> (50,30),(60,40),(50,40) ==> D11
//D12 ==> (50,30),(60,40),(60,30) ==> D12
//E1 ==> (0,40),(10,50),(0,50) ==> E1
//E2 ==> (0,40),(10,50),(10,40) ==> E2
//E3 ==> (10,40),(20,50),(10,50) ==> E3
//E4 ==> (10,40),(20,50),(20,40) ==> E4
//E5 ==> (20,40),(30,50),(20,50) ==> E5
//E6 ==> (20,40),(30,50),(30,40) ==> E6
//E7 ==> (30,40),(40,50),(30,50) ==> E7
//E8 ==> (30,40),(40,50),(40,40) ==> E8
//E9 ==> (40,40),(50,50),(40,50) ==> E9
//E10 ==> (40,40),(50,50),(50,40) ==> E10
//E11 ==> (50,40),(60,50),(50,50) ==> E11
//E12 ==> (50,40),(60,50),(60,40) ==> E12
//F1 ==> (0,50),(10,60),(0,60) ==> F1
//F2 ==> (0,50),(10,60),(10,50) ==> F2
//F3 ==> (10,50),(20,60),(10,60) ==> F3
//F4 ==> (10,50),(20,60),(20,50) ==> F4
//F5 ==> (20,50),(30,60),(20,60) ==> F5
//F6 ==> (20,50),(30,60),(30,50) ==> F6
//F7 ==> (30,50),(40,60),(30,60) ==> F7
//F8 ==> (30,50),(40,60),(40,50) ==> F8
//F9 ==> (40,50),(50,60),(40,60) ==> F9
//F10 ==> (40,50),(50,60),(50,50) ==> F10
//F11 ==> (50,50),(60,60),(50,60) ==> F11
//F12 ==> (50,50),(60,60),(60,50) ==> F12

}

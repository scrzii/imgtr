using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTransformer.Algorithms.Models
{
    public class Line
    {
        public FlatPoint P1 { get; set; }
        public FlatPoint P2 { get; set; }

        public Line(FlatPoint p1, FlatPoint p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public override bool Equals(object obj)
        {
            var line = (Line)obj;
            return P1 == line.P1 && P2 == line.P2 || P2 == line.P1 && P1 == line.P2;
        }

        public override int GetHashCode()
        {
            return P1.GetHashCode() + P2.GetHashCode();
        }
    }
}

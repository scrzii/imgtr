using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTransformer.Algorithms.Models
{
    public class PlanePoint : MDPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PlanePoint(double x, double y, int n) : base(n)
        {
            X = x;
            Y = y;
        }

        public PlanePoint(double x, double y, double[] coords) : base(coords)
        {
            X = x;
            Y = y;
        }

        public static double GetDist(PlanePoint p1, PlanePoint p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
    }
}

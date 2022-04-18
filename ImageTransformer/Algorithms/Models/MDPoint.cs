using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTransformer.Algorithms.Models
{
    public class MDPoint
    {
        public double[] Coords { get; set; }
        public Func<MDPoint, MDPoint, double> Formula { get; set; }
        public int Label { get; set; }

        public MDPoint(double[] coords)
        {
            Coords = coords;
            Formula = StandartFormula;
        }

        public MDPoint(int n)
        {
            Coords = new double[n];
            Formula = StandartFormula;
        }

        public static double StandartFormula(MDPoint p1, MDPoint p2)
        {
            if (p1.Coords.Length != p2.Coords.Length)
            {
                throw new Exception($"Количество измерений сравниваемых точек не совпадает: {p1.Coords.Length} и {p2.Coords.Length}");
            }

            double res = 0;
            for (int i = 0; i < p1.Coords.Length; i++)
            {
                res += Math.Pow(p1.Coords[i] - p2.Coords[i], 2);
            }

            return Math.Sqrt(res);
        }

        public static double operator-(MDPoint p1, MDPoint p2)
        {
            return p1.Formula(p1, p2);
        }
    }
}

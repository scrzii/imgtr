using ImageTransformer.Algorithms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageTransformer.Algorithms
{
    public class KMeans
    {
        public IEnumerable<MDPoint> Data { get; set; } 
        public MDPoint[] Centers { get; set; }

        public KMeans(IEnumerable<MDPoint> data)
        {
            Data = data;
        }

        public void Calculate(int clustersCount, int maxIter = 20)
        {
            Centers = new MDPoint[clustersCount];
            for (int i = 0; i < clustersCount; i++)
            {
                Centers[i] = new MDPoint(Data.First().Coords);
            }

            bool moved = true;
            for (int iter = 0; iter < maxIter; iter++)
            {
                if (!moved && iter > 10)
                {
                    break;
                }
                moved = false;

                foreach (var point in Data)
                {
                    var nearest = GetNearestIndex(point);
                    if (point.Label != nearest)
                    {
                        point.Label = nearest;
                        moved = true;
                    }
                }

                for (int i = 0; i < Centers.Length; i++)
                {
                    var points = Data.Where(_ => _.Label == i).ToList();
                    if (points.Count == 0)
                    {
                        continue;
                    }
                    var coords = new double[Data.First().Coords.Length];
                    for (int dim = 0; dim < coords.Length; dim++)
                    {
                        coords[dim] = 0;
                    }
                    foreach (var point in points)
                    {
                        for (int dim = 0; dim < point.Coords.Length; dim++)
                        {
                            coords[dim] += point.Coords[dim];
                        }
                    }
                    for (int dim = 0; dim < Centers[i].Coords.Length; dim++)
                    {
                        coords[dim] /= points.Count;
                    }
                    Centers[i].Coords = coords;
                }
            }
        }

        private int GetNearestIndex(MDPoint p)
        {
            var result = 0;
            var minDist = Centers[result] - p;
            for (int i = 0; i < Centers.Length; i++)
            {
                if (Centers[i] - p < minDist)
                {
                    minDist = Centers[i] - p;
                    result = i;
                }
            }

            return result;
        }
    }
}

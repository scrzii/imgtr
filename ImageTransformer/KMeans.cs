using ImageTransformer.Algorithms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTransformer.Algorithms
{
    public class KMeans
    {
        public MDPoint[] Data { get; set; } 
        private MDPoint[] Centers { get; set; }

        public KMeans(MDPoint[] data)
        {
            Data = data;
        }

        public void Calculate(int clustersCount, int maxIter = 300)
        {
            Centers = new MDPoint[clustersCount];

            bool moved = true;
            for (int iter = 0; iter < maxIter; iter++)
            {
                if (!moved)
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
                    var coords = new double[Centers.Length];
                    foreach (var point in points)
                    {
                        for (int dim = 0; dim < point.Coords.Length; dim++)
                        {
                            coords[dim] += point.Coords[dim];
                        }
                    }
                    for (int dim = 0; dim < Centers[i].Coords.Length; dim++)
                    {
                        Centers[i].Coords[dim] /= points.Count;
                    }
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

using ImageTransformer.Algorithms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTransformer.Algorithms
{
    public abstract class ClusteringAlgorithm
    {
        public MDPoint[] Data { get; set; }

        public ClusteringAlgorithm(MDPoint[] data)
        {
            Data = data;
        }

    }
}

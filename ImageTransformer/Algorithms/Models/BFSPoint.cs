using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTransformer.Algorithms.Models
{
    public class BFSPoint<T>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public List<int> Cluster { get; set; }
        public List<int> Generation { get; set; }
        public T Value { get; set; }
        public bool Enabled { get; set; } = false;

        public BFSPoint(int x, int y, T value)
        {
            X = x;
            Y = y;
            Value = value;
            Cluster = new List<int>();
            Generation = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                Cluster.Add(-1);
                Generation.Add(-1);
            }
        }
    }
}

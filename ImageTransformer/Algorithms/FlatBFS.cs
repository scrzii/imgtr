using ImageTransformer.Algorithms.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTransformer.Algorithms
{
    public class FlatBFS
    {
        public List<List<BFSPoint<Color>>> Points { get; set; }
        public List<List<List<List<FlatPoint>>>> PointList { get; set; }
        public int Comp { get; set; }

        public FlatBFS(List<List<BFSPoint<Color>>> data)
        {
            Points = data;
            PointList = new List<List<List<List<FlatPoint>>>>();
            for (int i = 0; i < 10; i++)
            {
                PointList.Add(new List<List<List<FlatPoint>>>());
            }
        }

        public void Start(int x, int y, int cluster, int comp, List<BFSPoint<Color>> startQ = null)
        {
            var queue = new Queue<BFSPoint<Color>>();
            PointList[cluster][comp].Add(new List<FlatPoint>());
            if (startQ == null)
            {
                queue.Enqueue(Points[x][y]);
            }
            else
            {
                foreach (var p in startQ)
                {
                    queue.Enqueue(Points[p.X][p.Y]);
                    Points[p.X][p.Y].Cluster[cluster] = comp;
                    Points[p.X][p.Y].Generation[cluster] = 0;
                    PointList[cluster][comp][0].Add(new FlatPoint(p.X, p.Y));
                }
            }

            Points[x][y].Cluster[cluster] = comp;
            Points[x][y].Generation[cluster] = 0;
            PointList[cluster][comp][0].Add(new FlatPoint(x, y));
            while (queue.Count > 0)
            {
                var curr = queue.Dequeue();
                TryAdd(curr.X + 1, curr.Y, cluster, comp, queue, curr.Generation[cluster] + 1);
                TryAdd(curr.X - 1, curr.Y, cluster, comp, queue, curr.Generation[cluster] + 1);
                TryAdd(curr.X, curr.Y + 1, cluster, comp, queue, curr.Generation[cluster] + 1);
                TryAdd(curr.X, curr.Y - 1, cluster, comp, queue, curr.Generation[cluster] + 1);

                TryAdd(curr.X - 1, curr.Y - 1, cluster, comp, queue, curr.Generation[cluster] + 1);
                TryAdd(curr.X + 1, curr.Y - 1, cluster, comp, queue, curr.Generation[cluster] + 1);
                TryAdd(curr.X - 1, curr.Y + 1, cluster, comp, queue, curr.Generation[cluster] + 1);
                TryAdd(curr.X + 1, curr.Y + 1, cluster, comp, queue, curr.Generation[cluster] + 1);
            }
        }

        public void StartAll(int cluster)
        {
            PointList[cluster] = new List<List<List<FlatPoint>>>();
            ClearCluster(cluster);
            Comp = 0;
            for (int x = 0; x < Points.Count; x++)
            {
                for (int y = 0; y < Points[x].Count; y++)
                {
                    if (Points[x][y].Cluster[cluster] == -1 && Points[x][y].Enabled)
                    {
                        PointList[cluster].Add(new List<List<FlatPoint>>());
                        Start(x, y, cluster, Comp++);
                    }
                }
            }
        }

        public void SetEnabled(bool enabled)
        {
            for (int x = 0; x < State.Width; x++)
            {
                for (int y = 0; y < State.Height; y++)
                {
                    Points[x][y].Enabled = enabled;
                }
            }
        }

        private void TryAdd(int x, int y, int cluster, int comp, Queue<BFSPoint<Color>> queue, int gen)
        {
            if (Exists(x, y) && Points[x][y].Cluster[cluster] == -1 && Points[x][y].Enabled)
            {
                Points[x][y].Cluster[cluster] = comp;
                if (Points[x][y].Generation[cluster] == -1 || Points[x][y].Generation[cluster] > gen)
                {
                    Points[x][y].Generation[cluster] = gen;
                }
                if (PointList[cluster][comp].Count <= gen)
                {
                    PointList[cluster][comp].Add(new List<FlatPoint>());
                }
                PointList[cluster][comp][gen].Add(new FlatPoint(x, y));
                queue.Enqueue(Points[x][y]);
            }
        }

        private void ClearCluster(int cluster)
        {
            foreach (var line in Points)
            {
                foreach (var point in line)
                {
                    point.Cluster[cluster] = -1;
                }
            }
        }

        private bool Exists(int x, int y)
        {
            return x > 0 && y > 0 && x < Points.Count && y < Points[0].Count;
        }
    }
}

using KNNonAir.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KNNonAir.Domain.Service
{
    class KdTreeNode
    {
        enum Axis { Latitude, Longitude };

        private const int DIMENSION = 2;

        public Vertex Point { get; set; }
        public KdTreeNode LeftChild { get; set; }
        public KdTreeNode RightChild { get; set; }

        public void Partition(Dictionary<Vertex, VoronoiCell> nvd, int depth, int frames, RegionHandler partitionCompleted)
        {
            if (!canPartition(nvd, depth, frames, partitionCompleted)) return;

            Axis axis = (Axis)Enum.Parse(typeof(Axis), (depth % DIMENSION).ToString());
            if (axis == Axis.Latitude) nvd = nvd.OrderBy(o => o.Key.Coordinate.Latitude).ToDictionary(i => i.Key, i => i.Value);
            else if (axis == Axis.Longitude) nvd = nvd.OrderBy(o => o.Key.Coordinate.Longitude).ToDictionary(keyvalue => keyvalue.Key, keyvalue => keyvalue.Value);

            int median = nvd.Count / 2;
            Point = nvd.ElementAt(median).Key;
            LeftChild = new KdTreeNode();
            LeftChild.Partition(nvd.Take(median).ToDictionary(i => i.Key, i => i.Value), depth + 1, frames, partitionCompleted);
            RightChild = new KdTreeNode();
            RightChild.Partition(nvd.Skip(median).ToDictionary(i => i.Key, i => i.Value), depth + 1, frames, partitionCompleted);
        }

        private bool canPartition(Dictionary<Vertex, VoronoiCell> nvd, int depth, int frames, RegionHandler partitionCompleted)
        {
            if (nvd.Count == 0) return false;
            if (depth == Math.Log(frames, 2))
            {
                Region region = new Region();
                foreach (KeyValuePair<Vertex, VoronoiCell> nvc in nvd) region.AddNVC(nvc.Value);
                region.RemoveSameBorder();
                partitionCompleted(region);
                return false;
            }
            else return true;
        }
    }
}

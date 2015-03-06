using KNNonAir.Domain.Entity;
using System.Collections.Generic;

namespace KNNonAir.Domain.Service
{
    class KdTree
    {
        private event RegionHandler PartitionCompleted;

        private KdTreeNode _root;
        public List<Region> Regions { get; set; }

        public KdTree(Dictionary<Vertex, VoronoiCell> nvd, int frames)
        {
            _root = new KdTreeNode();
            Regions = new List<Region>();

            PartitionCompleted += AddRegion;
            _root.Partition(nvd, 0, frames, PartitionCompleted);
        }

        private void AddRegion(Region region)
        {
            Regions.Add(region);
        }
    }
}

using System.Collections.Generic;
using KNNonAir.Domain.Entity;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;

namespace KNNonAir.Domain.Service
{
    class ShortcutNetwork
    {
        public int RegionId { get; set; }
        public Dictionary<Edge<Vertex>, double> Distances;
        public Dictionary<int, RoadGraph> Shortcut { get; set; }
        public RoadGraph RegionGraph { get; set; }


        public ShortcutNetwork(int id)
        {
            RegionId = id;
            Distances = new Dictionary<Edge<Vertex>, double>();
            Shortcut = new Dictionary<int, RoadGraph>();
            RegionGraph = new RoadGraph(false);
        }
    }
}

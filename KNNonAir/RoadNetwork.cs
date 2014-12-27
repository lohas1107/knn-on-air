using Geo.Geometries;
using QuickGraph;

namespace KNNonAir
{
    class RoadNetwork
    {
        public AdjacencyGraph<Point, Edge<Point>> Graph { get; set; }
    }
}

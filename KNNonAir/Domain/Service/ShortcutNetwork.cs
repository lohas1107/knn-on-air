using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KNNonAir.Access;
using KNNonAir.Domain.Entity;
using QuickGraph;

namespace KNNonAir.Domain.Service
{
    [Serializable]
    class ShortcutNetwork : ISerializable
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RegionId", RegionId);

            Dictionary<EdgeInfo, double> distances = new Dictionary<EdgeInfo, double>();
            foreach (KeyValuePair<Edge<Vertex>, double> distance in Distances)
            {
                VertexInfo source = new VertexInfo(distance.Key.Source.Coordinate.Latitude, distance.Key.Source.Coordinate.Longitude);
                VertexInfo target = new VertexInfo(distance.Key.Target.Coordinate.Latitude, distance.Key.Target.Coordinate.Longitude);
                distances.Add(new EdgeInfo(source, target), distance.Value);
            }

            info.AddValue("Distances", distances);
            info.AddValue("Shortcut", Shortcut);
            info.AddValue("RegionGraph", RegionGraph);
        }
    }
}

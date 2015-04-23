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
        public Dictionary<Edge<Vertex>, double> Distances;
        public Dictionary<int, RoadGraph> Shortcut { get; set; }

        public ShortcutNetwork()
        {
            Distances = new Dictionary<Edge<Vertex>, double>();
            Shortcut = new Dictionary<int, RoadGraph>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Dictionary<EdgeInfo, double> distances = new Dictionary<EdgeInfo, double>();
            foreach (KeyValuePair<Edge<Vertex>, double> distance in Distances)
            {
                VertexInfo source = new VertexInfo(distance.Key.Source.Coordinate.Latitude, distance.Key.Source.Coordinate.Longitude);
                VertexInfo target = new VertexInfo(distance.Key.Target.Coordinate.Latitude, distance.Key.Target.Coordinate.Longitude);
                distances.Add(new EdgeInfo(source, target), distance.Value);
            }

            info.AddValue("Distances", distances);
            info.AddValue("Shortcut", Shortcut);
        }
    }
}

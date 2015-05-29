using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KNNonAir.Access;
using KNNonAir.Domain.Entity;
using QuickGraph;

namespace KNNonAir.Domain.Service
{
    [Serializable]
    public class ShortcutNetwork : ISerializable
    {
        public Dictionary<Edge<Vertex>, double> Distances;
        public Dictionary<int, RoadGraph> Shortcut { get; set; }
        public Dictionary<int, List<Vertex>> RegionBorders { get; set; }

        public ShortcutNetwork()
        {
            Distances = new Dictionary<Edge<Vertex>, double>();
            Shortcut = new Dictionary<int, RoadGraph>();
            RegionBorders = new Dictionary<int, List<Vertex>>();
        }

        public ShortcutNetwork(ShortcutNetwork shortcutNetwork)
        {
            Distances = new Dictionary<Edge<Vertex>,double>(shortcutNetwork.Distances);
            Shortcut = new Dictionary<int,RoadGraph>(shortcutNetwork.Shortcut);
            RegionBorders = new Dictionary<int,List<Vertex>>(shortcutNetwork.RegionBorders);
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

            Dictionary<int, List<VertexInfo>> regionBorders = new Dictionary<int,List<VertexInfo>>();
            foreach (KeyValuePair<int, List<Vertex>> regionBorder in RegionBorders)
            {
                List<VertexInfo> borders = new List<VertexInfo>();
                foreach (Vertex border in regionBorder.Value)
                {
                    borders.Add(new VertexInfo(border.Coordinate.Latitude, border.Coordinate.Longitude));
                }
                regionBorders.Add(regionBorder.Key, borders);
            }

            info.AddValue("Distances", distances);
            info.AddValue("Shortcut", Shortcut);
            info.AddValue("RegionBorders", regionBorders);
        }
    }
}

using GMap.NET;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace KNNonAir
{
    class PresentationModel
    {
        private RoadNetwork _roadNetwork;
        private Random _random;

        public PresentationModel(RoadNetwork roadNetwork)
        {
            _roadNetwork = roadNetwork;
            _random = new Random(Guid.NewGuid().GetHashCode());
        }

        public List<List<PointLatLng>> GetRoads()
        {
            List<List<PointLatLng>> roads = new List<List<PointLatLng>>();

            foreach (Edge<Vertex> edge in _roadNetwork.Graph.Edges)
            {
                List<PointLatLng> points = new List<PointLatLng>();
                points.Add(new PointLatLng(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude));
                points.Add(new PointLatLng(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude));
                roads.Add(points);
            }

            return roads;
        }

        public List<Tuple<Color, List<PointLatLng>>> GetNVDEdges()
        {
            List<Tuple<Color, List<PointLatLng>>> nvdEdges = new List<Tuple<Color, List<PointLatLng>>>();


            foreach (KeyValuePair<Vertex, VoronoiCell> nvc in _roadNetwork.NVD)
            {
                Color color = Color.FromArgb(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255));

                foreach (Edge<Vertex> edge in nvc.Value.Graph.Edges)
                {
                    List<PointLatLng> points = new List<PointLatLng>();
                    points.Add(new PointLatLng(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude));
                    points.Add(new PointLatLng(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude));
                    nvdEdges.Add(new Tuple<Color, List<PointLatLng>>(color, points));
                }
            }

            return nvdEdges;
        }
    }
}

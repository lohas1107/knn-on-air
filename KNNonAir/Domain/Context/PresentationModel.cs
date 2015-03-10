using GMap.NET;
using KNNonAir.Domain.Entity;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace KNNonAir.Domain.Context
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

        private static List<PointLatLng> GetEdge(Edge<Vertex> edge)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude));
            points.Add(new PointLatLng(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude));
            return points;
        }

        public List<List<PointLatLng>> GetRoads()
        {
            List<List<PointLatLng>> roads = new List<List<PointLatLng>>();

            foreach (Edge<Vertex> edge in _roadNetwork.Graph.Edges)
            {
                List<PointLatLng> points = GetEdge(edge);
                roads.Add(points);
            }

            return roads;
        }

        private void GetColorEdges(List<Tuple<Color, List<PointLatLng>>> colorEdges, IEnumerable<Edge<Vertex>> edges)
        {
            Color color = Color.FromArgb(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255));

            foreach (Edge<Vertex> edge in edges)
            {
                List<PointLatLng> points = GetEdge(edge);
                colorEdges.Add(new Tuple<Color, List<PointLatLng>>(color, points));
            }
        }

        public List<Tuple<Color, List<PointLatLng>>> GetNVDEdges()
        {
            List<Tuple<Color, List<PointLatLng>>> nvdEdges = new List<Tuple<Color, List<PointLatLng>>>();

            foreach (KeyValuePair<Vertex, VoronoiCell> nvc in _roadNetwork.NVD)
            {
                GetColorEdges(nvdEdges, nvc.Value.Graph.Edges);
            }

            return nvdEdges;
        }

        public List<Tuple<Color, List<PointLatLng>>> GetRegionEdges()
        {
            List<Tuple<Color, List<PointLatLng>>> regionEdges = new List<Tuple<Color, List<PointLatLng>>>();

            foreach (KNNonAir.Domain.Entity.Region region in _roadNetwork.Regions)
            {
                GetColorEdges(regionEdges, region.Graph.Edges);
            }

            return regionEdges;
        }

        public List<List<PointLatLng>> GetVQTree()
        {
            List<List<PointLatLng>> mbrs = new List<List<PointLatLng>>();

            foreach (MBR mbr in _roadNetwork.QuadMBRs)
            {
                Vertex topLeft = new Vertex(mbr.Y, mbr.X);
                Vertex topRight = new Vertex(mbr.Y, mbr.X + mbr.Width);
                Vertex bottomLeft = new Vertex(mbr.Y - mbr.Height, mbr.X);
                Vertex bottomRight = new Vertex(mbr.Y - mbr.Height, mbr.X + mbr.Width);

                mbrs.Add(GetEdge(new Edge<Vertex>(topLeft, topRight)));
                mbrs.Add(GetEdge(new Edge<Vertex>(bottomLeft, bottomRight)));
                mbrs.Add(GetEdge(new Edge<Vertex>(topLeft, bottomLeft)));
                mbrs.Add(GetEdge(new Edge<Vertex>(topRight, bottomRight)));
            }

            return mbrs;
        }
    }
}

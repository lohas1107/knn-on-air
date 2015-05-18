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
        private Model _model;

        public PresentationModel(Model model)
        {
            _model = model;
        }

        private static List<PointLatLng> GetEdges(Edge<Vertex> edge)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude));
            points.Add(new PointLatLng(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude));
            return points;
        }

        public List<List<PointLatLng>> GetRoads()
        {
            List<List<PointLatLng>> roads = new List<List<PointLatLng>>();

            foreach (Edge<Vertex> edge in _model.Road.Graph.Edges)
            {
                List<PointLatLng> points = GetEdges(edge);
                roads.Add(points);
            }

            return roads;
        }

        private void GetColorEdges(List<Tuple<Color, List<PointLatLng>>> colorEdges, IEnumerable<Edge<Vertex>> edges)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            Color color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));

            foreach (Edge<Vertex> edge in edges)
            {
                List<PointLatLng> points = GetEdges(edge);
                colorEdges.Add(new Tuple<Color, List<PointLatLng>>(color, points));
            }
        }

        public List<Tuple<Color, List<PointLatLng>>> GetNVDEdges()
        {
            List<Tuple<Color, List<PointLatLng>>> nvdEdges = new List<Tuple<Color, List<PointLatLng>>>();

            foreach (KeyValuePair<Vertex, VoronoiCell> nvc in _model.NVD)
            {
                GetColorEdges(nvdEdges, nvc.Value.Road.Graph.Edges);
            }

            return nvdEdges;
        }

        public List<Tuple<Color, List<PointLatLng>>> GetRegionEdges(Dictionary<int, KNNonAir.Domain.Entity.Region> regions)
        {
            List<Tuple<Color, List<PointLatLng>>> regionEdges = new List<Tuple<Color, List<PointLatLng>>>();

            foreach (KeyValuePair<int, KNNonAir.Domain.Entity.Region> region in regions)
            {
                GetColorEdges(regionEdges, region.Value.Road.Graph.Edges);
            }

            return regionEdges;
        }

        public List<List<PointLatLng>> GetMBRs(List<MBR> mbrList)
        {
            List<List<PointLatLng>> mbrs = new List<List<PointLatLng>>();

            foreach (MBR mbr in mbrList)
            {
                Vertex topLeft = new Vertex(mbr.Y, mbr.X);
                Vertex topRight = new Vertex(mbr.Y, mbr.X + mbr.Width);
                Vertex bottomLeft = new Vertex(mbr.Y - mbr.Height, mbr.X);
                Vertex bottomRight = new Vertex(mbr.Y - mbr.Height, mbr.X + mbr.Width);

                mbrs.Add(GetEdges(new Edge<Vertex>(topLeft, topRight)));
                mbrs.Add(GetEdges(new Edge<Vertex>(bottomLeft, bottomRight)));
                mbrs.Add(GetEdges(new Edge<Vertex>(topLeft, bottomLeft)));
                mbrs.Add(GetEdges(new Edge<Vertex>(topRight, bottomRight)));
            }

            return mbrs;
        }
    }
}

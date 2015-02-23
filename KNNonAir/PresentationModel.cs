using GMap.NET;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace KNNonAir
{
    enum RouteColors { Red, Orange, Yellow, Green, SkyBlue, Blue, Purple };

    class PresentationModel
    {
        private RoadNetwork _roadNetwork;

        public PresentationModel(RoadNetwork roadNetwork)
        {
            _roadNetwork = roadNetwork;
        }

        public List<MapRoute> GetNVCMapRouteList(Vertex vertex)
        {
            List<MapRoute> mapRouteList = new List<MapRoute>();

            foreach (Edge<Vertex> edge in _roadNetwork.NVD[vertex].Graph.Edges)
            {
                PointLatLng start = new PointLatLng(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude);
                PointLatLng end = new PointLatLng(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude);
                MapRoute mapRoute = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetRoute(start, end, false, true, 15);
                if (mapRoute == null) continue;

                mapRouteList.Add(mapRoute);
            }

            return mapRouteList;
        }

        public List<Tuple<Color, List<PointLatLng>>> GetNVDEdges()
        {
            List<Tuple<Color, List<PointLatLng>>> nvdEdges = new List<Tuple<Color, List<PointLatLng>>>();
            Color color;

            foreach (KeyValuePair<Vertex, VoronoiCell> nvc in _roadNetwork.NVD)
            {
                color = RandomizeColor();

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

        private Color RandomizeColor()
        {
            Array values = Enum.GetValues(typeof(RouteColors));
            Random random = new Random();
            RouteColors randomColor = (RouteColors)values.GetValue(random.Next(values.Length));
            return Color.FromName(randomColor.ToString());
        }
    }
}

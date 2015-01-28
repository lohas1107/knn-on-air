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

        public List<Tuple<Color, MapRoute>> GetNVDMapRoutes()
        {
            List<Tuple<Color, MapRoute>> mapRoutes = new List<Tuple<Color, MapRoute>>();
            Color originColor = Color.White;
            Color color = Color.White;

            foreach (KeyValuePair<Vertex, VoronoiCell> nvc in _roadNetwork.NVD)
            {              
                while (color == originColor) { color = RandomizeColor(); }
                originColor = color;

                foreach (MapRoute route in GetNVCMapRouteList(nvc.Key))
                {
                    mapRoutes.Add(new Tuple<Color, MapRoute>(color, route));
                }
            }

            return mapRoutes;
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

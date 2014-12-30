using Geo.Geometries;
using GMap.NET;
using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir
{
    class RoadNetwork
    {
        public AdjacencyGraph<Point, Edge<Point>> Graph { get; set; }

        public List<MapRoute> getMapRouteList()
        {
            List<MapRoute> mapRouteList = new List<MapRoute>();

            foreach (Edge<Point> edge in Graph.Edges)
            {
                PointLatLng start = new PointLatLng(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude);
                PointLatLng end = new PointLatLng(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude);
                MapRoute mapRoute = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetRoute(start, end, false, true, 15);

                if (mapRoute != null)
                {
                    mapRouteList.Add(mapRoute);
                }
            }

            return mapRouteList;
        }
    }
}

using GMap.NET;
using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir
{
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
    }
}

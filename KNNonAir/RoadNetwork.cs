using System;
using System.Collections.Generic;
using Geo;
using Geo.Geometries;
using GMap.NET;
using QuickGraph;

namespace KNNonAir
{
    class RoadNetwork
    {
        public event Event.Handler LoadSiteCompleted;

        public AdjacencyGraph<Point, Edge<Point>> Graph { get; set; }
        public List<PointLatLng> Sites { get; set; }

        public RoadNetwork()
        {
            Graph = new AdjacencyGraph<Point, Edge<Point>>(false);
            Sites = new List<PointLatLng>();
        }

        public List<MapRoute> GetMapRouteList()
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

        public void LoadSite()
        {
            List<Point> siteList = FileIO.ReadSiteFile();
            if (siteList == null) return;

            foreach (Point site in siteList)
            {
                Point adjustedSite = AdjustSiteToEdge(site);
                if (adjustedSite == null) continue;

                Graph.AddVertex(adjustedSite);
                Sites.Add(new PointLatLng(adjustedSite.Coordinate.Latitude, adjustedSite.Coordinate.Longitude));
            }

            LoadSiteCompleted();
        }

        private Point AdjustSiteToEdge(Point site)
        {
            if (IsEdgeContainsVertex(site)) return site;

            double minDiatance = double.MaxValue;
            Point adjustedSite = null;

            foreach (Edge<Point> edge in Graph.Edges)
            {
                Point newSite = Project(edge.Source.Coordinate, edge.Target.Coordinate, site.Coordinate);
                if (newSite == null) continue;

                double distance = Math.Sqrt(Math.Pow(newSite.Coordinate.Latitude - site.Coordinate.Latitude, 2) + Math.Pow(newSite.Coordinate.Longitude - site.Coordinate.Longitude, 2));
                if (distance >= minDiatance) continue;

                minDiatance = distance;
                adjustedSite = newSite;
            }

            return adjustedSite;
        }

        private bool IsEdgeContainsVertex(Point site)
        {
            foreach (Edge<Point> edge in Graph.Edges)
            {
                if (edge.IsAdjacent(site)) return true;
            }

            return false;
        }

        private Point Project(Coordinate source, Coordinate target, Coordinate site)
        {
            double U = ((site.Latitude - source.Latitude) * (target.Latitude - source.Latitude)) + ((site.Longitude - source.Longitude) * (target.Longitude - source.Longitude));
            double Udenom = Math.Pow(target.Latitude - source.Latitude, 2) + Math.Pow(target.Longitude - source.Longitude, 2);
            U /= Udenom;

            double latitude = source.Latitude + (U * (target.Latitude - source.Latitude));
            double longitude = source.Longitude + (U * (target.Longitude - source.Longitude));
            Point result = new Point(latitude, longitude);

            double minX, maxX, minY, maxY;
            minX = Math.Min(source.Latitude, target.Latitude);
            maxX = Math.Max(source.Latitude, target.Latitude);
            minY = Math.Min(source.Longitude, target.Longitude);
            maxY = Math.Max(source.Longitude, target.Longitude);

            bool isValid = (result.Coordinate.Latitude >= minX && result.Coordinate.Latitude <= maxX) && (result.Coordinate.Longitude >= minY && result.Coordinate.Longitude <= maxY);
            return isValid ? result : null;
        }
    }
}

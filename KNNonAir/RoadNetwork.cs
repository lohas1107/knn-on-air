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

        public AdjacencyGraph<Vertex, Edge<Vertex>> Graph { get; set; }
        public List<PointLatLng> Sites { get; set; }

        public RoadNetwork()
        {
            Graph = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
            Sites = new List<PointLatLng>();
        }

        public List<MapRoute> GetMapRouteList()
        {
            List<MapRoute> mapRouteList = new List<MapRoute>();

            foreach (Edge<Vertex> edge in Graph.Edges)
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
            List<Vertex> siteList = FileIO.ReadSiteFile();
            if (siteList == null) return;

            foreach (Vertex site in siteList)
            {
                Vertex adjustedSite = AdjustSiteToEdge(site);
                if (adjustedSite == null) continue;

                Sites.Add(new PointLatLng(adjustedSite.Coordinate.Latitude, adjustedSite.Coordinate.Longitude));
            }

            LoadSiteCompleted();
        }

        private Vertex AdjustSiteToEdge(Vertex site)
        {
            double minDiatance = double.MaxValue;
            Vertex adjustedSite = null;
            Edge<Vertex> toEdge = null;

            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                Vertex newSite = Project(edge.Source.Coordinate, edge.Target.Coordinate, site.Coordinate);
                if (newSite == null) continue;

                double distance = Math.Sqrt(Math.Pow(newSite.Coordinate.Latitude - site.Coordinate.Latitude, 2) + Math.Pow(newSite.Coordinate.Longitude - site.Coordinate.Longitude, 2));
                if (distance >= minDiatance) continue;

                minDiatance = distance;
                adjustedSite = newSite;
                toEdge = edge;
            }

            InsertVertex(adjustedSite, toEdge);

            return adjustedSite;
        }

        private void InsertVertex(Vertex adjustedSite, Edge<Vertex> edge)
        {
            Graph.RemoveEdge(edge);
            Graph.AddVertex(adjustedSite);
            Graph.AddEdge(new Edge<Vertex>(edge.Source, adjustedSite));
            Graph.AddEdge(new Edge<Vertex>(adjustedSite, edge.Target));
        }

        private bool IsEdgeContainsVertex(Vertex site)
        {
            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                if (edge.IsAdjacent(site)) return true;
            }

            return false;
        }

        private Vertex Project(Coordinate source, Coordinate target, Coordinate site)
        {
            double U = ((site.Latitude - source.Latitude) * (target.Latitude - source.Latitude)) + ((site.Longitude - source.Longitude) * (target.Longitude - source.Longitude));
            double Udenom = Math.Pow(target.Latitude - source.Latitude, 2) + Math.Pow(target.Longitude - source.Longitude, 2);
            U /= Udenom;

            double latitude = source.Latitude + (U * (target.Latitude - source.Latitude));
            double longitude = source.Longitude + (U * (target.Longitude - source.Longitude));
            Vertex result = new InterestPoint(latitude, longitude);

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

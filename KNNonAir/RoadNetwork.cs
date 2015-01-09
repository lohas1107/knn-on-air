using System;
using System.Collections.Generic;
using Geo;
using GMap.NET;
using QuickGraph;

namespace KNNonAir
{
    class RoadNetwork
    {
        public event Event.Handler LoadRoadsCompleted;
        public event Event.Handler LoadPoIsCompleted;

        public AdjacencyGraph<Vertex, Edge<Vertex>> Graph { get; set; }
        public List<PointLatLng> PoIs { get; set; }

        public RoadNetwork()
        {
            Graph = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
            PoIs = new List<PointLatLng>();
        }

        public void LoadRoads()
        {
            List<Edge<Vertex>> edgeList = FileIO.ReadRoadFile();
            if (edgeList == null) return;

            foreach (Edge<Vertex> edge in edgeList)
            {
                Graph.AddVertex(edge.Source);
                Graph.AddVertex(edge.Target);
                Graph.AddEdge(edge);
            }

            LoadRoadsCompleted();
        }

        public List<MapRoute> GetMapRouteList()
        {
            List<MapRoute> mapRouteList = new List<MapRoute>();

            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                PointLatLng start = new PointLatLng(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude);
                PointLatLng end = new PointLatLng(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude);
                MapRoute mapRoute = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetRoute(start, end, false, true, 15);
                if (mapRoute == null) continue;

                mapRouteList.Add(mapRoute);
            }

            return mapRouteList;
        }

        public void LoadPoIs()
        {
            List<Vertex> poiList = FileIO.ReadPoIFile();
            if (poiList == null) return;

            foreach (Vertex poi in poiList)
            {
                Vertex adjustedPoI = AdjustPoIToEdge(poi);
                if (adjustedPoI == null) continue;

                PoIs.Add(new PointLatLng(adjustedPoI.Coordinate.Latitude, adjustedPoI.Coordinate.Longitude));
            }

            LoadPoIsCompleted();
        }

        private Vertex AdjustPoIToEdge(Vertex poi)
        {
            double minDiatance = double.MaxValue;
            Vertex adjustedPoI = null;
            Edge<Vertex> toEdge = null;

            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                Vertex newPoI = Project(edge.Source.Coordinate, edge.Target.Coordinate, poi.Coordinate);
                if (newPoI == null) continue;

                double distance = Math.Sqrt(Math.Pow(newPoI.Coordinate.Latitude - poi.Coordinate.Latitude, 2) + Math.Pow(newPoI.Coordinate.Longitude - poi.Coordinate.Longitude, 2));
                if (distance >= minDiatance) continue;

                minDiatance = distance;
                adjustedPoI = newPoI;
                toEdge = edge;
            }

            InsertVertex(adjustedPoI, toEdge);

            return adjustedPoI;
        }

        private void InsertVertex(Vertex vertex, Edge<Vertex> edge)
        {
            if (vertex == null || edge == null) return;

            Graph.RemoveEdge(edge);
            Graph.AddVertex(vertex);
            Graph.AddEdge(new Edge<Vertex>(edge.Source, vertex));
            Graph.AddEdge(new Edge<Vertex>(vertex, edge.Target));
        }

        private bool IsEdgeContainsVertex(Vertex vertex)
        {
            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                if (edge.IsAdjacent(vertex)) return true;
            }

            return false;
        }

        private Vertex Project(Coordinate source, Coordinate target, Coordinate vertex)
        {
            double U = ((vertex.Latitude - source.Latitude) * (target.Latitude - source.Latitude)) + ((vertex.Longitude - source.Longitude) * (target.Longitude - source.Longitude));
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

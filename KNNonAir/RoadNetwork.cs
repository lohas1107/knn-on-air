using GMap.NET;
using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir
{
    class RoadNetwork
    {
        public event Handler LoadRoadsCompleted;
        public event Handler LoadPoIsCompleted;
        public event Handler GenerateNVDCompleted;

        public AdjacencyGraph<Vertex, Edge<Vertex>> Graph { get; set; }
        public List<Vertex> PoIs { get; set; }
        public Dictionary<Vertex, VoronoiCell> NVD { get; set; }

        public RoadNetwork()
        {
            Graph = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
            PoIs = new List<Vertex>();
            NVD = new Dictionary<Vertex, VoronoiCell>();
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

        public List<MapRoute> GetRoadMapRouteList()
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

                PoIs.Add(adjustedPoI);
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
                Vertex newPoI = Arithmetics.Project(edge.Source.Coordinate, edge.Target.Coordinate, poi.Coordinate);
                if (newPoI == null) continue;

                double distance = Arithmetics.CalculateDistance(poi.Coordinate, newPoI.Coordinate);
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

        public void GenerateNVD()
        {
            NVD.Add(PoIs[0], new PathTree(PoIs[0]).GenerateNVC(Graph));
            GenerateNVDCompleted();
        }
    }
}

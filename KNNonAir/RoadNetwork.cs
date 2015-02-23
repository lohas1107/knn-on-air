using GMap.NET;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace KNNonAir
{
    class RoadNetwork
    {
        private const double ROAD_WIDTH_OFFSET = 0.00001; // 1m
        private const double FIFTY_METER = 0.0005;
        private const double TEN_METER = 0.0001;

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
                Edge<Vertex> sourceEdge = null;
                Edge<Vertex> targetEdge = null;
                Vertex source = edge.Source;
                Vertex target = edge.Target;

                if (!Graph.ContainsVertex(source)) sourceEdge = FindOverlapedEdge(source, Arithmetics.GetSlope(edge), 0.1, true, ROAD_WIDTH_OFFSET);
                if (!Graph.ContainsVertex(target)) targetEdge = FindOverlapedEdge(target, Arithmetics.GetSlope(edge), 0.1, true, ROAD_WIDTH_OFFSET);
                if (sourceEdge != null) source = AdjustOverlap(sourceEdge, target);
                if (targetEdge != null) target = AdjustOverlap(targetEdge, source);
                if (sourceEdge != null && targetEdge != null && (sourceEdge == targetEdge)) continue;

                Graph.AddVertex(source);
                Graph.AddVertex(target);
                Graph.AddEdge(new Edge<Vertex>(source, target));
            }

            ConnectBrokenEdge();
            LoadRoadsCompleted();
        }

        private Edge<Vertex> FindOverlapedEdge(Vertex vertex, double slope, double angle, bool isBelow, double far)
        {
            Vertex projectVertex = null;
            Edge<Vertex> projectEdge = null;
            double minDistance = double.MaxValue;

            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                if (isBelow)
                {
                    if (edge.Source == vertex || edge.Target == vertex) return null;
                    if (Arithmetics.GetIncludedAngle(Arithmetics.GetSlope(edge), slope) > angle) continue;
                }
                if (!isBelow && Arithmetics.GetIncludedAngle(Arithmetics.GetSlope(edge), slope) < angle) continue;

                projectVertex = Arithmetics.Project(edge.Source.Coordinate, edge.Target.Coordinate, vertex.Coordinate);
                if (projectVertex == null) continue;

                double distance = Arithmetics.CalculateDistance( vertex.Coordinate, projectVertex.Coordinate);
                if (distance < minDistance && distance < far)
                {
                    minDistance = distance;
                    projectEdge = edge;
                }
            }

            return projectEdge;
        }

        private Vertex AdjustOverlap(Edge<Vertex> edge, Vertex vertex)
        {
            double sourceDistance = Arithmetics.CalculateDistance(edge.Source.Coordinate, vertex.Coordinate);
            double targetDistance = Arithmetics.CalculateDistance(edge.Target.Coordinate, vertex.Coordinate);

            if (sourceDistance < targetDistance) return edge.Source;
            else return edge.Target;            
        }

        private void ConnectBrokenEdge()
        {
            List<Vertex> sideVertexs = GetSideVertexs();

            sideVertexs = sideVertexs.OrderBy(o => o.Coordinate.Latitude).ToList();
            FindNearPointPair(sideVertexs);
            sideVertexs = sideVertexs.OrderBy(o => o.Coordinate.Longitude).ToList();
            FindNearPointPair(sideVertexs);

            foreach (Vertex sideVertex in GetSideVertexs())
            {
                PathTree pathTree = new PathTree(sideVertex);
                List<Vertex> connectVertex = pathTree.FindPathsByRange(Graph, FIFTY_METER);

                double minDistance = double.MaxValue;
                Vertex minVertex = null;
                foreach (Vertex vertex in Graph.Vertices)
                {
                    if (connectVertex.Contains(vertex)) continue;

                    double distance = Arithmetics.CalculateDistance(vertex.Coordinate, sideVertex.Coordinate);
                    if (distance < minDistance && distance < TEN_METER)
                    {
                        minDistance = distance;
                        minVertex = vertex;
                    }
                }

                if (minVertex != null) Graph.AddEdge(new Edge<Vertex>(sideVertex, minVertex));
            }
        }

        private void FindNearPointPair(List<Vertex> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (Arithmetics.CalculateDistance(points[i].Coordinate, points[i + 1].Coordinate) < ROAD_WIDTH_OFFSET)
                {
                    Graph.AddEdge(new Edge<Vertex>(points[i], points[i + 1]));
                }
            }
        }

        public List<Vertex> GetSideVertexs()
        {
            List<Vertex> sideVertexs = new List<Vertex>();

            foreach(Vertex vertex in Graph.Vertices)
            {
                if (IsEdgeContainsVertex(vertex) != null) sideVertexs.Add(vertex);
            }

            return sideVertexs;
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

        private Edge<Vertex> IsEdgeContainsVertex(Vertex vertex)
        {
            int count = 0;
            Edge<Vertex> sideEdge = null;

            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                if (edge.IsAdjacent(vertex)) count++;
                sideEdge = edge;
            }

            if (count < 2) return sideEdge;
            return null;
        }

        public void GenerateNVD()
        {
            foreach (Vertex poi in PoIs)
            {
                NVD.Add(poi, new PathTree(poi).GenerateNVC(Graph));
            }

            GenerateNVDCompleted();
        }
    }
}

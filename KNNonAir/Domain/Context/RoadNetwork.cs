using KNNonAir.Access;
using KNNonAir.Domain.Entity;
using KNNonAir.Domain.Service;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;

namespace KNNonAir.Domain.Context
{
    class RoadNetwork
    {
        private const double ROAD_WIDTH_OFFSET = 0.00001; // 1m
        private const double FIFTY_METER = 0.0005;
        private const double TEN_METER = 0.0001;

        public event VertexListHandler LoadPoIsCompleted;

        public AdjacencyGraph<Vertex, Edge<Vertex>> Graph { get; set; }
        public List<Vertex> PoIs { get; set; }
        public Dictionary<Vertex, VoronoiCell> NVD { get; set; }
        public List<Region> Regions { get; set; }
        public List<MBR> QuadMBRs { get; set; }

        public RoadNetwork()
        {
            Graph = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
            PoIs = new List<Vertex>();
            NVD = new Dictionary<Vertex, VoronoiCell>();
            Regions = new List<Region>();
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

                if (!Graph.ContainsVertex(source)) sourceEdge = FindOverlapedEdge(source, Arithmetics.GetSlope(edge));
                if (!Graph.ContainsVertex(target)) targetEdge = FindOverlapedEdge(target, Arithmetics.GetSlope(edge));
                if (sourceEdge != null) source = AdjustOverlap(sourceEdge, target);
                if (targetEdge != null) target = AdjustOverlap(targetEdge, source);
                if (sourceEdge != null && targetEdge != null && (sourceEdge == targetEdge)) continue;
                if (ConnectVertexCount(source) > 1 && ConnectVertexCount(target) > 1) continue;

                Graph.AddVertex(source);
                Graph.AddVertex(target);
                Graph.AddEdge(new Edge<Vertex>(source, target));
            }

            ConnectBrokenEdge();
        }

        /// <summary>
        /// 找出有重疊到的 Edge
        /// </summary>
        /// <param name="vertex">投影到 Edge 上</param>
        /// <param name="slope">判斷斜率相近的 Edge</param>
        /// <returns></returns>
        private Edge<Vertex> FindOverlapedEdge(Vertex vertex, double slope)
        {
            Vertex projectVertex = null;
            Edge<Vertex> projectEdge = null;
            double minDistance = double.MaxValue;

            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                if (edge.Source == vertex || edge.Target == vertex) return null;
                if (Arithmetics.GetIncludedAngle(Arithmetics.GetSlope(edge), slope) > 0.1) continue;

                projectVertex = Arithmetics.Project(edge.Source, edge.Target, vertex);
                if (projectVertex == null) continue;

                double distance = Arithmetics.GetDistance( vertex, projectVertex);
                if (distance < minDistance && distance < ROAD_WIDTH_OFFSET)
                {
                    minDistance = distance;
                    projectEdge = edge;
                }
            }

            return projectEdge;
        }

        /// <summary>
        /// 將重疊到的點，調整到沒有重疊的位置
        /// </summary>
        /// <param name="edge">被重疊到的 Edge</param>
        /// <param name="vertex">重疊的 Edge 另一端</param>
        /// <returns></returns>
        private Vertex AdjustOverlap(Edge<Vertex> edge, Vertex vertex)
        {
            double sourceDistance = Arithmetics.GetDistance(edge.Source, vertex);
            double targetDistance = Arithmetics.GetDistance(edge.Target, vertex);

            if (sourceDistance < targetDistance) return edge.Source;
            else return edge.Target;            
        }

        private void ConnectBrokenEdge()
        {
            // 孤點對孤點連接
            List<Vertex> sideVertexs = GetSideVertexs();
            sideVertexs = sideVertexs.OrderBy(o => o.Coordinate.Latitude).ToList();
            FindNearPointPair(sideVertexs);
            sideVertexs = sideVertexs.OrderBy(o => o.Coordinate.Longitude).ToList();
            FindNearPointPair(sideVertexs);

            // 孤點對線連接
            foreach (Vertex sideVertex in GetSideVertexs())
            {
                PathTree pathTree = new PathTree(sideVertex);
                List<Vertex> connectVertex = pathTree.FindPathsByRange(Graph, FIFTY_METER);

                double minDistance = double.MaxValue;
                Vertex minVertex = null;
                foreach (Vertex vertex in Graph.Vertices)
                {
                    if (connectVertex.Contains(vertex)) continue;

                    double distance = Arithmetics.GetDistance(vertex, sideVertex);
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
                if (Arithmetics.GetDistance(points[i], points[i + 1]) < ROAD_WIDTH_OFFSET)
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
                if (ConnectVertexCount(vertex) < 2) sideVertexs.Add(vertex);
            }

            return sideVertexs;
        }

        private int ConnectVertexCount(Vertex vertex)
        {
            int count = 0;

            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                if (edge.IsAdjacent(vertex)) count++;
            }

            return count;
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

            LoadPoIsCompleted(PoIs);
        }

        private Vertex AdjustPoIToEdge(Vertex poi)
        {
            double minDiatance = double.MaxValue;
            Vertex adjustedPoI = null;
            Edge<Vertex> toEdge = null;

            foreach (Edge<Vertex> edge in Graph.Edges)
            {
                Vertex newPoI = Arithmetics.Project(edge.Source, edge.Target, poi);
                if (newPoI == null) continue;

                double distance = Arithmetics.GetDistance(poi, newPoI);
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

        public void GenerateNVD()
        {
            foreach (Vertex poi in PoIs)
            {
                NVD.Add(poi, new PathTree(poi).GenerateNVC(Graph));
            }
        }

        public void AddNVD()
        {
            List<NVCInfo> nvcList = FileIO.ReadNVDFile();
            Graph = Parser.ParseNVCInfoToGraph(nvcList);
            NVD = Parser.ParseNVCInfoToNVD(nvcList);
            PoIs = Parser.ParsePoIInfo(nvcList);
        }

        public void Partition(int frames)
        {
            KdTree kdTree = new KdTree(NVD, frames);
            Regions = kdTree.Regions;
        }

        public void GenerateVQTree()
        {
            List<Vertex> borderPoints = new List<Vertex>();
            List<Vertex> vertices = Graph.Vertices.ToList();

            foreach(Region region in Regions)
            {
                foreach (Vertex borderPoint in region.BorderPoints) borderPoints.Add(borderPoint);
            }

            vertices = vertices.OrderBy(o => o.Coordinate.Longitude).ToList();
            double x = vertices.First().Coordinate.Longitude;
            double width = vertices.Last().Coordinate.Longitude - x;

            vertices = vertices.OrderBy(o => o.Coordinate.Latitude).ToList();
            double y = vertices.Last().Coordinate.Latitude;
            double height = y - vertices.First().Coordinate.Latitude;

            VQTree vqTree = new VQTree(borderPoints, new MBR(x, y, width, height));
            QuadMBRs = vqTree.MBRs;
        }
    }
}

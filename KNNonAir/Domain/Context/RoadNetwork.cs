using KNNonAir.Access;
using KNNonAir.Domain.Entity;
using KNNonAir.Domain.Service;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KNNonAir.Domain.Context
{
    class RoadNetwork
    {
        public RoadGraph Road { get; set; }
        public List<Vertex> PoIs { get; set; }
        public Dictionary<Vertex, Edge<Vertex>> BorderPoints { get; set; }
        public Dictionary<Vertex, VoronoiCell> NVD { get; set; }
        public Dictionary<int, Region> Regions { get; set; }
        public VQTree VQTree { get; set; }
        public List<MBR> QuadMBRs { get; set; }
        public Dictionary<int, Dictionary<int, double>> MinTable { get; set; }
        public Dictionary<int, Tuple<int, double>> MaxCountTable { get; set; }
        public Vertex QueryPoint { get; set; }
        public List<Vertex> Answers { get; set; }

        public RoadNetwork()
        {
            Road = new RoadGraph(false);
            PoIs = new List<Vertex>();
            BorderPoints = new Dictionary<Vertex, Edge<Vertex>>();
            NVD = new Dictionary<Vertex, VoronoiCell>();
        }

        public void LoadRoads()
        {
            List<Edge<Vertex>> edgeList = Parser.ParseRoadData(FileIO.ReadGeoJsonFile());
            if (edgeList == null) return;

            Road.LoadRoads(edgeList);
        }

        public void LoadPoIs()
        {
            List<Vertex> poiList = Parser.ParsePoIData(FileIO.ReadGeoJsonFile());
            if (poiList == null) return;

            foreach (Vertex poi in poiList)
            {
                Vertex adjustedPoI = Road.AdjustPoIToEdge(poi);
                if (adjustedPoI == null) continue;

                PoIs.Add(adjustedPoI);
            }
        }

        public void GenerateNVD()
        {
            GenerateNVC(0, PoIs.Count, Road);
        }

        private void AddBorderPoint(Vertex borderPoint, Edge<Vertex> edge)
        {
            BorderPoints.Add(borderPoint, edge);
        }

        private void GenerateNVC(int index, int count, RoadGraph road)
        {
            if (index >= count) return;

            PathTree pathTree = new PathTree(PoIs[index]);
            pathTree.FindBorderPointCompleted += AddBorderPoint;

            while (pathTree.IsRepeat)
            {
                pathTree.GenerateNVC(road);

                foreach (KeyValuePair<Vertex, Edge<Vertex>> kvp in BorderPoints)
                {
                    Edge<Vertex> brokenEdge = null;
                    if (road.Graph.TryGetEdge(kvp.Value.Source, kvp.Value.Target, out brokenEdge)) road.InsertVertex(kvp.Key, brokenEdge);
                    else if (road.Graph.TryGetEdge(kvp.Value.Target, kvp.Value.Source, out brokenEdge)) road.InsertVertex(kvp.Key, brokenEdge);
                    else pathTree.IsRepeat = true;
                }
                BorderPoints.Clear();
            }

            VoronoiCell nvc = pathTree.FindNVC();
            NVD.Add(PoIs[index], nvc);

            foreach (Edge<Vertex> pathEdge in nvc.Road.Graph.Edges)
            {
                Edge<Vertex> removeEdge = null;
                if (road.Graph.TryGetEdge(pathEdge.Source, pathEdge.Target, out removeEdge)) road.Graph.RemoveEdge(removeEdge);
                else if (road.Graph.TryGetEdge(pathEdge.Target, pathEdge.Source, out removeEdge)) road.Graph.RemoveEdge(removeEdge);
            }

            GenerateNVC(index + 1, count, road);
        }

        public void AddNVD()
        {
            List<NVCInfo> nvcList = FileIO.ReadNVDFile();
            if (nvcList == null) return;

            Road = Parser.ParseNVCInfoToGraph(nvcList);
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
            List<Vertex> vertices = Road.Graph.Vertices.ToList();

            vertices = vertices.OrderBy(o => o.Coordinate.Longitude).ToList();
            double x = vertices.First().Coordinate.Longitude;
            double width = vertices.Last().Coordinate.Longitude - x;

            vertices = vertices.OrderBy(o => o.Coordinate.Latitude).ToList();
            double y = vertices.Last().Coordinate.Latitude;
            double height = y - vertices.First().Coordinate.Latitude;

            MBR mbr = new MBR(x, y, width, height);
            foreach (KeyValuePair<int, Region> region in Regions)
            {
                foreach (Vertex borderPoint in region.Value.BorderPoints) borderPoints.Add(borderPoint);
                mbr.AddVertices(region.Value.Road.Graph.Vertices);
            }

            VQTree = new VQTree(borderPoints, mbr);
            QuadMBRs = VQTree.MBRs;
        }

        public void ComputeTable()
        {
            CountingTable table = new CountingTable(Road);
            table.ComputeTables(Regions);
            MinTable = table.MinTable;
            MaxCountTable = table.MaxCountTable;
        }

        public void SearchKNN()
        {
            int regionId = -1;
            do
            {
                QueryPoint = Road.PickQueryPoint();
                regionId = VQTree.searchRegion(QueryPoint);
            }
            while (regionId == -1);
            double upperBound = MaxCountTable[regionId].Item2;

            List<Region> cList = new List<Region>();
            for (int i = 0; i < Regions.Count; i++)
            {
                if (i < regionId && MinTable[i][regionId] <= upperBound) cList.Add(Regions[i]);
                else if (i == regionId) cList.Add(Regions[i]);
                else if (i > regionId && MinTable[regionId][i] <= upperBound) cList.Add(Regions[i]);
            }

            List<Vertex> pois = new List<Vertex>();
            RoadGraph graph = new RoadGraph(false);
            while (cList.Count > 0)
            {
                Region region = cList.First();
                cList.RemoveAt(0);

                if (region.Id == regionId)
                {
                    foreach (Vertex poi in region.PoIs) pois.Add(poi);
                    graph.AddGraph(region.Road);
                    CountingTable table = new CountingTable(graph);
                    upperBound = table.UpdateUpperBound(QueryPoint, pois, region, 10);
                    graph = table.PruneGraphVertices(QueryPoint, pois, upperBound, 10);
                }
                else if (CanTune(region.Id, regionId, upperBound))
                {
                    foreach (Vertex poi in region.PoIs) pois.Add(poi);
                    CountingTable table = new CountingTable(region.Road);
                    graph.AddGraph(table.PruneRegionVertices(region, QueryPoint, pois, upperBound, 10));
                }
            }

            CountingTable answer = new CountingTable(graph);
            Answers = answer.GetKNN(QueryPoint, pois, 10);
        }

        private bool CanTune(int id, int regionId, double upperBound)
        {
            if (id < regionId && MinTable[id][regionId] < upperBound) return true;
            if (id > regionId && MinTable[regionId][id] < upperBound) return true;
            
            return false;
        }
    }
}

﻿using KNNonAir.Access;
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
        public ShortcutNetwork Shortcut { get; set; }
        public CountingTable EBTable  { get; set; }
        public Vertex QueryPoint { get; set; }
        public List<Vertex> Answers { get; set; }
        public List<Region> Latency { get; set; }
        public List<Region> Tuning { get; set; }
        public Dictionary<int, PATableInfo> PATable { get; set; }

        public RoadNetwork()
        {
            Road = new RoadGraph(false);
            PoIs = new List<Vertex>();
            BorderPoints = new Dictionary<Vertex, Edge<Vertex>>();
            NVD = new Dictionary<Vertex, VoronoiCell>();
            Latency = new List<Region>();
            Tuning = new List<Region>();
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

        public void ComputeEBTable()
        {
            EBTable = new CountingTable(Road, PoIs);
            EBTable.ComputeTables(Regions);
        }

        public void SearchKNN(int k)
        {
            int regionId = -1;
            do
            {
                QueryPoint = Road.PickQueryPoint();
                regionId = VQTree.searchRegion(QueryPoint);
            } 
            while (regionId == -1);

            double upperBound = EBTable.GetUpperBound(regionId, k);

            Queue<Region> cList = new Queue<Region>();
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int position = random.Next(0, Regions.Count - 1);

            for (int i = 0; i < Regions.Count; i++)
            {
                int index = (position + i) % Regions.Count;
                if (EBTable.CanTune(index, regionId, upperBound)) cList.Enqueue(Regions[index]);
            }

            RoadGraph graph = new RoadGraph(false);
            int start = cList.First().Id;
            int end = start;
            Tuning.Clear();
            while (cList.Count > 0)
            {
                Region region = cList.Dequeue();
                if (!EBTable.CanTune(region.Id, regionId, upperBound)) continue;
                Tuning.Add(region);
                end = region.Id;

                if (region.Id == regionId)
                {
                    graph.AddGraph(region.Road);
                    EBTable.Initialize(graph);
                    if (region.Id >= regionId) upperBound = EBTable.UpdateUpperBound(QueryPoint, k, upperBound);
                    graph = EBTable.PruneGraphVertices(QueryPoint, upperBound, k);
                }
                else
                {
                    EBTable.Initialize(region.Road);
                    graph.AddGraph(EBTable.PruneRegionVertices(region, QueryPoint, upperBound, k));
                }
            }

            Latency.Clear();
            while (start % Regions.Count() != end)
            {
                Latency.Add(Regions[start % Regions.Count()]);
                start++;
            }
            Latency.Add(Regions[end]);

            EBTable.Initialize(graph);
            Answers = EBTable.GetKNN(QueryPoint, k);
        }

        public String GetSize(object obj, double packetSize)
        {
            double packetCount = Math.Ceiling(Parser.ObjectToByteArray(obj).Count() / packetSize);
            return packetCount.ToString();
        }

        public void GenerateSN()
        {
            RoadGraph road = new RoadGraph(false);
            foreach (KeyValuePair<int, Region> region in Regions)
            {
                road.AddGraph(region.Value.Road);
            }

            CountingTable counting = new CountingTable(road);
            Shortcut = counting.GenerateSN(Regions);
        }

        public void ComputePATable()
        {
            PATable = new Dictionary<int, PATableInfo>();
            foreach (KeyValuePair<int, Region> region in Regions)
            {
                List<Vertex> vertices = region.Value.Road.Graph.Vertices.ToList();
                vertices = vertices.OrderBy(o => o.Coordinate.Longitude).ToList();
                double x = vertices.First().Coordinate.Longitude;
                double width = vertices.Last().Coordinate.Longitude - x;

                vertices = vertices.OrderBy(o => o.Coordinate.Latitude).ToList();
                double y = vertices.Last().Coordinate.Latitude;
                double height = y - vertices.First().Coordinate.Latitude;

                PATableInfo tableInfo = new PATableInfo(region.Value.PoIs.Count, new MBR(x, y, width, height));
                PATable.Add(region.Key, tableInfo);
            }
        }

        public void SaveEBTable()
        {
            FileIO.SaveEBTable(Parser.ParseCountingTable(PoIs, EBTable));
        }

        public void AddEBTable()
        {
            EBTableInfo tableInfo = FileIO.ReadEBTableFile();
            EBTable = new CountingTable(Road, PoIs);
            EBTable.MinTable = tableInfo.MinTable;
            EBTable.MaxCountTable = tableInfo.MaxCountTable;
        }

        public void PASearch(int k)
        {
            QueryPoint = Road.PickQueryPoint();
            CountingTable counting = new CountingTable(PoIs);
            counting.ComputePAMinMax(Shortcut, PATable, QueryPoint, k);

            double upperBound = 0;
            Queue<Region> cList = new Queue<Region>();

            Random random = new Random(Guid.NewGuid().GetHashCode());
            int position = random.Next(0, Regions.Count-1);
            int count = 0;

            foreach (KeyValuePair<int, double> max in counting.PAMax.OrderBy(o => o.Value))
            {
                upperBound = max.Value;
                count += PATable[max.Key].PoICount - 1;
                int temp = 0;

                for (int i = 0; i < Regions.Count; i++)
                {
                    int index = (position + i) % Regions.Count;
                    
                    if (counting.PAMin[index] <= upperBound)
                    {
                        temp++;
                        cList.Enqueue(Regions[index]);
                    }
                    //else counting.ShortcutNetwork.Shortcut.Remove(index);
                }

                if (count + temp >= k) break;
            }

            int start = cList.First().Id;
            int end = start;
            Tuning.Clear();
            while (cList.Count > 0)
            {
                Region region = cList.Dequeue();
                if (counting.PAMin[region.Id] > upperBound)
                {
                    //counting.ShortcutNetwork.Shortcut.Remove(region.Id);
                    continue;
                }
                Tuning.Add(region);
                end = region.Id;

                if (region.Road.Graph.ContainsVertex(QueryPoint))
                {
                    counting.PADirect.Graph.Clear();

                    //foreach (Edge<Vertex> edge in counting.DirectEdges) counting.PARoadGraph.Graph.RemoveEdge(edge);
                    //counting.DirectEdges.Clear();
                }
                counting.ShortcutNetwork.Shortcut.Remove(region.Id);
                counting.PARoadGraph.AddGraph(region.Road);

                
                upperBound = counting.UpdateUpperBoundPA(QueryPoint, upperBound, k);
            }

            Latency.Clear();
            while (start % Regions.Count() != end)
            {
                Latency.Add(Regions[start % Regions.Count()]);
                start++;
            }
            Latency.Add(Regions[end]);

            //counting.Initialize(counting.PARoadGraph);
            Answers = counting.GetKNN(QueryPoint, k);
        }
    }
}

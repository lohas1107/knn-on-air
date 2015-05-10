using KNNonAir.Access;
using KNNonAir.Domain.Entity;
using KNNonAir.Domain.Service;
using QuickGraph;
using System.Collections.Generic;
using System;
using System.IO;

namespace KNNonAir.Domain.Context
{
    public class Model
    {
        public RoadGraph Road { get; set; }
        public List<Vertex> PoIs { get; set; }
        public Dictionary<Vertex, VoronoiCell> NVD { get; set; }
        public Dictionary<int, Region> Regions { get; set; }

        public Strategy CurrentStrategy { get; set; }
        public StrategyEB EB { get; set; }
        public StrategyPA PA { get; set; }
        public StrategyNPI NPI { get; set; }

        public List<Vertex> Answers { get; set; }

        public Model()
        {
            Road = new RoadGraph(false);
            PoIs = new List<Vertex>();
        }

        public void LoadRoads()
        {
            List<Edge<Vertex>> edgeList = Parser.ParseRoadData(FileIO.ReadGeoJsonFile());
            if (edgeList != null) Road.LoadRoads(edgeList);
        }

        public void LoadPoIs()
        {
            List<Vertex> poiList = Parser.ParsePoIData(FileIO.ReadGeoJsonFile());
            if (poiList == null) return;

            foreach (Vertex poi in poiList)
            {
                Vertex adjustedPoI = Road.AdjustPoIToEdge(poi);
                if (adjustedPoI != null) PoIs.Add(adjustedPoI);
            }
        }

        public void GenerateNVD()
        {
            foreach (Vertex poi in PoIs)
            {
                PathTree pathTree = new PathTree(poi, Road);
                pathTree.GenerateNVC();
                VoronoiCell nvc = pathTree.FindNVC();
                NVD.Add(poi, nvc);
            }
        }

        public void AddNVD(string filepath)
        {
            List<NVCInfo> nvcList = FileIO.ReadNVDFile(filepath);

            if (nvcList != null)
            {
                Road = Parser.ParseNVCInfoToGraph(nvcList);
                NVD = Parser.ParseNVCInfoToNVD(nvcList);
                PoIs = Parser.ParsePoIInfo(nvcList);
            }
        }

        public void Partition(int frames)
        {
            KdTree kdTree = new KdTree(NVD, frames);
            Regions = kdTree.Regions;
        }

        public void ChangeStrategy(string text)
        {
            if (text == "EB") CurrentStrategy = EB;
            else if (text == "PA") CurrentStrategy = PA;
            else if (text == "NPI") CurrentStrategy = NPI;
        }

        public void InitializeStrategy(string text)
        {
            EB = new StrategyEB(Road, PoIs, Regions);
            PA = new StrategyPA(Road, PoIs, Regions);
            NPI = new StrategyNPI(Road, PoIs, Regions);
            ChangeStrategy(text);
        }

        public void GenerateIndex()
        {
            CurrentStrategy.GenerateIndex();
        }

        public void ComputeTable()
        {
            CurrentStrategy.ComputeTable();
        }

        public void SaveEBTable()
        {
            FileIO.SaveEBTable(Parser.ParseCountingTable(PoIs, EB));
        }

        public void AddEBTable(String filepath)
        {
            EBTableInfo tableInfo = FileIO.ReadEBTableFile(filepath);
            EB.MinTable = tableInfo.MinTable;
            EB.MaxCountTable = tableInfo.MaxCountTable;
        }

        public void SearchKNN(int k)
        {
            Answers = CurrentStrategy.SearchKNN(k);
            CurrentStrategy.Evaluate();
        }

        public double GetSize(object obj, double packetSize)
        {
            return Math.Ceiling(Parser.ObjectToByteArray(obj) / packetSize);
        }
    }
}

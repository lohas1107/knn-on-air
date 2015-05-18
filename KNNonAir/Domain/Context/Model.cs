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

        public Algorithm CurrentAlgorithm { get; set; }
        public AlgorithmEB EB { get; set; }
        public AlgorithmPA PA { get; set; }
        public AlgorithmNPI NPI { get; set; }

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

        public void ChangeAlgorithm(string text)
        {
            if (text == "EB") CurrentAlgorithm = EB;
            else if (text == "PA") CurrentAlgorithm = PA;
            else if (text == "NPI") CurrentAlgorithm = NPI;
        }

        public void InitializeAlgorithm(string text)
        {
            EB = new AlgorithmEB(Road, PoIs);
            PA = new AlgorithmPA(Road, PoIs);
            NPI = new AlgorithmNPI(Road, PoIs);
            ChangeAlgorithm(text);
        }

        public void Partition(int frames)
        {
            if (CurrentAlgorithm is AlgorithmNPI) CurrentAlgorithm.Partition(Road, frames);
            else CurrentAlgorithm.Partition(NVD, frames);
        }

        public void GenerateIndex()
        {
            CurrentAlgorithm.GenerateIndex();
        }

        public void ComputeTable()
        {
            CurrentAlgorithm.ComputeTable();
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
            Answers = CurrentAlgorithm.SearchKNN(k);
            CurrentAlgorithm.Evaluate();
        }

        public double GetSize(object obj, double packetSize)
        {
            return Math.Ceiling(Parser.ObjectToByteArray(obj) / packetSize);
        }
    }
}

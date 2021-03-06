﻿using KNNonAir.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using QuickGraph;

namespace KNNonAir.Domain.Service
{
    [Serializable]
    public class AlgorithmEB : Algorithm, ISerializable
    {
        public VQTree VQTree { get; set; }

        public Dictionary<int, Dictionary<int, double>> MinTable { get; set; }
        public Dictionary<int, Tuple<int, double>> MaxCountTable { get; set; }

        public AlgorithmEB(RoadGraph road, List<Vertex> pois) : base(road, pois)
        {
            MinTable = new Dictionary<int, Dictionary<int, double>>();
            MaxCountTable = new Dictionary<int, Tuple<int, double>>();
        }

        public AlgorithmEB(SerializationInfo info, StreamingContext context)
        {
            MinTable = (Dictionary<int, Dictionary<int, double>>)info.GetValue("MinTable", typeof(Dictionary<int, Dictionary<int, double>>));
            MaxCountTable = (Dictionary<int, Tuple<int, double>>)info.GetValue("MaxCountTable", typeof(Dictionary<int, Tuple<int, double>>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MinTable", MinTable);
            info.AddValue("MaxCountTable", MaxCountTable);
        }

        public override void Partition(Dictionary<Vertex, VoronoiCell> nvd, int amount)
        {
            base.Partition(nvd, amount);

            KdTree kdTree = new KdTree(nvd, amount);
            Regions = kdTree.Regions;
        }

        public override void GenerateIndex()
        {
            List<Vertex> borderPoints = new List<Vertex>();
            MBR mbr = new MBR(Road.Graph.Vertices);

            foreach (Region region in Regions)
            {
                foreach (Vertex borderPoint in region.BorderPoints)
                {
                    if (!borderPoints.Contains(borderPoint)) borderPoints.Add(borderPoint);
                }
                mbr.AddVertices(region.Road.Graph.Vertices);
            }

            VQTree = new VQTree(borderPoints, mbr);
        }

        public override void ComputeTable()
        {
            for (int from = 0; from < Regions.Count; from++)
            {
                Dictionary<int, double> tableItem = new Dictionary<int, double>();

                for (int to = from + 1; to < Regions.Count; to++)
                {
                    double min = ComputeMin(Regions[from], Regions[to]);
                    tableItem.Add(Regions[to].Id, min);
                }
                MinTable.Add(from, tableItem);

                Tuple<int, double> maxCount = new Tuple<int, double>(Regions[from].PoIs.Count, ComputeMax(Regions[from]));
                MaxCountTable.Add(from, maxCount);
            }            
        }

        private double ComputeMin(Region fromRegion, Region toRegion)
        {
            double minDistance = double.MaxValue;

            foreach (Vertex fromBorder in fromRegion.BorderPoints)
            {
                _dijkstra.Compute(fromBorder);

                foreach (Vertex poi in toRegion.PoIs)
                {
                    if (_dijkstra.Distances.ContainsKey(poi) && _dijkstra.Distances[poi] < minDistance)
                    {
                        minDistance = _dijkstra.Distances[poi];
                    }
                }
            }

            return minDistance;
        }

        private double ComputeMax(Region fromRegion)
        {
            double maxDistance = double.MinValue;

            foreach (Vertex fromBorder in fromRegion.BorderPoints)
            {
                _dijkstra.Compute(fromBorder);

                foreach (Vertex poi in fromRegion.PoIs)
                {
                    if (_dijkstra.Distances.ContainsKey(poi) && _dijkstra.Distances[poi] > maxDistance)
                    {
                        maxDistance = _dijkstra.Distances[poi];
                    }
                }
            }

            return maxDistance;
        }
        
        public bool CanTune(int id, int regionId, double upperBound)
        {
            if (id == regionId) return true;
            if (id < regionId && MinTable[id][regionId] <= upperBound) return true;
            if (id > regionId && MinTable[regionId][id] <= upperBound) return true;

            return false;
        }

        private double GetUpperBound(int regionId, int k)
        {
            if (MaxCountTable.Count() == 0 && MinTable.Count() == 0) return 0;

            double ub = MaxCountTable[regionId].Item2;
            int count = 0;
            List<double> minList = new List<double>();

            for (int i = 0; i < MaxCountTable.Count; i++)
            {
                if (i < regionId) minList.Add(MinTable[i][regionId]);
                else if (i > regionId) minList.Add(MinTable[regionId][i]);
            }

            minList.Sort();
            double min = minList.First();

            while (count + MaxCountTable[regionId].Item1 < k && count < MaxCountTable.Count && minList.Count > 0)
            {
                count = 0;
                min = minList.First();
                for (int i = 0; i < MaxCountTable.Count; i++)
                {
                    if (CanTune(i, regionId, ub + min)) count++;
                }
                minList.RemoveAt(0);
            }

            if (count + MaxCountTable[regionId].Item1 < k) return double.MaxValue;
            else return ub + min;
        }

        private double UpdateUpperBound(int k, double upperBound)
        {
            List<Vertex> knn = GetKNN(QueryPoint, k);
            if (knn.Count() < k) return upperBound;
            if (_dijkstra.Distances[knn.Last()] < upperBound) return _dijkstra.Distances[knn.Last()];
            else return upperBound;
        }

        //private RoadGraph PruneRegionVertices(Region region, Vertex queryPoint, double upperBound, int k)
        //{
        //    List<Vertex> savedVertex = new List<Vertex>();
        //    savedVertex.Add(queryPoint);

        //    foreach (Vertex border in region.BorderPoints)
        //    {
        //        _dijkstra.Compute(border);

        //        int count = 0;
        //        foreach (KeyValuePair<Vertex, double> kvp in _dijkstra.Distances.OrderBy(o => o.Value))
        //        {
        //            if (count < k && kvp.Value <= upperBound - Arithmetics.GetDistance(queryPoint, border))
        //            {
        //                if (PoIs.Contains(kvp.Key)) count++;
        //                if (!savedVertex.Contains(kvp.Key)) savedVertex.Add(kvp.Key);
        //            }
        //        }
        //    }

        //    RoadGraph road = new RoadGraph(false);
        //    foreach (Edge<Vertex> edge in region.Road.Graph.Edges)
        //    {
        //        if (savedVertex.Contains(edge.Source) || savedVertex.Contains(edge.Target)) road.Graph.AddVerticesAndEdge(edge);
        //    }

        //    return road;
        //}

        private RoadGraph PruneGraphVertices(double upperBound, int k)
        {
            List<Vertex> savedVertex = new List<Vertex>();
            savedVertex.Add(QueryPoint);

            _dijkstra.Compute(QueryPoint);

            int count = 0;
            foreach (KeyValuePair<Vertex, double> kvp in _dijkstra.Distances.OrderBy(o => o.Value))
            {
                if (count < k && kvp.Value <= upperBound)
                {
                    if (PoIs.Contains(kvp.Key)) count++;
                    if (!savedVertex.Contains(kvp.Key)) savedVertex.Add(kvp.Key);
                }
            }

            RoadGraph road = new RoadGraph(false);
            foreach (Edge<Vertex> edge in _dijkstra.Road.Graph.Edges)
            {
                if (savedVertex.Contains(edge.Source) || savedVertex.Contains(edge.Target)) road.Graph.AddVerticesAndEdge(edge);
            }

            return road;
        }

        public override List<Vertex> SearchKNN(int k)
        {
            int regionId = -1;

            while (regionId == -1)
            {
                InitializeQuery();
                regionId = VQTree.searchRegion(QueryPoint);
            }

            double upperBound = GetUpperBound(regionId, k);

            Queue<Region> cList = new Queue<Region>();

            if (k == 1)
            {
                cList.Enqueue(Regions[regionId]);
            }
            else
            {
                for (int i = 0; i < Regions.Count; i++)
                {
                    if (CanTune(i, regionId, upperBound)) cList.Enqueue(Regions[i]);
                }
            }
            Start = 0;
            End = Start;

            RoadGraph graph = new RoadGraph(false);

            while (cList.Count > 0)
            {
                Region region = cList.Dequeue();
                if (!CanTune(region.Id, regionId, upperBound)) continue;
                Tuning.Add(region);
                End = region.Id;

                graph.AddGraph(region.Road);
                UpdateVisitGraph(graph);

                if (graph.Graph.Vertices.Contains(QueryPoint))
                {
                    upperBound = UpdateUpperBound(k, upperBound);
                }
            }

            UpdateVisitGraph(graph);
            return GetKNN(QueryPoint, k);
        }

        public override void Evaluate()
        {
            base.Evaluate();

            for (int i = Position; i < Regions.Count; i++)
            {
                Overflow.Add(Regions[i]);
            }
        }
    }
}

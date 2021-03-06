﻿using KNNonAir.Domain.Entity;
using System;
using System.Collections.Generic;
using QuickGraph;

namespace KNNonAir.Domain.Service
{
    public abstract class Algorithm
    {
        private Random _random;
        protected Dijkstra _dijkstra;

        public RoadGraph Road;
        public List<Vertex> PoIs;
        public List<Region> Regions { get; set; }
        public Vertex QueryPoint { get; set; }

        public int Position { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        public List<Region> Tuning { get; set; }
        public List<Region> Latency { get; set; }
        public List<Region> Overflow { get; set; }
        public int LatencySlot { get; set; }

        public Algorithm() { }

        public Algorithm(RoadGraph road, List<Vertex> pois)
        {
            Road = road;
            PoIs = pois;

            _random = new Random(Guid.NewGuid().GetHashCode());
            _dijkstra = new Dijkstra(road);

            Latency = new List<Region>();
            Overflow = new List<Region>();
            Tuning = new List<Region>();

            LatencySlot = 0;
        }

        public virtual void UpdateVisitGraph(RoadGraph road)
        {
            _dijkstra = new Dijkstra(road);
        }

        public virtual void UpdateVisitGraph(RoadGraph road, Dictionary<Edge<Vertex>, double> distances)
        {
            _dijkstra = new Dijkstra(road, distances);
        }

        public virtual void Partition(Dictionary<Vertex, VoronoiCell> nvd, int amount) { }

        public virtual void Partition(RoadGraph road, int amount) { }

        public virtual List<Region> Schedule(List<Region> regions)
        {
            HilbertCurve hilbert = new HilbertCurve();
            return hilbert.OrderByHilbert(regions);
        }

        public abstract void GenerateIndex();

        public abstract void ComputeTable();

        public virtual void InitializeQuery()
        {
            QueryPoint = Road.PickQueryPoint();
            Position = _random.Next(0, Regions.Count - 1);
            Overflow.Clear();
            Latency.Clear();
            Tuning.Clear();
            UpdateVisitGraph(Road);
            LatencySlot = 0;
        }

        public List<Vertex> GetKNN(Vertex queryPoint, int k)
        {
            List<Vertex> knnList = new List<Vertex>();

            if (!_dijkstra.Road.Graph.ContainsVertex(queryPoint)) return knnList;
            _dijkstra.Compute(queryPoint);

            foreach (KeyValuePair<Vertex, double> kvp in _dijkstra.Distances)
            {
                if (PoIs.Contains(kvp.Key) && knnList.Count < k)
                {
                    knnList.Add(kvp.Key);
                }
                if (knnList.Count == k) break;
            }

            return knnList;
        }

        public abstract List<Vertex> SearchKNN(int k);

        public virtual void Evaluate()
        {
            int start = Start;
            while (start % Regions.Count != End)
            {
                if (Regions[start % Regions.Count].Road.Graph.VertexCount > 0)
                {
                    Latency.Add(Regions[start % Regions.Count]);
                }
                LatencySlot++;
                start++;
            }
            if (Regions[start % Regions.Count].Road.Graph.VertexCount > 0)
            {
                Latency.Add(Regions[start % Regions.Count]);
            }
            LatencySlot++;
        }
    }
}

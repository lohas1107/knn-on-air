using KNNonAir.Domain.Entity;
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
        public Dictionary<int, Region> Regions { get; set; }
        public Vertex QueryPoint { get; set; }

        public int Position { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        public List<Region> Tuning { get; set; }
        public List<Region> Latency { get; set; }
        public List<Region> Overflow { get; set; }

        public Algorithm() { }

        public Algorithm(RoadGraph road, List<Vertex> pois, Dictionary<int, Region> regions)
        {
            Road = road;
            PoIs = pois;
            Regions = regions;

            _random = new Random(Guid.NewGuid().GetHashCode());
            _dijkstra = new Dijkstra(road);

            Latency = new List<Region>();
            Overflow = new List<Region>();
            Tuning = new List<Region>();
        }

        public virtual void UpdateVisitGraph(RoadGraph road)
        {
            _dijkstra = new Dijkstra(road);
        }

        public virtual void UpdateVisitGraph(RoadGraph road, Dictionary<Edge<Vertex>, double> distances)
        {
            _dijkstra = new Dijkstra(road, distances);
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
        }

        public List<Vertex> GetKNN(Vertex queryPoint, int k)
        {
            List<Vertex> knnList = new List<Vertex>();

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
            while (Start % Regions.Count != End)
            {
                Latency.Add(Regions[Start % Regions.Count]);
                Start++;
            }
            Latency.Add(Regions[End]);

            for (int i = Position; i < Regions.Count; i++)
            {
                Overflow.Add(Regions[i]);
            }
        }
    }
}

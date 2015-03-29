using KNNonAir.Domain.Entity;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KNNonAir.Domain.Service
{
    class CountingTable
    {
        private RoadGraph _road;
        private List<Vertex> _pois;
        private Dictionary<Edge<Vertex>, double> _distances;
        private UndirectedDijkstraShortestPathAlgorithm<Vertex, Edge<Vertex>> _dijkstra;
        private UndirectedVertexDistanceRecorderObserver<Vertex, Edge<Vertex>> _distObserver;

        public Dictionary<int, Dictionary<int, double>> MinTable { get; set; }
        public Dictionary<int, Tuple<int, double>> MaxCountTable { get; set; }

        public CountingTable(RoadGraph road)
        {
            Initialize(road);
        }

        public CountingTable(RoadGraph road, List<Vertex> pois)
        {
            Initialize(road);
            _pois = pois;
        }

        public void Initialize(RoadGraph road)
        {
            _road = road;
            _distances = new Dictionary<Edge<Vertex>, double>();

            foreach (Edge<Vertex> edge in road.Graph.Edges)
            {
                _distances.Add(edge, Arithmetics.GetDistance(edge.Source, edge.Target));
            }

            _dijkstra = new UndirectedDijkstraShortestPathAlgorithm<Vertex, Edge<Vertex>>(road.Graph, AlgorithmExtensions.GetIndexer<Edge<Vertex>, double>(_distances));
            _distObserver = new UndirectedVertexDistanceRecorderObserver<Vertex, Edge<Vertex>>(AlgorithmExtensions.GetIndexer<Edge<Vertex>, double>(_distances));
            _distObserver.Attach(_dijkstra);
        }

        private void Reset()
        {
            _distObserver.Distances.Clear();
        }

        public void ComputeTables(Dictionary<int, Region> regions)
        {
            MinTable = new Dictionary<int, Dictionary<int, double>>();
            MaxCountTable = new Dictionary<int, Tuple<int, double>>();
            int from = 0;

            while (from < regions.Count())
            {
                Dictionary<int, double> tableItem = new Dictionary<int, double>();

                for (int to = from + 1; to < regions.Count(); to++)
                {
                    double min = ComputeMin(regions[from], regions[to]);

                    tableItem.Add(regions[to].Id, min);
                }
                MinTable.Add(from, tableItem);

                Tuple<int, double> maxCount = new Tuple<int, double>(regions[from].BorderPoints.Count(), ComputeMax(regions[from]));
                MaxCountTable.Add(from, maxCount);

                from++;
            }
        }

        private double ComputeMin(Region formRegion, Region toRegion)
        {
            double minDistance = double.MaxValue;

            foreach (Vertex fromBorder in formRegion.BorderPoints)
            {
                Reset();
                _dijkstra.Compute(fromBorder);
                Dictionary<Vertex, double> distances = new Dictionary<Vertex, double>(_distObserver.Distances);

                foreach (Vertex toBorder in toRegion.BorderPoints)
                {
                    Reset();
                    _dijkstra.Compute(toBorder);

                    foreach (Vertex poi in toRegion.PoIs)
                    {
                        if (!_distObserver.Distances.ContainsKey(poi)) continue;
                        if (distances[toBorder] + _distObserver.Distances[poi] < minDistance)
                        {
                            minDistance = distances[toBorder] + _distObserver.Distances[poi];
                        }
                    }
                }
            }

            return minDistance;
        }

        private double ComputeMax(Region formRegion)
        {
            double maxDistance = double.MinValue;

            foreach (Vertex fromBorder in formRegion.BorderPoints)
            {
                Reset();
                _dijkstra.Compute(fromBorder);

                foreach (Vertex poi in formRegion.PoIs)
                {
                    if (!_distObserver.Distances.ContainsKey(poi)) continue;
                    if (_distObserver.Distances[poi] > maxDistance)
                    {
                        maxDistance = _distObserver.Distances[poi];
                    }
                }
            }

            return maxDistance;
        }

        public RoadGraph PruneRegionVertices(Region region, Vertex queryPoint, double upperBound, int k)
        {
            List<Vertex> savedVertex = new List<Vertex>();
            savedVertex.Add(queryPoint);

            foreach (Vertex border in region.BorderPoints)
            {
                Reset();
                _dijkstra.Compute(border);
                _distObserver.Distances.OrderBy(o => o.Value);

                int count = 0;
                foreach (KeyValuePair<Vertex, double> kvp in _distObserver.Distances)
                {
                    if (count < k && kvp.Value <= upperBound - Arithmetics.GetDistance(queryPoint, border))
                    {
                        if (_pois.Contains(kvp.Key)) count++;
                        if (!savedVertex.Contains(kvp.Key)) savedVertex.Add(kvp.Key);
                    }
                }
            }

            RoadGraph road = new RoadGraph(false);
            foreach (Edge<Vertex> edge in region.Road.Graph.Edges)
            {
                if (savedVertex.Contains(edge.Source) && savedVertex.Contains(edge.Target)) road.Graph.AddVerticesAndEdge(edge);
            }

            return road;
        }

        public RoadGraph PruneGraphVertices(Vertex queryPoint, double upperBound, int k)
        {
            List<Vertex> savedVertex = new List<Vertex>();
            savedVertex.Add(queryPoint);

            Reset();
            _dijkstra.Compute(queryPoint);
            _distObserver.Distances.OrderBy(o => o.Value);

            int count = 0;
            foreach (KeyValuePair<Vertex, double> kvp in _distObserver.Distances)
            {
                if (count < k && kvp.Value <= upperBound)
                {
                    if (_pois.Contains(kvp.Key)) count++;
                    if (!savedVertex.Contains(kvp.Key)) savedVertex.Add(kvp.Key);
                }
            }

            RoadGraph road = new RoadGraph(false);
            foreach (Edge<Vertex> edge in _road.Graph.Edges)
            {
                if (savedVertex.Contains(edge.Source) && savedVertex.Contains(edge.Target)) road.Graph.AddVerticesAndEdge(edge);
            }

            return road;
        }

        public double UpdateUpperBound(Vertex queryPoint, int k, double upperBound)
        {
            double newUB = double.MaxValue;

            Reset();
            _dijkstra.Compute(queryPoint);
            _distObserver.Distances.OrderBy(o=>o.Value);

            int count = 0;
            foreach (KeyValuePair<Vertex, double> kvp in _distObserver.Distances)
            {
                if (_pois.Contains(kvp.Key) && count < k)
                {
                    count++;
                    newUB = kvp.Value;
                }
                else if (count == k && newUB < upperBound) return newUB;
            }

            return upperBound;
        }

        public List<Vertex> GetKNN(Vertex queryPoint, int k)
        {
            List<Vertex> knnList = new List<Vertex>();

            Reset();
            _dijkstra.Compute(queryPoint);

            foreach (KeyValuePair<Vertex, double> kvp in _distObserver.Distances)
            {
                if (_pois.Contains(kvp.Key) && knnList.Count < k) 
                    knnList.Add(kvp.Key);
                if (knnList.Count == k) break;
            }

            return knnList;
        }
    }
}

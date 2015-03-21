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
        private UndirectedDijkstraShortestPathAlgorithm<Vertex, Edge<Vertex>> _dijkstra;
        private UndirectedVertexDistanceRecorderObserver<Vertex, Edge<Vertex>> _distObserver;

        public Dictionary<int, Dictionary<int, double>> MinTable { get; set; }
        public Dictionary<int, Tuple<int, double>> MaxCountTable { get; set; }

        public CountingTable(RoadGraph road, Dictionary<int, Region> regions)
        {
            Dictionary<Edge<Vertex>, double> distances = new Dictionary<Edge<Vertex>, double>();

            foreach (Edge<Vertex> edge in road.Graph.Edges)
            {
                distances.Add(edge, Arithmetics.GetDistance(edge.Source, edge.Target));
            }

            _dijkstra = new UndirectedDijkstraShortestPathAlgorithm<Vertex, Edge<Vertex>>(road.Graph, AlgorithmExtensions.GetIndexer<Edge<Vertex>, double>(distances));
            _distObserver = new UndirectedVertexDistanceRecorderObserver<Vertex, Edge<Vertex>>(AlgorithmExtensions.GetIndexer<Edge<Vertex>, double>(distances));
            _distObserver.Attach(_dijkstra);

            MinTable = new Dictionary<int, Dictionary<int, double>>();
            MaxCountTable = new Dictionary<int, Tuple<int, double>>();

            ComputeTables(regions);
        }

        private void ComputeTables(Dictionary<int, Region> regions)
        {
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
                _dijkstra.Compute(fromBorder);

                foreach (Vertex toBorder in toRegion.BorderPoints)
                {
                    double distance = _distObserver.Distances[toBorder];
                    _dijkstra.Compute(toBorder);

                    foreach (Vertex poi in toRegion.PoIs)
                    {
                        if (!_distObserver.Distances.ContainsKey(poi)) continue;
                        if (distance + _distObserver.Distances[poi] < minDistance)
                        {
                            minDistance = _distObserver.Distances[toBorder];
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
    }
}

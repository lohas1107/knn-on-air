using KNNonAir.Domain.Entity;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using System.Collections.Generic;

namespace KNNonAir.Domain.Service
{
    public class Dijkstra
    {
        public RoadGraph Road { get; set; }
        private Dictionary<Edge<Vertex>, double> _distances;
        private UndirectedDijkstraShortestPathAlgorithm<Vertex, Edge<Vertex>> _dijkstra;
        private UndirectedVertexDistanceRecorderObserver<Vertex, Edge<Vertex>> _distObserver;

        public IDictionary<Vertex, double> Distances { get { return _distObserver.Distances; } }

        public Dijkstra(RoadGraph road)
        {
            Road = road;
            _distances = new Dictionary<Edge<Vertex>, double>();

            foreach (Edge<Vertex> edge in road.Graph.Edges)
            {
                _distances.Add(edge, Arithmetics.GetDistance(edge.Source, edge.Target));
            }

            _dijkstra = new UndirectedDijkstraShortestPathAlgorithm<Vertex, Edge<Vertex>>(Road.Graph, AlgorithmExtensions.GetIndexer<Edge<Vertex>, double>(_distances));
            _distObserver = new UndirectedVertexDistanceRecorderObserver<Vertex, Edge<Vertex>>(AlgorithmExtensions.GetIndexer<Edge<Vertex>, double>(_distances));
            _distObserver.Attach(_dijkstra);
        }


        public Dijkstra(RoadGraph road, Dictionary<Edge<Vertex>, double> distances)
        {
            Road = road;
            _distances = distances;

            _dijkstra = new UndirectedDijkstraShortestPathAlgorithm<Vertex, Edge<Vertex>>(Road.Graph, AlgorithmExtensions.GetIndexer<Edge<Vertex>, double>(_distances));
            _distObserver = new UndirectedVertexDistanceRecorderObserver<Vertex, Edge<Vertex>>(AlgorithmExtensions.GetIndexer<Edge<Vertex>, double>(_distances));
            _distObserver.Attach(_dijkstra);
        }

        public void Compute(Vertex vertex)
        {
            _distObserver.Distances.Clear();
            _dijkstra.Compute(vertex);
        }
    }
}

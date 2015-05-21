using System.Collections.Generic;
using System.Linq;
using KNNonAir.Access;
using KNNonAir.Domain.Entity;
using QuickGraph;

namespace KNNonAir.Domain.Service
{
    public class AlgorithmPA : Algorithm
    {
        private ShortcutNetwork _shortcutNetwork;

        public ShortcutNetwork ShortcutNetwork { get; set; }
        public Dictionary<int, PATableInfo> PATable { get; set; }
        public RoadGraph PAShortcut { get; set; }
        public RoadGraph PADirect { get; set; }
        public RoadGraph PARoadGraph { get; set; }
        public Dictionary<int, double> PAMin { get; set; }
        public Dictionary<int, double> PAMax { get; set; }

        public AlgorithmPA(RoadGraph road, List<Vertex> pois) : base(road, pois)
        {
            _shortcutNetwork = new ShortcutNetwork();

            PATable = new Dictionary<int, PATableInfo>();
            PAShortcut = new RoadGraph(false);
            PADirect = new RoadGraph(false);
            PARoadGraph = new RoadGraph(false);
            PAMin = new Dictionary<int, double>();
            PAMax = new Dictionary<int, double>();
        }

        public override void Partition(Dictionary<Vertex, VoronoiCell> nvd, int amount)
        {
            base.Partition(nvd, amount);

            KdTree kdTree = new KdTree(nvd, amount);
            Regions = kdTree.Regions;
        }

        public override void GenerateIndex()
        {
            RoadGraph road = new RoadGraph(false);
            foreach (Region region in Regions)
            {
                road.AddGraph(region.Road);
            }
            UpdateVisitGraph(road);

            Dictionary<int, RoadGraph> shortcutGraph = new Dictionary<int, RoadGraph>();
            Dictionary<Edge<Vertex>, double> distances = new Dictionary<Edge<Vertex>, double>();

            foreach (Region region in Regions)
            {
                RoadGraph shortcut = new RoadGraph(false);
                List<Vertex> borders = region.BorderPoints;
                for (int i = 0; i < borders.Count; i++)
                {
                    _dijkstra.Compute(borders[i]);
                    for (int j = i + 1; j < borders.Count; j++)
                    {
                        Edge<Vertex> edge = new Edge<Vertex>(borders[i], borders[j]);
                        shortcut.Graph.AddVerticesAndEdge(edge);
                        distances.Add(edge, _dijkstra.Distances[borders[j]]);
                    }
                }
                shortcutGraph.Add(region.Id, shortcut);
                _shortcutNetwork.RegionBorders.Add(region.Id, region.BorderPoints);
            }

            _shortcutNetwork.Distances = distances;
            _shortcutNetwork.Shortcut = shortcutGraph;
            ShortcutNetwork = new ShortcutNetwork(_shortcutNetwork);
        }

        public override void ComputeTable()
        {
            foreach (Region region in Regions)
            {
                List<Vertex> vertices = region.Road.Graph.Vertices.ToList();
                vertices = vertices.OrderBy(o => o.Coordinate.Longitude).ToList();
                double x = vertices.First().Coordinate.Longitude;
                double width = vertices.Last().Coordinate.Longitude - x;

                vertices = vertices.OrderBy(o => o.Coordinate.Latitude).ToList();
                double y = vertices.Last().Coordinate.Latitude;
                double height = y - vertices.First().Coordinate.Latitude;

                PATableInfo tableInfo = new PATableInfo(region.PoIs.Count, new MBR(x, y, width, height));
                PATable.Add(region.Id, tableInfo);
            }
        }

        private void UpdateVisitGraph()
        {
            PAShortcut.Graph.Clear();
            foreach (KeyValuePair<int, RoadGraph> graph in ShortcutNetwork.Shortcut)
            {
                PAShortcut.AddGraph(graph.Value);
            }

            RoadGraph road = new RoadGraph(false);
            road.AddGraph(PAShortcut);
            road.AddGraph(PADirect);
            road.AddGraph(PARoadGraph);

            Dictionary<Edge<Vertex>, double> distances = new Dictionary<Edge<Vertex>, double>(ShortcutNetwork.Distances);

            foreach (Edge<Vertex> edge in PADirect.Graph.Edges)
            {
                distances.Add(edge, Arithmetics.GetDistance(edge.Source, edge.Target));
            }

            foreach (Edge<Vertex> edge in PARoadGraph.Graph.Edges)
            {
                distances.Add(edge, Arithmetics.GetDistance(edge.Source, edge.Target));
            }

            UpdateVisitGraph(road, distances);
        }

        private void ComputePAMinMax(int k)
        {
            foreach (KeyValuePair<int, PATableInfo> paItem in PATable)
            {
                if (paItem.Value.BorderMBR.Contains(QueryPoint))
                {
                    foreach (Vertex border in ShortcutNetwork.RegionBorders[paItem.Key])
                    {
                        PADirect.Graph.AddVerticesAndEdge(new Edge<Vertex>(QueryPoint, border));
                    }
                }
            }

            UpdateVisitGraph();               
            _dijkstra.Compute(QueryPoint);

            foreach (KeyValuePair<int, List<Vertex>> borders in ShortcutNetwork.RegionBorders)
            {
                double min = double.MaxValue;
                double max = double.MinValue;

                foreach (Vertex border in borders.Value)
                {
                    if (_dijkstra.Distances[border] < min) min = _dijkstra.Distances[border];
                    if (_dijkstra.Distances[border] > max) max = _dijkstra.Distances[border];
                }

                PAMin.Add(borders.Key, min);
                PAMax.Add(borders.Key, max);
            }
        }

        private double UpdateUpperBoundPA(double upperBound, int k)
        {
            UpdateVisitGraph();

            List<Vertex> knn = GetKNN(QueryPoint, k);
            if (_dijkstra.Distances[knn.Last()] < upperBound) return _dijkstra.Distances[knn.Last()];
            else return upperBound;
        }

        public override void InitializeQuery()
        {
            base.InitializeQuery();
            ShortcutNetwork = new ShortcutNetwork(_shortcutNetwork);
            PAShortcut = new RoadGraph(false);
            PADirect = new RoadGraph(false);
            PARoadGraph = new RoadGraph(false);
            PAMax.Clear();
            PAMin.Clear();
        }

        public override List<Vertex> SearchKNN(int k)
        {
            InitializeQuery();
            ComputePAMinMax(k);
            double upperBound = 0;
            int poiCount = 0;
            Queue<Region> cList = new Queue<Region>();

            foreach (KeyValuePair<int, double> max in PAMax.OrderBy(o => o.Value))
            {
                upperBound = max.Value;
                poiCount += PATable[max.Key].PoICount - 1;
                int temp = 0;
                cList.Clear();

                for (int i = 0; i < Regions.Count; i++)
                {
                    int index = (Position + i) % Regions.Count;

                    if (PAMin[index] <= upperBound)
                    {
                        temp++;
                        cList.Enqueue(Regions[index]);
                    }
                }

                if (poiCount + temp >= k) break;
            }

            Start = cList.First().Id;
            End = Start;

            while (cList.Count > 0)
            {
                Region region = cList.Dequeue();
                if (PAMin[region.Id] > upperBound) continue;

                Tuning.Add(region);
                End = region.Id;

                if (region.Road.Graph.ContainsVertex(QueryPoint))
                {
                    PADirect.Graph.Clear();
                }
                ShortcutNetwork.Shortcut.Remove(region.Id);
                PARoadGraph.AddGraph(region.Road);


                upperBound = UpdateUpperBoundPA(upperBound, k);
            }

            return GetKNN(QueryPoint, k); ;
        }

        public override void Evaluate()
        {
            base.Evaluate();
            Overflow.Clear();
        }
    }
}

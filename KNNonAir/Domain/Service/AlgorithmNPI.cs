using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KNNonAir.Domain.Entity;

namespace KNNonAir.Domain.Service
{
    public class AlgorithmNPI : Algorithm
    {
        public int IndexCount { get; set; }
        public List<MBR> Grids { get; set; }
        public Dictionary<int, Tuple<int, double>> CountDiameterTable { get; set; }
        public Dictionary<int, Dictionary<int, Tuple<double, double>>> MinMaxTable { get; set; }

        public AlgorithmNPI(RoadGraph road, List<Vertex> pois) : base(road, pois)
        {
            IndexCount = 0;
            Regions = new List<Region>();
            CountDiameterTable = new Dictionary<int, Tuple<int, double>>();
            MinMaxTable = new Dictionary<int, Dictionary<int, Tuple<double, double>>>();
        }

        public override void Partition(RoadGraph road, int amount)
        {
            base.Partition(road, amount);

            GridPartition gridPartition = new GridPartition();
            Grids = gridPartition.Partition(road, amount);

            foreach (MBR grid in Grids)
            {
                Region region = grid.ToRegion(Road, PoIs);
                Regions.Add(region);
                if (region.Road.Graph.VertexCount > 0) IndexCount++;
            }

            Regions = Schedule(Regions);
        }

        public override void GenerateIndex()
        {
            // the very Grids
        }

        public override void ComputeTable()
        {
            foreach (Region region in Regions)
            {
                int count = region.PoIs.Count();
                double diameter = 0;

                foreach (Vertex vertex in region.Road.Graph.Vertices)
                {
                    _dijkstra.Compute(vertex);
                    double tempDiameter = _dijkstra.Distances.OrderBy(o => o.Value).Last().Value;
                    if (tempDiameter > diameter) diameter = tempDiameter;
                }

                CountDiameterTable.Add(region.Id, new Tuple<int, double>(count, diameter));
            }

            for (int from = 0; from < Regions.Count; from++)
            {
                Dictionary<int, Tuple<double, double>> item = new Dictionary<int, Tuple<double, double>>();

                for (int to = 0; to < Regions.Count; to++)
                {
                    if (from == to) continue;

                    Tuple<double, double> minMax = ComputeMinMax(Regions[from], Regions[to]);
                    item.Add(Regions[to].Id, minMax);
                }

                MinMaxTable.Add(Regions[from].Id, item);
            }  
        }

        private Tuple<double, double> ComputeMinMax(Region fromRegion, Region toRegion)
        {
            if (fromRegion.BorderPoints.Count() == 0) return new Tuple<double, double>(double.MaxValue, double.MaxValue);

            double min = double.MaxValue;
            double max = double.MinValue;

            foreach (Vertex fromBorder in fromRegion.BorderPoints)
            {
                _dijkstra.Compute(fromBorder);
                if (toRegion.BorderPoints.Count() == 0) return new Tuple<double, double>(double.MaxValue, double.MaxValue);

                foreach (Vertex toBorder in toRegion.BorderPoints)
                {
                    if (!_dijkstra.Distances.ContainsKey(toBorder)) continue;
                    
                    double distance = _dijkstra.Distances[toBorder];
                    if (distance < min) min = distance;
                    if (distance > max) max = distance;
                }
            }

            return new Tuple<double, double>(min, max);
        }

        //public override void Schedule()
        //{
        //    HilbertCurve hilbert = new HilbertCurve();
        //    Regions = hilbert.OrderByHilbert(Regions);
        //}

        private double GetUpperBound(int regionId, int k)
        {
            int count = 0;
            double upperBound = 0;

            Dictionary<int, Tuple<double, double>> minmax = MinMaxTable[regionId].OrderBy(i => i.Value.Item1).ToDictionary(i=>i.Key, i=>i.Value);
            foreach (KeyValuePair<int, Tuple<double, double>> kvp in minmax)
            {
                count += CountDiameterTable[kvp.Key].Item1;
                double tempUB = CountDiameterTable[regionId].Item2 + CountDiameterTable[kvp.Key].Item2 + MinMaxTable[regionId][kvp.Key].Item2;
                upperBound = Math.Max(upperBound, tempUB);
                if (count >= k) break;
            }

            return upperBound;
        }

        public override List<Vertex> SearchKNN(int k)
        {
            int regionId = -1;
            while (regionId == -1)
            {
                InitializeQuery();
                foreach (MBR mbr in Grids)
                {
                    if (mbr.Contains(QueryPoint))
                    {
                        regionId = mbr.Id;
                        break;
                    }
                }
            }

            double upperBound = GetUpperBound(regionId, k);

            List<Region> cList = new List<Region>();
            for (int i = 0; i < Regions.Count; i++)
            {
                int index = (Position + i) % Regions.Count;
                if (regionId == Regions[index].Id || MinMaxTable[regionId][Regions[index].Id].Item1 <= upperBound)
                {
                    cList.Add(Regions[index]);
                }
            }

            Start = Position;
            End = Regions.IndexOf(cList.First());

            RoadGraph graph = new RoadGraph(false);

            foreach (Region region in cList)
            {
                Tuning.Add(region);
                End = Regions.IndexOf(region);
                graph.AddGraph(region.Road);
            }

            UpdateVisitGraph(graph);
            return GetKNN(QueryPoint, k);
        }
    }
}

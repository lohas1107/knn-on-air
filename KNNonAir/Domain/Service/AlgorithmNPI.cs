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
        public List<MBR> Grids { get; set; }

        public AlgorithmNPI(RoadGraph road, List<Vertex> pois) : base(road, pois)
        {
            Regions = new Dictionary<int, Region>();
        }

        public override void Partition(RoadGraph road, int amount)
        {
            base.Partition(road, amount);

            GridPartition gridPartition = new GridPartition();
            Grids = gridPartition.Partition(road, amount);

            foreach (MBR grid in Grids)
            {
                Regions.Add(grid.Id, grid.ToRegion(Road));
            }
        }

        public override void GenerateIndex()
        {
            // the very Grids
        }

        public override void ComputeTable()
        {
            
        }

        public override List<Vertex> SearchKNN(int k)
        {
            return null;
        }
    }
}

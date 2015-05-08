using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KNNonAir.Domain.Entity;

namespace KNNonAir.Domain.Service
{
    class StrategyNPI : Strategy
    {
        public StrategyNPI(RoadGraph road, List<Vertex> pois, Dictionary<int, Region> regions) : base(road, pois, regions)
        {

        }

        public override void GenerateIndex()
        {
            
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

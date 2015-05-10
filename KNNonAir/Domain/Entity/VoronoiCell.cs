using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir.Domain.Entity
{
    public class VoronoiCell
    {
        public Vertex PoI { get; set; }
        public RoadGraph Road { get; set; }
        public List<Vertex> BorderPoints { get; set; }

        public VoronoiCell()
        {
            PoI = null;
            Road = new RoadGraph(false);
            BorderPoints = new List<Vertex>();
        }
    }
}

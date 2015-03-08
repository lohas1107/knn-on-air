using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir.Domain.Entity
{
    class VoronoiCell
    {
        public Vertex PoI { get; set; }
        public AdjacencyGraph<Vertex, Edge<Vertex>> Graph { get; set; }
        public List<Vertex> BorderPoints { get; set; }

        public VoronoiCell()
        {
            PoI = null;
            Graph = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
            BorderPoints = new List<Vertex>();
        }
    }
}

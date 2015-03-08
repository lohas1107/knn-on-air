using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir.Domain.Entity
{
    class Region
    {
        public List<Vertex> PoIs { get; set; }
        public AdjacencyGraph<Vertex, Edge<Vertex>> Graph { get; set; }
        public List<Vertex> BorderPoints { get; set; }

        public Region()
        {
            PoIs = new List<Vertex>();
            Graph = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
            BorderPoints = new List<Vertex>();
        }

        public void AddNVC(KeyValuePair<Vertex, VoronoiCell> nvc)
        {
            PoIs.Add(nvc.Key);

            foreach(Edge<Vertex> edge in nvc.Value.Graph.Edges)
            {
                Graph.AddVertex(edge.Source);
                Graph.AddVertex(edge.Target);
                Graph.AddEdge(edge);
            }

            foreach(Vertex borderPoint in nvc.Value.BorderPoints)
            {
                BorderPoints.Add(borderPoint);
            }
        }
    }
}

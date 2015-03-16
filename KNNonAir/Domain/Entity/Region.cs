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

        public void AddNVC(VoronoiCell nvc)
        {
            PoIs.Add(nvc.PoI);

            foreach(Edge<Vertex> edge in nvc.Graph.Edges)
            {
                if (!Graph.ContainsVertex(edge.Source)) Graph.AddVertex(edge.Source);
                if (!Graph.ContainsVertex(edge.Target)) Graph.AddVertex(edge.Target);
                Graph.AddEdge(edge);
            }

            foreach(Vertex borderPoint in nvc.BorderPoints) BorderPoints.Add(borderPoint);
        }

        public void RemoveSameBorder()
        {
            List<Vertex> tempBorders = new List<Vertex>();
            foreach (BorderPoint borderPoint in BorderPoints)
            {
                foreach(Vertex poi in borderPoint.PoIs)
                {
                    if (!PoIs.Contains(poi))
                    {
                        tempBorders.Add(borderPoint);
                        break;
                    }
                }
            }

            BorderPoints.Clear();
            foreach (Vertex border in tempBorders) BorderPoints.Add(border);
        }
    }
}

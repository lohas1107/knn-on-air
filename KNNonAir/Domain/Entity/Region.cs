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
                if (!Graph.ContainsVertex(edge.Source)) Graph.AddVertex(edge.Source);
                if (!Graph.ContainsVertex(edge.Target)) Graph.AddVertex(edge.Target);
                Graph.AddEdge(edge);
            }

            foreach(Vertex borderPoint in nvc.Value.BorderPoints)
            {
                Vertex sameVertex = ContainsBorderPoint(borderPoint);
                if (sameVertex != null) BorderPoints.Remove(sameVertex);
                else BorderPoints.Add(borderPoint);
            }
        }

        private Vertex ContainsBorderPoint(Vertex vertex)
        {
            foreach (Vertex borderPoint in BorderPoints)
            {
                if (borderPoint.Coordinate.Latitude == vertex.Coordinate.Latitude &&
                    borderPoint.Coordinate.Longitude == vertex.Coordinate.Longitude) return borderPoint;
            }

            return null;
        }
    }
}

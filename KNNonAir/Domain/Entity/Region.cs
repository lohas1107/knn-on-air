using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir.Domain.Entity
{
    class Region
    {
        public int Id { get; set; }
        public List<Vertex> PoIs { get; set; }
        public RoadGraph Road { get; set; }
        public List<Vertex> BorderPoints { get; set; }

        public Region()
        {
            Id = -1;
            PoIs = new List<Vertex>();
            Road = new RoadGraph(false);
            BorderPoints = new List<Vertex>();
        }

        public void AddNVC(VoronoiCell nvc)
        {
            PoIs.Add(nvc.PoI);

            foreach(Edge<Vertex> edge in nvc.Road.Graph.Edges)
            {
                if (!Road.Graph.ContainsVertex(edge.Source)) Road.Graph.AddVertex(edge.Source);
                if (!Road.Graph.ContainsVertex(edge.Target)) Road.Graph.AddVertex(edge.Target);
                Road.Graph.AddEdge(edge);
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

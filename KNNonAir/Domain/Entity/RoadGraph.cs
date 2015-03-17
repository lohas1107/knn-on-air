using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace KNNonAir.Domain.Entity
{
    class RoadGraph<TVertex, TEdge> : UndirectedGraph<TVertex, TEdge> where TEdge : QuickGraph.IEdge<TVertex>
    {
        public RoadGraph(bool allowParallelEdges) : base(allowParallelEdges) { }

        public RoadGraph<Vertex, Edge<Vertex>> Clone()
        {
            RoadGraph<Vertex, Edge<Vertex>> graph = new RoadGraph<Vertex, Edge<Vertex>>(false);

            foreach (IEdge<Vertex> edge in Edges)
            {
                if (!graph.ContainsVertex(edge.Source)) graph.AddVertex(edge.Source);
                if (!graph.ContainsVertex(edge.Target)) graph.AddVertex(edge.Target);
                graph.AddEdge((Edge<Vertex>)edge);
            }

            return graph;
        }
    }
}

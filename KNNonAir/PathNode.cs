using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir
{
    class PathNode
    {
        public Vertex Vertex { get; set; }
        public Edge<Vertex> Edge { get; set; }
        public double Distance 
        {
            get
            {
                if (Parent == null) return 0;
                return Parent.Distance + Arithmetics.CalculateDistance(Edge.Source.Coordinate, Edge.Target.Coordinate);
            }
        }
        public PathNode Parent { get; set; }
        public List<PathNode> Children { get; set; }

        public PathNode()
        {
            Vertex = null;
            Edge = null;
            Parent = null;
            Children = new List<PathNode>();
        }

        public void FindPath(AdjacencyGraph<Vertex, Edge<Vertex>> graph, PathNodeHandler findPathCompleted)
        {
            foreach (Edge<Vertex> edge in graph.Edges)
            {
                if (!edge.IsAdjacent(Vertex)) continue;
                if (Parent != null && edge.GetOtherVertex(Vertex) == Parent.Vertex) continue;

                PathNode child = new PathNode();
                child.Vertex = edge.GetOtherVertex(Vertex);
                child.Edge = new Edge<Vertex>(Vertex, child.Vertex);
                child.Parent = this;

                Children.Add(child);
            }

            foreach (PathNode child in Children)
            {
                if (child.Vertex.GetType() == new InterestPoint().GetType()) findPathCompleted(child);
                else child.FindPath(graph, findPathCompleted);
            }
        }

        public void InsertBorderPoint(double halfDistance)
        {
            if (Parent.Distance < halfDistance)
            {
                Parent.Children.Remove(this);

                Vertex divisionPoint = Arithmetics.FindDivisionPoint(halfDistance - Parent.Distance, Edge.Source.Coordinate, Edge.Target.Coordinate);
                
                PathNode borderPoint = new PathNode();
                borderPoint.Vertex = new BorderPoint(divisionPoint.Coordinate.Latitude, divisionPoint.Coordinate.Longitude);
                borderPoint.Edge = new Edge<Vertex>(Parent.Vertex, divisionPoint);
                borderPoint.Parent = Parent;

                Parent.Children.Add(borderPoint);
            }
            else if (Parent.Distance == halfDistance)
            {
                Parent.Children.Remove(this);
                Parent.Vertex = new BorderPoint(Parent.Vertex.Coordinate.Latitude, Parent.Vertex.Coordinate.Longitude);
            }
            else Parent.InsertBorderPoint(halfDistance);
        }

        public void LoadNVC(VoronoiCell nvc)
        {
            if (Edge != null)
            {
                nvc.Graph.AddVertex(Edge.Source);
                nvc.Graph.AddVertex(Edge.Target);
                nvc.Graph.AddEdge(Edge);
            }

            if (Children.Count == 0) nvc.BorderPoints.Add(Vertex);
            else { foreach (PathNode child in Children) child.LoadNVC(nvc); }
        }
    }
}

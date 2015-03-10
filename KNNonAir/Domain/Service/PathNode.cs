using KNNonAir.Domain.Entity;
using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir.Domain.Service
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
                return Parent.Distance + Arithmetics.GetDistance(Edge.Source, Edge.Target);
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

        public void FindPathsByRange(AdjacencyGraph<Vertex, Edge<Vertex>> graph, double range, PathNodeHandler findPathCompleted)
        {
            findPathCompleted(this);
            AddChild(graph);

            foreach (PathNode child in Children)
            {
                if (child.Distance > range) continue;
                else child.FindPathsByRange(graph, range, findPathCompleted);
            }
        }

        public void FindPath(AdjacencyGraph<Vertex, Edge<Vertex>> graph, Vertex poi, PathNodeHandler findPathCompleted)
        {
            AddChild(graph);

            foreach (PathNode child in Children)
            {
                if (child.Vertex.GetType() == typeof(InterestPoint)) findPathCompleted(child);
                else if (child.Vertex.GetType() == typeof(BorderPoint) && ((BorderPoint)child.Vertex).PoIs.Contains(poi)) continue;
                else if (Children.Count > 1) child.FindPath(graph.Clone(), poi, findPathCompleted);
                else child.FindPath(graph, poi, findPathCompleted);
            }
        }

        private void AddChild(AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            Edge<Vertex> myEdge = null;
            if (Edge != null && graph.TryGetEdge(Edge.Source, Edge.Target, out myEdge)) graph.RemoveEdge(myEdge);
            if (Edge != null && graph.TryGetEdge(Edge.Target, Edge.Source, out myEdge)) graph.RemoveEdge(myEdge);

            foreach (Edge<Vertex> edge in graph.Edges)
            {
                if (!edge.IsAdjacent(Vertex)) continue;

                PathNode child = new PathNode();
                child.Vertex = edge.GetOtherVertex(Vertex);
                child.Edge = new Edge<Vertex>(Vertex, child.Vertex);
                child.Parent = this;
                Children.Add(child);
            }
        }

        public void InsertBorderPoint(double halfDistance, Vertex poi, Vertex leaf, BorderPointHandler findBorderPointCompleted)
        {
            if (Parent.Distance < halfDistance)
            {
                Parent.Children.Remove(this);

                Vertex divisionPoint = Arithmetics.FindDivisionPoint(halfDistance - Parent.Distance, Edge.Source, Edge.Target);
                
                PathNode borderNode = new PathNode();
                borderNode.Vertex = new BorderPoint(divisionPoint.Coordinate.Latitude, divisionPoint.Coordinate.Longitude);
                borderNode.Edge = new Edge<Vertex>(Parent.Vertex, borderNode.Vertex);
                borderNode.Parent = Parent;
                ((BorderPoint)borderNode.Vertex).PoIs.Add(poi);
                ((BorderPoint)borderNode.Vertex).PoIs.Add(leaf);

                Parent.Children.Add(borderNode);
                findBorderPointCompleted(borderNode.Vertex, Edge);
            }
            else if (Parent.Distance == halfDistance)
            {
                Parent.Children.Remove(this);
                Parent.Vertex = new BorderPoint(Parent.Vertex.Coordinate.Latitude, Parent.Vertex.Coordinate.Longitude);
                ((BorderPoint)Parent.Vertex).PoIs.Add(poi);
                ((BorderPoint)Parent.Vertex).PoIs.Add(leaf);

                if (Parent.Edge != null)
                {
                    Parent.Edge = new Edge<Vertex>(Parent.Parent.Vertex, Parent.Vertex);
                    findBorderPointCompleted(Parent.Vertex, Parent.Edge);
                }
            }
            else Parent.InsertBorderPoint(halfDistance, poi, leaf, findBorderPointCompleted);
        }

        public void LoadNVC(VoronoiCell nvc)
        {
            if (Edge != null)
            {
                if (!nvc.Graph.ContainsVertex(Edge.Source)) nvc.Graph.AddVertex(Edge.Source);
                if (!nvc.Graph.ContainsVertex(Edge.Target)) nvc.Graph.AddVertex(Edge.Target);
                nvc.Graph.AddEdge(Edge);
            }

            if (Children.Count == 0 && Vertex.GetType() == typeof(BorderPoint)) nvc.BorderPoints.Add(Vertex);
            else { foreach (PathNode child in Children) child.LoadNVC(nvc); }
        }
    }
}

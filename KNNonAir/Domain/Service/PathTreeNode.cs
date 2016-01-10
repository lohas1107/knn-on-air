using KNNonAir.Domain.Entity;
using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir.Domain.Service
{
    class PathTreeNode
    {
        public Vertex Vertex { get; set; }
        public Edge<Vertex> Edge { get; set; }
        public PathTreeNode Parent { get; set; }
        public double Distance { get; set; }
        public List<PathTreeNode> Children { get; set; }

        public PathTreeNode()
        {
            Vertex = null;
            Edge = null;
            Parent = null;
            Distance = 0;
            Children = new List<PathTreeNode>();
        }

        private void AddChild(AdjacencyGraph<Vertex, Edge<Vertex>> bidirectionRoad)
        {
            Edge<Vertex> myEdge = null;
            if (Edge != null && bidirectionRoad.TryGetEdge(Edge.Source, Edge.Target, out myEdge)) bidirectionRoad.RemoveEdge(myEdge);
            if (Edge != null && bidirectionRoad.TryGetEdge(Edge.Target, Edge.Source, out myEdge)) bidirectionRoad.RemoveEdge(myEdge);

            IEnumerable<Edge<Vertex>> outEdges = new List<Edge<Vertex>>();
            bidirectionRoad.TryGetOutEdges(Vertex, out outEdges);
            foreach (Edge<Vertex> edge in outEdges)
            {
                PathTreeNode child = new PathTreeNode();
                child.Vertex = edge.Target;
                child.Edge = edge;
                child.Parent = this;
                child.Distance = Distance + Arithmetics.GetDistance(child.Edge.Source, child.Edge.Target);
                Children.Add(child);
            }
        }

        //public void FindPathsByRange(RoadGraph graph, double range, PathNodeHandler findPathCompleted)
        //{
        //    findPathCompleted(this);
        //    AddChild(graph);

        //    foreach (PathTreeNode child in Children)
        //    {
        //        if (child.Distance > range) continue;
        //        else child.FindPathsByRange(graph, range, findPathCompleted);
        //    }
        //}

        public void FindPath(AdjacencyGraph<Vertex, Edge<Vertex>> bidirectionRoad, Vertex poi, PathNodeHandler findPathCompleted)
        {
            AddChild(bidirectionRoad);

            foreach (PathTreeNode child in Children)
            {
                if (child.Vertex is InterestPoint) findPathCompleted(child);
                else if (child.Vertex is BorderPoint && ((BorderPoint)child.Vertex).PoIs.Contains(poi)) continue;
                else if (Children.Count > 1) child.FindPath(bidirectionRoad.Clone(), poi, findPathCompleted);
                else child.FindPath(bidirectionRoad, poi, findPathCompleted);
            }
        }

        public void InsertBorderPoint(double halfDistance, Vertex poi, Vertex leaf, BorderPointHandler findBorderPointCompleted)
        {
            if (Parent.Distance < halfDistance)
            {
                Parent.Children.Remove(this);

                Vertex divisionPoint = Arithmetics.FindDivisionPoint(halfDistance - Parent.Distance, Edge.Source, Edge.Target);
                
                PathTreeNode borderNode = new PathTreeNode();
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
            if (Edge != null) nvc.Road.Graph.AddVerticesAndEdge(Edge);

            if (Children.Count == 0 && Vertex is BorderPoint) nvc.BorderPoints.Add(Vertex);
            else { foreach (PathTreeNode child in Children) child.LoadNVC(nvc); }
        }
    }
}

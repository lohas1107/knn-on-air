using KNNonAir.Domain.Entity;
using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir.Domain.Service
{
    class PathTree
    {
        private PathTreeNode _root;
        private Dictionary<Vertex, PathTreeNode> _leaves;
        //private List<Vertex> _pathVertexs;
        private bool _isRepeat;
        private RoadGraph _road;
        private Dictionary<Vertex, Edge<Vertex>> _borderPoints;

        private event PathNodeHandler FindPathCompleted;
        private event BorderPointHandler FindBorderPointCompleted;

        public PathTree(Vertex root, RoadGraph road)
        {
            _road = road;
            _root = new PathTreeNode() { Vertex = root };
            _leaves = new Dictionary<Vertex, PathTreeNode>();
            //_pathVertexs = new List<Vertex>();
            _isRepeat = true;
            _borderPoints = new Dictionary<Vertex, Edge<Vertex>>();

            FindPathCompleted += AddLeaf;
            FindBorderPointCompleted += AddBorderPoint;
        }

        public void GenerateNVC()
        {
            while (_isRepeat)
            {
                AdjacencyGraph<Vertex, Edge<Vertex>>bidirectionRoad = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);
                foreach (Edge<Vertex> edge in _road.Graph.Edges)
                {
                    bidirectionRoad.AddVerticesAndEdge(edge);
                    bidirectionRoad.AddVerticesAndEdge(new Edge<Vertex>(edge.Target, edge.Source));
                }

                FindPaths(bidirectionRoad);
                FindBorderPoint();                

                foreach (KeyValuePair<Vertex, Edge<Vertex>> kvp in _borderPoints)
                {
                    Edge<Vertex> brokenEdge = null;
                    if (_road.Graph.TryGetEdge(kvp.Value.Source, kvp.Value.Target, out brokenEdge)) _road.InsertVertex(kvp.Key, brokenEdge);
                    else if (_road.Graph.TryGetEdge(kvp.Value.Target, kvp.Value.Source, out brokenEdge)) _road.InsertVertex(kvp.Key, brokenEdge);
                    else _isRepeat = true;
                }
                _borderPoints.Clear();
            }
        }

        private void FindPaths(AdjacencyGraph<Vertex, Edge<Vertex>> bidirectionRoad)
        {
            _isRepeat = false;
            _leaves.Clear();

            if (_root.Children.Count > 0) _root.Children.Clear();
            _root.FindPath(bidirectionRoad, _root.Vertex, FindPathCompleted);
        }

        private void AddLeaf(PathTreeNode node)
        {
            if (_leaves.ContainsKey(node.Vertex))
            {
                _isRepeat = true;
                if (node.Distance < _leaves[node.Vertex].Distance) _leaves[node.Vertex] = node;
            }
            else _leaves.Add(node.Vertex, node);
        }

        private void FindBorderPoint()
        {
            foreach (KeyValuePair<Vertex, PathTreeNode> leaf in _leaves)
            {
                leaf.Value.InsertBorderPoint(leaf.Value.Distance / 2, _root.Vertex, leaf.Key, FindBorderPointCompleted);
            }
        }

        private void AddBorderPoint(Vertex borderPoint, Edge<Vertex> edge)
        {
            _borderPoints.Add(borderPoint, edge);
        }

        public VoronoiCell FindNVC()
        {
            VoronoiCell nvc = new VoronoiCell() { PoI = _root.Vertex };
            _root.LoadNVC(nvc);

            foreach (Edge<Vertex> pathEdge in nvc.Road.Graph.Edges)
            {
                Edge<Vertex> removeEdge = null;
                if (_road.Graph.TryGetEdge(pathEdge.Source, pathEdge.Target, out removeEdge)) _road.Graph.RemoveEdge(removeEdge);
                else if (_road.Graph.TryGetEdge(pathEdge.Target, pathEdge.Source, out removeEdge)) _road.Graph.RemoveEdge(removeEdge);
            }

            return nvc;
        }

        //public List<Vertex> FindPathsByRange(RoadGraph graph, double range)
        //{
        //    FindPathCompleted += AddPathVertex;
        //    _root.FindPathsByRange(graph.Clone(), range, FindPathCompleted);

        //    return _pathVertexs;
        //}

        //private void AddPathVertex(PathTreeNode node)
        //{
        //    _pathVertexs.Add(node.Vertex);
        //}
    }
}

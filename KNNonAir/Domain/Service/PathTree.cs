using KNNonAir.Domain.Entity;
using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir.Domain.Service
{
    class PathTree
    {
        private PathNode _root;
        private Dictionary<Vertex, PathNode> _leaves;
        private List<Vertex> _pathVertexs;
        public bool IsRepeat;

        private event PathNodeHandler FindPathCompleted;
        public event BorderPointHandler FindBorderPointCompleted;

        public PathTree(Vertex root)
        {
            _root = new PathNode() { Vertex = root };
            _leaves = new Dictionary<Vertex, PathNode>();
            _pathVertexs = new List<Vertex>();
            IsRepeat = true;
        }

        public void GenerateNVC(AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            FindPaths(graph);
            FindBorderPoint();
        }

        private void FindPaths(AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            IsRepeat = false;
            FindPathCompleted += AddLeaf;
            _leaves.Clear();
            if (_root.Children.Count > 0) _root.Children.Clear();
            _root.FindPath(graph, _root.Vertex, FindPathCompleted);
        }

        private void AddLeaf(PathNode node)
        {
            if (_leaves.ContainsKey(node.Vertex))
            {
                IsRepeat = true;
                if (node.Distance < _leaves[node.Vertex].Distance) _leaves[node.Vertex] = node;
            }
            else _leaves.Add(node.Vertex, node);
        }

        private void FindBorderPoint()
        {
            foreach (KeyValuePair<Vertex, PathNode> leaf in _leaves)
            {
                leaf.Value.InsertBorderPoint(leaf.Value.Distance / 2, _root.Vertex, leaf.Key, FindBorderPointCompleted);
            }
        }

        public VoronoiCell FindNVC()
        {
            VoronoiCell nvc = new VoronoiCell() { PoI = _root.Vertex };
            _root.LoadNVC(nvc);         
            return nvc;
        }

        public List<Vertex> FindPathsByRange(AdjacencyGraph<Vertex, Edge<Vertex>> graph, double range)
        {
            FindPathCompleted += AddPathVertex;
            _root.FindPathsByRange(graph.Clone(), range, FindPathCompleted);

            return _pathVertexs;
        }

        private void AddPathVertex(PathNode node)
        {
            _pathVertexs.Add(node.Vertex);
        }
    }
}

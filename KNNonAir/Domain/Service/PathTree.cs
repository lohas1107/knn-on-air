using KNNonAir.Domain.Entity;
using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir.Domain.Service
{
    class PathTree
    {
        private PathTreeNode _root;
        private Dictionary<Vertex, PathTreeNode> _leaves;
        private List<Vertex> _pathVertexs;

        public bool IsRepeat { get; set; }

        private event PathNodeHandler FindPathCompleted;
        public event BorderPointHandler FindBorderPointCompleted;

        public PathTree(Vertex root)
        {
            _root = new PathTreeNode() { Vertex = root };
            _leaves = new Dictionary<Vertex, PathTreeNode>();
            _pathVertexs = new List<Vertex>();
            IsRepeat = true;
            FindPathCompleted += AddLeaf;
        }

        public void GenerateNVC(RoadGraph graph)
        {
            FindPaths(graph);
            FindBorderPoint();
        }

        private void FindPaths(RoadGraph graph)
        {
            IsRepeat = false;
            _leaves.Clear();

            if (_root.Children.Count > 0) _root.Children.Clear();
            _root.FindPath(graph, _root.Vertex, FindPathCompleted);
        }

        private void AddLeaf(PathTreeNode node)
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
            foreach (KeyValuePair<Vertex, PathTreeNode> leaf in _leaves)
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

        public List<Vertex> FindPathsByRange(RoadGraph graph, double range)
        {
            FindPathCompleted += AddPathVertex;
            _root.FindPathsByRange(graph.Clone(), range, FindPathCompleted);

            return _pathVertexs;
        }

        private void AddPathVertex(PathTreeNode node)
        {
            _pathVertexs.Add(node.Vertex);
        }
    }
}

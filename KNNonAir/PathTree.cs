﻿using QuickGraph;
using System.Collections.Generic;

namespace KNNonAir
{
    class PathTree
    {
        private PathNode _root;
        private List<PathNode> _leaves;

        private event PathNodeHandler FindPathCompleted;

        public PathTree(Vertex root)
        {
            _root = new PathNode() { Vertex = root };
            _leaves = new List<PathNode>();
        }

        public VoronoiCell GenerateNVC(AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            FindPaths(graph);
            FindBorderPoint();
            return FindNVC();
        }

        private void FindPaths(AdjacencyGraph<Vertex, Edge<Vertex>> graph)
        {
            FindPathCompleted += AddLeaf;
            _root.FindPath(graph, FindPathCompleted);
        }

        private void AddLeaf(PathNode node)
        {
            _leaves.Add(node);
        }

        private void FindBorderPoint()
        {
            foreach (PathNode leaf in _leaves)
            {
                leaf.InsertBorderPoint(leaf.Distance/2);
            }
        }

        private VoronoiCell FindNVC()
        {
            VoronoiCell nvc = new VoronoiCell() { PoI = _root.Vertex };
            _root.LoadNVC(nvc);         
            return nvc;
        }
    }
}

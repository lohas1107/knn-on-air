using System.Collections.Generic;
using KNNonAir.Domain.Entity;

namespace KNNonAir.Domain.Service
{
    class VQTree
    {
        private VQTreeNode _root;

        public VQTree(List<Vertex> borderPoints, MBR mbr)
        {
            _root = new VQTreeNode();
            _root.Partition(borderPoints, mbr);
        }
    }
}

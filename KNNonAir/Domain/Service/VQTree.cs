using System.Collections.Generic;
using KNNonAir.Domain.Entity;

namespace KNNonAir.Domain.Service
{
    class VQTree
    {
        public event MBRHandler PartitionMBRCompleted;

        private VQTreeNode _root;

        public List<MBR> MBRs { get; set; }

        public VQTree(List<Vertex> borderPoints, MBR mbr)
        {
            _root = new VQTreeNode();
            MBRs = new List<MBR>();

            PartitionMBRCompleted += AddMBR;
            _root.Partition(borderPoints, mbr, PartitionMBRCompleted);
        }

        private void AddMBR(MBR mbr)
        {
            MBRs.Add(mbr);
        }
    }
}

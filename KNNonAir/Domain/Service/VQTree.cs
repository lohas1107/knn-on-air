using System.Collections.Generic;
using KNNonAir.Domain.Entity;
using System;
using System.Runtime.Serialization;

namespace KNNonAir.Domain.Service
{
    [Serializable]
    class VQTree : ISerializable
    {
        public event MBRHandler PartitionMBRCompleted;
        [NonSerialized]
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

        public int searchRegion(Vertex queryPoint)
        {
            return _root.searchRegion(queryPoint);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MBRs", MBRs);
        }
    }
}

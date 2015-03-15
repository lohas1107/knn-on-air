using System.Collections.Generic;
using KNNonAir.Domain.Entity;

namespace KNNonAir.Domain.Service
{
    class VQTreeNode
    {
        public VQTreeNode NE { get; set; }
        public VQTreeNode NW { get; set; }
        public VQTreeNode SE { get; set; }
        public VQTreeNode SW { get; set; }

        public void Partition(List<Vertex> borderPoints, MBR mbr, MBRHandler partitionMBRCompleted)
        {
            partitionMBRCompleted(mbr);

            double halfWidth = mbr.Width / 2;
            double halfHeight = mbr.Height / 2;
            //if (halfWidth < 0.0001 || halfHeight < 0.0001) return;

            foreach (Vertex borderPoint in borderPoints)
            {
                if (mbr.Contains(borderPoint))
                {
                    MBR neMBR = new MBR(mbr.X + halfWidth, mbr.Y, halfWidth, halfHeight);
                    NE = new VQTreeNode();
                    NE.Partition(neMBR.GetVerticeInMBR(borderPoints), neMBR, partitionMBRCompleted);

                    MBR nwMBR = new MBR(mbr.X, mbr.Y, halfWidth, halfHeight);
                    NW = new VQTreeNode();
                    NW.Partition(nwMBR.GetVerticeInMBR(borderPoints), nwMBR, partitionMBRCompleted);

                    MBR seMBR = new MBR(mbr.X + halfWidth, mbr.Y - halfHeight, halfWidth, halfHeight);
                    SE = new VQTreeNode();
                    SE.Partition(seMBR.GetVerticeInMBR(borderPoints), seMBR, partitionMBRCompleted);

                    MBR swMBR = new MBR(mbr.X, mbr.Y - halfHeight, halfWidth, halfHeight);
                    SW = new VQTreeNode();
                    SW.Partition(swMBR.GetVerticeInMBR(borderPoints), swMBR, partitionMBRCompleted);

                    break;
                }
            }
        }
    }
}

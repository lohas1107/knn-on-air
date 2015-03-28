using System.Collections.Generic;
using KNNonAir.Domain.Entity;

namespace KNNonAir.Domain.Service
{
    class VQTreeNode
    {
        public MBR MBR { get; set; }
        public VQTreeNode NE { get; set; }
        public VQTreeNode NW { get; set; }
        public VQTreeNode SE { get; set; }
        public VQTreeNode SW { get; set; }

        public void Partition(List<Vertex> borderPoints, MBR mbr, MBRHandler partitionMBRCompleted)
        {
            MBR = mbr;
            partitionMBRCompleted(mbr);

            double halfWidth = mbr.Width / 2;
            double halfHeight = mbr.Height / 2;
            //if (halfWidth < 0.0001 || halfHeight < 0.0001) return;

            foreach (Vertex borderPoint in borderPoints)
            {
                if (mbr.ContainsIn(borderPoint))
                {
                    MBR neMBR = new MBR(mbr.X + halfWidth, mbr.Y, halfWidth, halfHeight);
                    neMBR.AddVertices(mbr.Vertices);
                    NE = new VQTreeNode();
                    NE.Partition(neMBR.GetVerticeInMBR(borderPoints), neMBR, partitionMBRCompleted);

                    MBR nwMBR = new MBR(mbr.X, mbr.Y, halfWidth, halfHeight);
                    nwMBR.AddVertices(mbr.Vertices);
                    NW = new VQTreeNode();
                    NW.Partition(nwMBR.GetVerticeInMBR(borderPoints), nwMBR, partitionMBRCompleted);

                    MBR seMBR = new MBR(mbr.X + halfWidth, mbr.Y - halfHeight, halfWidth, halfHeight);
                    seMBR.AddVertices(mbr.Vertices);
                    SE = new VQTreeNode();
                    SE.Partition(seMBR.GetVerticeInMBR(borderPoints), seMBR, partitionMBRCompleted);

                    MBR swMBR = new MBR(mbr.X, mbr.Y - halfHeight, halfWidth, halfHeight);
                    swMBR.AddVertices(mbr.Vertices);
                    SW = new VQTreeNode();
                    SW.Partition(swMBR.GetVerticeInMBR(borderPoints), swMBR, partitionMBRCompleted);

                    break;
                }
            }
        }

        public int searchRegion(Vertex queryPoint)
        {
            if (NE == null)
            {
                foreach (Vertex vertex in MBR.Vertices)
                {
                    if (vertex.RegionId != -1) return vertex.RegionId;
                }
            }
            else
            {
                if (NE.MBR.Contains(queryPoint)) return NE.searchRegion(queryPoint);
                else if (NW.MBR.Contains(queryPoint)) return NW.searchRegion(queryPoint);
                else if (SE.MBR.Contains(queryPoint)) return SE.searchRegion(queryPoint);
                else if (SW.MBR.Contains(queryPoint)) return SW.searchRegion(queryPoint);
            }

            return -1;
        }
    }
}

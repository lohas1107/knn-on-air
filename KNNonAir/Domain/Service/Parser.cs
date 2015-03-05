using System.Collections.Generic;
using KNNonAir.Access;
using QuickGraph;

namespace KNNonAir.Domain.Service
{
    class Parser
    {
        public static List<NVCInfo> ParseNVD(Dictionary<Vertex, VoronoiCell> nvd)
        {
            List<NVCInfo> nvcList = new List<NVCInfo>();

            foreach (KeyValuePair<Vertex, VoronoiCell> nvc in nvd)
            {
                NVCInfo nvcInfo = new NVCInfo();
                nvcInfo.PoI = new VertexInfo(nvc.Key.Coordinate.Latitude, nvc.Key.Coordinate.Longitude);

                foreach (Edge<Vertex> edge in nvc.Value.Graph.Edges)
                {
                    VertexInfo source = new VertexInfo(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude);
                    VertexInfo target = new VertexInfo(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude);
                    nvcInfo.Graph.Add(new EdgeInfo(source, target));
                }

                foreach (Vertex borderPoint in nvc.Value.BorderPoints)
                {
                    nvcInfo.BPs.Add(new VertexInfo(borderPoint.Coordinate.Latitude, borderPoint.Coordinate.Longitude));
                }

                nvcList.Add(nvcInfo);
            }

            return nvcList;
        }
    }
}

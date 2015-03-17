using KNNonAir.Access;
using KNNonAir.Domain.Entity;
using QuickGraph;
using System.Collections.Generic;

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

                foreach (BorderPoint borderPoint in nvc.Value.BorderPoints)
                {
                    BorderInfo bp = new BorderInfo(borderPoint.Coordinate.Latitude, borderPoint.Coordinate.Longitude);
                    foreach (Vertex poi in borderPoint.PoIs) bp.PoIs.Add(new VertexInfo(poi.Coordinate.Latitude, poi.Coordinate.Longitude));
                    nvcInfo.BPs.Add(bp);
                }

                nvcList.Add(nvcInfo);
            }

            return nvcList;
        }

        public static RoadGraph<Vertex, Edge<Vertex>> ParseNVCInfoToGraph(List<NVCInfo> nvcList)
        {
            RoadGraph<Vertex, Edge<Vertex>> graph = new RoadGraph<Vertex, Edge<Vertex>>(false);

            foreach (NVCInfo nvc in nvcList)
            {
                graph.AddVertex(new InterestPoint(nvc.PoI.Latitude, nvc.PoI.Longitude));

                foreach (BorderInfo borderInfo in nvc.BPs)
                {
                    Vertex borderPoint = new BorderPoint(borderInfo.Vertex.Latitude, borderInfo.Vertex.Longitude);
                    graph.AddVertex(borderPoint);
                }

                foreach (EdgeInfo edge in nvc.Graph)
                {
                    Vertex source = new Vertex(edge.Source.Latitude, edge.Source.Longitude);
                    Vertex target = new Vertex(edge.Target.Latitude, edge.Target.Longitude);
                    if (!graph.ContainsVertex(source)) graph.AddVertex(source);
                    if (!graph.ContainsVertex(target)) graph.AddVertex(target);
                    graph.AddEdge(new Edge<Vertex>(source, target));
                }
            }

            return graph;
        }

        public static Dictionary<Vertex, VoronoiCell> ParseNVCInfoToNVD(List<NVCInfo> nvcList)
        {
            Dictionary<Vertex, VoronoiCell> nvd = new Dictionary<Vertex, VoronoiCell>();

            foreach (NVCInfo nvc in nvcList)
            {
                VoronoiCell vc = new VoronoiCell();
                vc.PoI = new Vertex(nvc.PoI.Latitude, nvc.PoI.Longitude);

                foreach (EdgeInfo edge in nvc.Graph)
                {
                    Vertex source = new Vertex(edge.Source.Latitude, edge.Source.Longitude);
                    Vertex target = new Vertex(edge.Target.Latitude, edge.Target.Longitude);
                    if (!vc.Graph.ContainsVertex(source)) vc.Graph.AddVertex(source);
                    if (!vc.Graph.ContainsVertex(target)) vc.Graph.AddVertex(target);
                    vc.Graph.AddEdge(new Edge<Vertex>(source, target));
                }

                foreach (BorderInfo borderPoint in nvc.BPs)
                {
                    BorderPoint bp = new BorderPoint(borderPoint.Vertex.Latitude, borderPoint.Vertex.Longitude);
                    foreach (VertexInfo poi in borderPoint.PoIs) bp.PoIs.Add(new Vertex(poi.Latitude, poi.Longitude));
                    vc.BorderPoints.Add(bp);
                }

                nvd.Add(vc.PoI, vc);
            }

            return nvd;
        }

        public static List<Vertex> ParsePoIInfo(List<NVCInfo> nvcList)
        {
            List<Vertex> pois = new List<Vertex>();

            foreach (NVCInfo nvc in nvcList)
            {
                pois.Add(new Vertex(nvc.PoI.Latitude, nvc.PoI.Longitude));
            }

            return pois;
        }
    }
}

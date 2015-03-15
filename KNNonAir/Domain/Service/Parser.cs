﻿using KNNonAir.Access;
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

                foreach (Vertex borderPoint in nvc.Value.BorderPoints)
                {
                    nvcInfo.BPs.Add(new VertexInfo(borderPoint.Coordinate.Latitude, borderPoint.Coordinate.Longitude));
                }

                nvcList.Add(nvcInfo);
            }

            return nvcList;
        }

        public static AdjacencyGraph<Vertex, Edge<Vertex>> ParseNVCInfoToGraph(List<NVCInfo> nvcList)
        {
            AdjacencyGraph<Vertex, Edge<Vertex>> graph = new AdjacencyGraph<Vertex, Edge<Vertex>>(false);

            foreach (NVCInfo nvc in nvcList)
            {
                foreach (EdgeInfo edge in nvc.Graph)
                {
                    Vertex source = new Vertex(edge.Source.Latitude, edge.Source.Longitude);
                    Vertex target = new Vertex(edge.Target.Latitude, edge.Target.Longitude);
                    graph.AddVertex(source);
                    graph.AddVertex(target);
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

                foreach (VertexInfo borderPoint in nvc.BPs)
                {
                    vc.BorderPoints.Add(new BorderPoint(borderPoint.Latitude, borderPoint.Longitude));
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
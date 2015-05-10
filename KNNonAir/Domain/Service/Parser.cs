using Geo;
using Geo.Geometries;
using Geo.IO.GeoJson;
using KNNonAir.Access;
using KNNonAir.Domain.Entity;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KNNonAir.Domain.Service
{
    class Parser
    {
        public static List<Edge<Vertex>> ParseRoadData(string rawData)
        {
            if (rawData == null) return null;
            List<Edge<Vertex>> edgeList = new List<Edge<Vertex>>();

            FeatureCollection geoObject = GeoJson.DeSerialize(rawData) as FeatureCollection;
            foreach (Feature feature in geoObject.Features)
            {
                if (feature.Geometry is MultiLineString)
                {
                    MultiLineString multiLingString = feature.Geometry as MultiLineString;
                    foreach (LineString lineString in multiLingString.Geometries)
                    {
                        LoadLineString(edgeList, lineString);
                    }
                }
                else
                {
                    LineString lineString = feature.Geometry as LineString;
                    LoadLineString(edgeList, lineString);
                }
            }

            return edgeList;
        }

        private static void LoadLineString(List<Edge<Vertex>> edgeList, LineString lineString)
        {
            Vertex source = null;
            Vertex target = null;

            foreach (Coordinate coordinate in lineString.Coordinates)
            {
                if (source == null)
                {
                    source = new Vertex(coordinate);
                }
                else
                {
                    target = new Vertex(coordinate);
                    edgeList.Add(new Edge<Vertex>(source, target));
                    source = target;
                }
            }
        }

        public static List<Vertex> ParsePoIData(string rawData)
        {
            if (rawData == null) return null;
            List<Vertex> poiList = new List<Vertex>();

            FeatureCollection geoObject = GeoJson.DeSerialize(rawData) as FeatureCollection;
            foreach (Feature feature in geoObject.Features)
            {
                Point point = feature.Geometry as Point;
                poiList.Add(new InterestPoint(point.Coordinate.Latitude, point.Coordinate.Longitude));
            }

            return poiList;
        }

        public static List<NVCInfo> ParseNVD(Dictionary<Vertex, VoronoiCell> nvd)
        {
            List<NVCInfo> nvcList = new List<NVCInfo>();

            foreach (KeyValuePair<Vertex, VoronoiCell> nvc in nvd)
            {
                NVCInfo nvcInfo = new NVCInfo();
                nvcInfo.PoI = new VertexInfo(nvc.Key.Coordinate.Latitude, nvc.Key.Coordinate.Longitude);

                foreach (Edge<Vertex> edge in nvc.Value.Road.Graph.Edges)
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

        public static RoadGraph ParseNVCInfoToGraph(List<NVCInfo> nvcList)
        {
            RoadGraph road = new RoadGraph(false);

            foreach (NVCInfo nvc in nvcList)
            {
                road.Graph.AddVertex(new InterestPoint(nvc.PoI.Latitude, nvc.PoI.Longitude));

                foreach (BorderInfo borderInfo in nvc.BPs)
                {
                    Vertex borderPoint = new BorderPoint(borderInfo.Vertex.Latitude, borderInfo.Vertex.Longitude);
                    road.Graph.AddVertex(borderPoint);
                }

                foreach (EdgeInfo edge in nvc.Graph)
                {
                    Vertex source = new Vertex(edge.Source.Latitude, edge.Source.Longitude);
                    Vertex target = new Vertex(edge.Target.Latitude, edge.Target.Longitude);
                    if (!road.Graph.ContainsVertex(source)) road.Graph.AddVertex(source);
                    if (!road.Graph.ContainsVertex(target)) road.Graph.AddVertex(target);
                    road.Graph.AddEdge(new Edge<Vertex>(source, target));
                }
            }

            return road;
        }

        public static Dictionary<Vertex, VoronoiCell> ParseNVCInfoToNVD(List<NVCInfo> nvcList)
        {
            Dictionary<Vertex, VoronoiCell> nvd = new Dictionary<Vertex, VoronoiCell>();

            foreach (NVCInfo nvc in nvcList)
            {
                VoronoiCell vc = new VoronoiCell();
                vc.PoI = new InterestPoint(nvc.PoI.Latitude, nvc.PoI.Longitude);
                vc.Road.Graph.AddVertex(vc.PoI);

                foreach (BorderInfo borderPoint in nvc.BPs)
                {
                    BorderPoint bp = new BorderPoint(borderPoint.Vertex.Latitude, borderPoint.Vertex.Longitude);
                    foreach (VertexInfo poi in borderPoint.PoIs) bp.PoIs.Add(new InterestPoint(poi.Latitude, poi.Longitude));
                    vc.BorderPoints.Add(bp);
                    vc.Road.Graph.AddVertex(bp);
                }

                foreach (EdgeInfo edge in nvc.Graph)
                {
                    Vertex source = new Vertex(edge.Source.Latitude, edge.Source.Longitude);
                    Vertex target = new Vertex(edge.Target.Latitude, edge.Target.Longitude);
                    if (!vc.Road.Graph.ContainsVertex(source)) vc.Road.Graph.AddVertex(source);
                    if (!vc.Road.Graph.ContainsVertex(target)) vc.Road.Graph.AddVertex(target);
                    vc.Road.Graph.AddEdge(new Edge<Vertex>(source, target));
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
                pois.Add(new InterestPoint(nvc.PoI.Latitude, nvc.PoI.Longitude));
            }

            return pois;
        }

        public static long ObjectToByteArray(Object obj)
        {
            if (obj == null) return 0;

            BinaryFormatter formatter = new BinaryFormatter();
            using(MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.Length;
            }
        }

        public static EBTableInfo ParseCountingTable(List<Vertex> pois, AlgorithmEB eb)
        {
            List<VertexInfo> poiList = new List<VertexInfo>();

            foreach (Vertex poi in pois)
            {
                poiList.Add(new VertexInfo(poi.Coordinate.Latitude, poi.Coordinate.Longitude));
            }

            return new EBTableInfo(poiList, eb.MinTable, eb.MaxCountTable);
        }
    }
}

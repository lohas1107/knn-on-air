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
                foreach (EdgeInfo edge in nvc.Graph)
                {
                    Vertex source;
                    Vertex target;

                    if (edge.Source == nvc.PoI) source = new InterestPoint(edge.Source.Latitude, edge.Source.Longitude);
                    else if (nvc.BPs.Contains(new BorderInfo(edge.Source.Latitude, edge.Source.Longitude))) source = new BorderPoint(edge.Source.Latitude, edge.Source.Longitude);
                    else source = new Vertex(edge.Source.Latitude, edge.Source.Longitude);

                    if (edge.Target == nvc.PoI) target = new InterestPoint(edge.Target.Latitude, edge.Target.Longitude);
                    else if (nvc.BPs.Contains(new BorderInfo(edge.Target.Latitude, edge.Target.Longitude))) target = new BorderPoint(edge.Target.Latitude, edge.Target.Longitude);
                    else target = new Vertex(edge.Target.Latitude, edge.Target.Longitude);

                    road.Graph.AddVerticesAndEdge(new Edge<Vertex>(source, target));
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

                foreach (BorderInfo borderPoint in nvc.BPs)
                {
                    BorderPoint bp = new BorderPoint(borderPoint.Vertex.Latitude, borderPoint.Vertex.Longitude);
                    foreach (VertexInfo poi in borderPoint.PoIs) bp.PoIs.Add(new InterestPoint(poi.Latitude, poi.Longitude));
                    vc.BorderPoints.Add(bp);
                }

                foreach (EdgeInfo edge in nvc.Graph)
                {
                    Vertex source;
                    Vertex target;

                    if (edge.Source == nvc.PoI) source = new InterestPoint(edge.Source.Latitude, edge.Source.Longitude);
                    else if (nvc.BPs.Contains(new BorderInfo(edge.Source.Latitude, edge.Source.Longitude))) source = new BorderPoint(edge.Source.Latitude, edge.Source.Longitude);
                    else source = new Vertex(edge.Source.Latitude, edge.Source.Longitude);

                    if (edge.Target == nvc.PoI) target = new InterestPoint(edge.Target.Latitude, edge.Target.Longitude);
                    else if (nvc.BPs.Contains(new BorderInfo(edge.Target.Latitude, edge.Target.Longitude))) target = new BorderPoint(edge.Target.Latitude, edge.Target.Longitude);
                    else target = new Vertex(edge.Target.Latitude, edge.Target.Longitude);

                    vc.Road.Graph.AddVerticesAndEdge(new Edge<Vertex>(source, target));
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

        public static NPITableInfo ParseNPITable(AlgorithmNPI NPI)
        {
            return new NPITableInfo(NPI.CountDiameterTable, NPI.MinMaxTable);
        }

        public static RoadPoIInfo ParseRoadPoI(RoadGraph road, List<Vertex> pois)
        {
            List<EdgeInfo> edgeInfo = new List<EdgeInfo>();
            List<VertexInfo> poiInfo = new List<VertexInfo>();

            foreach (Edge<Vertex> edge in road.Graph.Edges)
            {
                VertexInfo source = new VertexInfo(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude);
                VertexInfo target = new VertexInfo(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude);
                edgeInfo.Add(new EdgeInfo(source, target));
            }

            foreach (Vertex poi in pois) poiInfo.Add(new VertexInfo(poi.Coordinate.Latitude, poi.Coordinate.Longitude));

            return new RoadPoIInfo(edgeInfo, poiInfo);
        }

        public static RoadGraph ParseRoadInfo(RoadPoIInfo roadPoI)
        {
            RoadGraph road = new RoadGraph(false);

            foreach (EdgeInfo edge in roadPoI.Road)
            {
                Vertex source = new Vertex(edge.Source.Latitude, edge.Source.Longitude);
                Vertex target = new Vertex(edge.Target.Latitude, edge.Target.Longitude);

                if (roadPoI.PoIs.Contains(edge.Source)) source = new InterestPoint(edge.Source.Latitude, edge.Source.Longitude);
                if (roadPoI.PoIs.Contains(edge.Target)) target = new InterestPoint(edge.Target.Latitude, edge.Target.Longitude);

                road.Graph.AddVerticesAndEdge(new Edge<Vertex>(source, target));
            }

            return road;
        }

        public static List<Vertex> ParsePoIInfo(List<VertexInfo> list)
        {
            List<Vertex> pois = new List<Vertex>();

            foreach (VertexInfo poi in list)
            {
                pois.Add(new InterestPoint(poi.Latitude, poi.Longitude));
            }

            return pois;
        }
    }
}

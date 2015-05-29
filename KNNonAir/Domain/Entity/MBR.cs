using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using QuickGraph;

namespace KNNonAir.Domain.Entity
{
    [Serializable]
    public class MBR : ISerializable
    {
        public int Id { get; set; }
        public List<Vertex> Vertices { get; set; }
        public double X { get; set; } // Longitude
        public double Y { get; set; } // Latitude
        public double Width { get; set; }
        public double Height { get; set; }

        public MBR(double x, double y, double width, double height)
        {
            Vertices = new List<Vertex>();
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public MBR(IEnumerable<Vertex> vertices)
        {
            Vertices = new List<Vertex>();

            vertices = vertices.OrderBy(o => o.Coordinate.Longitude).ToList();
            X = vertices.First().Coordinate.Longitude;
            Width = vertices.Last().Coordinate.Longitude - X;

            vertices = vertices.OrderBy(o => o.Coordinate.Latitude).ToList();
            Y = vertices.Last().Coordinate.Latitude;
            Height = Y - vertices.First().Coordinate.Latitude;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
        }

        public bool ContainsIn(Vertex vertex)
        {
            if (vertex.Coordinate.Longitude > X && vertex.Coordinate.Longitude < X + Width &&
                vertex.Coordinate.Latitude > Y - Height && vertex.Coordinate.Latitude < Y) return true;
            else return false;
        }

        public bool Contains(Vertex vertex)
        {
            if (vertex.Coordinate.Longitude >= X && vertex.Coordinate.Longitude <= X + Width &&
                vertex.Coordinate.Latitude >= Y - Height && vertex.Coordinate.Latitude <= Y) return true;
            else return false;
        }

        public List<Vertex> GetVerticeInMBR(List<Vertex> borderPoints)
        {
            List<Vertex> mbrPoints = new List<Vertex>();

            foreach (Vertex borderPoint in borderPoints)
            {
                if (ContainsIn(borderPoint)) mbrPoints.Add(borderPoint);
            }

            return mbrPoints;
        }

        public void AddVertices(IEnumerable<Vertex> vertices)
        {
            foreach (Vertex vertex in vertices)
            {
                if (ContainsIn(vertex)) Vertices.Add(vertex);
            }
        }

        public Region ToRegion(RoadGraph road, List<Vertex> pois)
        {
            Region region = new Region();
            region.Id = Id;

            foreach (Edge<Vertex> edge in road.Graph.Edges)
            {
                if (!Vertices.Contains(edge.Source) && !Vertices.Contains(edge.Target)) continue;
                else if (Vertices.Contains(edge.Source) && Vertices.Contains(edge.Target))
                {
                    if (pois.Contains(edge.Source) && !region.PoIs.Contains(edge.Source)) region.PoIs.Add(edge.Source);
                    if (pois.Contains(edge.Target) && !region.PoIs.Contains(edge.Target)) region.PoIs.Add(edge.Target);
                    region.Road.Graph.AddVerticesAndEdge(edge);
                }
                else if (Vertices.Contains(edge.Source) || Vertices.Contains(edge.Target))
                {
                    Vertex borderSource = new BorderPoint(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude);
                    Vertex borderTarget = new BorderPoint(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude);
                    region.BorderPoints.Add(borderSource);
                    region.BorderPoints.Add(borderTarget);
                    Edge<Vertex> borderEdge = new Edge<Vertex>(borderSource, borderTarget);
                    region.Road.Graph.AddVerticesAndEdge(borderEdge);
                }
            }

            return region;
        }
    }
}

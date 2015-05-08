using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace KNNonAir.Domain.Entity
{
    [Serializable]
    public class MBR : ISerializable
    {
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
        }
    }
}

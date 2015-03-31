using System.Collections.Generic;

namespace KNNonAir.Domain.Entity
{
    class MBR
    {
        public int RegionId { get; set; }
        public List<Vertex> Vertices { get; set; }
        public double X { get; set; } // Longitude
        public double Y { get; set; } // Latitude
        public double Width { get; set; }
        public double Height { get; set; }

        public MBR(double x, double y, double width, double height)
        {
            RegionId = -1;
            Vertices = new List<Vertex>();
            X = x;
            Y = y;
            Width = width;
            Height = height;
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
    }
}

using System.Collections.Generic;

namespace KNNonAir.Domain.Entity
{
    class MBR
    {
        public double X { get; set; } // Longitude
        public double Y { get; set; } // Latitude
        public double Width { get; set; }
        public double Height { get; set; }

        public MBR(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(Vertex vertex)
        {
            if (vertex.Coordinate.Longitude > X && vertex.Coordinate.Longitude < X + Width &&
                vertex.Coordinate.Latitude > Y - Height && vertex.Coordinate.Latitude < Y) return true;
            else return false;
        }

        public List<Vertex> GetVerticeInMBR(List<Vertex> borderPoints)
        {
            List<Vertex> mbrPoints = new List<Vertex>();

            foreach (Vertex borderPoint in borderPoints)
            {
                if (Contains(borderPoint)) mbrPoints.Add(borderPoint);
            }

            return mbrPoints;
        }
    }
}

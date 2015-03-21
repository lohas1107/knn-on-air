using System;
using Geo;
using Geo.Geometries;

namespace KNNonAir.Domain.Entity
{
    class Vertex : IComparable<Vertex>
    {
        private Point _point;
        public Coordinate Coordinate { get; set; }

        public Vertex() { } // InterestPoint() 繼承 Vertex()

        public Vertex(double latitude, double longitude)
        {
            _point = new Point(latitude, longitude);
            Coordinate = _point.Coordinate;
        }

        public override int GetHashCode()
        {
            return Coordinate.GetHashCode();
        } 

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            
            if (obj is Vertex || obj is InterestPoint || obj is BorderPoint)
            {
                var vertex = (Vertex)obj;
                return (vertex.Coordinate == Coordinate);
            }
            return false;
        }

        public int CompareTo(Vertex other)
        {
            return 0;
        }
    }
}

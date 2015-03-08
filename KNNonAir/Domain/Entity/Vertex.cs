using Geo;
using Geo.Geometries;

namespace KNNonAir.Domain.Entity
{
    class Vertex
    {
        private Point _point;
        public Coordinate Coordinate { get; set; }

        public Vertex() { } // InterestPoint() 繼承 Vertex()

        public Vertex(double latitude, double longitude)
        {
            _point = new Point(latitude, longitude);
            Coordinate = _point.Coordinate;
        }
    }
}

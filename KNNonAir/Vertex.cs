using Geo;
using Geo.Geometries;

namespace KNNonAir
{
    class Vertex
    {
        private Point point;
        public Coordinate Coordinate { get; set; }

        public Vertex() { }

        public Vertex(double latitude, double longitude)
        {
            point = new Point(latitude, longitude);
            Coordinate = point.Coordinate;
        }

    }
}

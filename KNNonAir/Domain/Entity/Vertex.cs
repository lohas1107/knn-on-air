using System;
using Geo;
using Geo.Geometries;
using System.Runtime.Serialization;
using KNNonAir.Access;

namespace KNNonAir.Domain.Entity
{
    [Serializable]
    class Vertex : IComparable<Vertex>, ISerializable
    {
        [NonSerialized]
        private Point _point;
        public Coordinate Coordinate { get; set; }
        public int RegionId { get; set; }

        public Vertex() { } // InterestPoint() 繼承 Vertex()

        public Vertex(double latitude, double longitude)
        {
            _point = new Point(latitude, longitude);
            Coordinate = _point.Coordinate;
            RegionId = -1;
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Point", new VertexInfo(Coordinate.Latitude, Coordinate.Longitude));
            info.AddValue("RegionId", RegionId);
        }
    }
}

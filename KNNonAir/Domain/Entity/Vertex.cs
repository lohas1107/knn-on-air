using System;
using System.Runtime.Serialization;
using Geo;
using Geo.Geometries;
using KNNonAir.Access;

namespace KNNonAir.Domain.Entity
{
    [Serializable]
    public class Vertex : IEquatable<Vertex>, IComparable<Vertex>, ISerializable
    {
        [NonSerialized]
        private Point _point;
        public Coordinate Coordinate { get; set; }
        public int RegionId { get; set; }

        public Vertex() { } // InterestPoint() 繼承 Vertex()

        public Vertex(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }

        public Vertex(double latitude, double longitude)
        {
            _point = new Point(latitude, longitude);
            Coordinate = _point.Coordinate;
            RegionId = -1;
        }

        public override int GetHashCode()
        {
            unchecked { return (Coordinate.Latitude).GetHashCode() + (Coordinate.Longitude).GetHashCode(); }
        }

        public static bool operator ==(Vertex v1, Vertex v2)
        {
            if (object.ReferenceEquals(v1, v2))
            {
                return true;
            }
            if (object.ReferenceEquals(v1, null) || object.ReferenceEquals(v2, null))
            {
                return false;
            }

            return v1.Coordinate.Latitude == v2.Coordinate.Latitude && v1.Coordinate.Longitude == v2.Coordinate.Longitude;
        }

        public static bool operator !=(Vertex v1, Vertex v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object other)
        {
            return this == (other as Vertex);
        }

        public bool Equals(Vertex other)
        {
            return this == other;
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

using System;
using System.Runtime.Serialization;

namespace KNNonAir.Access
{
    [Serializable]
    public class VertexInfo : IEquatable<VertexInfo>, ISerializable
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public VertexInfo() { }

        public VertexInfo(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }

        public VertexInfo(SerializationInfo info, StreamingContext context)
        {
            Latitude = (double)info.GetValue("Latitude", typeof(double));
            Longitude = (double)info.GetValue("Longitude", typeof(double));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Latitude", Latitude);
            info.AddValue("Longitude", Longitude);
        }

        public bool Equals(VertexInfo other)
        {
            return this.Latitude == other.Latitude && this.Longitude == other.Longitude;
        }
    }
}

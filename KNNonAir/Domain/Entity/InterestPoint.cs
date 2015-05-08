using System;
using System.Runtime.Serialization;

namespace KNNonAir.Domain.Entity
{
    [Serializable]
    class InterestPoint : Vertex, ISerializable
    {
        public InterestPoint() : base() { }

        public InterestPoint(double latitude, double longitude) : base(latitude, longitude) { }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KNNonAir.Domain.Entity
{
    [Serializable]
    class BorderPoint : Vertex, ISerializable
    {
        public List<Vertex> PoIs { get; set; }

        public BorderPoint(double latitude, double longitude) : base(latitude, longitude)
        {
            PoIs = new List<Vertex>();
        }
    }
}

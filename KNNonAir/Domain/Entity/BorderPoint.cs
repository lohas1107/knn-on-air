using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNNonAir.Domain.Entity
{
    class BorderPoint : Vertex
    {
        public List<Vertex> PoIs { get; set; }

        public BorderPoint(double latitude, double longitude) : base(latitude, longitude)
        {
            PoIs = new List<Vertex>();
        }
    }
}

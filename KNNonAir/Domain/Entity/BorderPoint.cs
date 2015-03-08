using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNNonAir.Domain.Entity
{
    class BorderPoint : Vertex
    {
        public BorderPoint(double latitude, double longitude) : base(latitude, longitude) { }
    }
}

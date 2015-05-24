using KNNonAir.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KNNonAir.Access
{
    [Serializable]
    public class RoadPoIInfo : ISerializable
    {
        public List<EdgeInfo> Road { get; set; }
        public List<VertexInfo> PoIs { get; set; }

        public RoadPoIInfo() { }

        public RoadPoIInfo(List<EdgeInfo> road, List<VertexInfo> pois)
        {
            Road = road;
            PoIs = pois;
        }

        public RoadPoIInfo(SerializationInfo info, StreamingContext context)
        {
            Road = (List<EdgeInfo>)info.GetValue("Road", typeof(List<EdgeInfo>));
            PoIs = (List<VertexInfo>)info.GetValue("PoIs", typeof(List<VertexInfo>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Road", Road);
            info.AddValue("PoIs", PoIs);
        }
    }
}

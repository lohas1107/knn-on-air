using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KNNonAir.Access
{
    [Serializable]
    public class BorderInfo : ISerializable
    {
        public VertexInfo Vertex { get; set; }
        public List<VertexInfo> PoIs { get; set; }

        public BorderInfo()
        {
            Vertex = new VertexInfo();
            PoIs = new List<VertexInfo>();
        }

        public BorderInfo(double lat, double lng)
        {
            Vertex = new VertexInfo(lat, lng);
            PoIs = new List<VertexInfo>();
        }

        public BorderInfo(SerializationInfo info, StreamingContext context)
        {
            Vertex = (VertexInfo)info.GetValue("BorderPoint", typeof(VertexInfo));
            PoIs = (List<VertexInfo>)info.GetValue("PoIs", typeof(List<VertexInfo>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BorderPoint", Vertex);
            info.AddValue("PoIs", PoIs);
        }
    }
}

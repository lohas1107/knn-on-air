using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KNNonAir.Access
{
    [Serializable]
    public class NVCInfo : ISerializable
    {
        public VertexInfo PoI { get; set; }
        public List<EdgeInfo> Graph { get; set; }
        public List<VertexInfo> BPs { get; set; }

        public NVCInfo()
        {
            PoI = new VertexInfo();
            Graph = new List<EdgeInfo>();
            BPs = new List<VertexInfo>();
        }

        // Deserialization constructor
        public NVCInfo(SerializationInfo info, StreamingContext context)
        {
            PoI = (VertexInfo)info.GetValue("PoI", typeof(VertexInfo));
            Graph = (List<EdgeInfo>)info.GetValue("Graph", typeof(List<EdgeInfo>));
            BPs = (List<VertexInfo>)info.GetValue("BPs", typeof(List<VertexInfo>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PoI", PoI);
            info.AddValue("Graph", Graph);
            info.AddValue("BPs", BPs);
        }
    }
}

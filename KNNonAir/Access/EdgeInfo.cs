using System;
using System.Runtime.Serialization;

namespace KNNonAir.Access
{
    [Serializable]
    public class EdgeInfo : ISerializable
    {
        public VertexInfo Source { get; set; }
        public VertexInfo Target { get; set; }

        public EdgeInfo() { }

        public EdgeInfo(VertexInfo source, VertexInfo target)
        {
            Source = source;
            Target = target;
        }

        public EdgeInfo(SerializationInfo info, StreamingContext context)
        {
            Source = (VertexInfo)info.GetValue("Source", typeof(VertexInfo));
            Target = (VertexInfo)info.GetValue("Target", typeof(VertexInfo));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Source", Source);
            info.AddValue("Target", Target);
        }
    }
}

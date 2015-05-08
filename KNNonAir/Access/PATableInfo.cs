using System;
using System.Runtime.Serialization;
using KNNonAir.Domain.Entity;

namespace KNNonAir.Access
{
    [Serializable]
    public class PATableInfo : ISerializable
    {
        public int PoICount { get; set; }
        public MBR BorderMBR { get; set; }

        public PATableInfo(int poICount, MBR borderMBR)
        {
            PoICount = poICount;
            BorderMBR = borderMBR;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PoICount", PoICount);
            info.AddValue("BorderMBR", BorderMBR);
        }
    }
}

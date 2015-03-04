using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KNNonAir.Access
{
    [Serializable]
    public class NVDItem : ISerializable
    {
        public double[] PoI { get; set; }
        //public List<Tuple<double[], double[]>> Graph { get; set; }
        //public List<double[]> BPs { get; set; }

        public NVDItem() { }

        // Deserialization constructor
        public NVDItem(SerializationInfo info, StreamingContext context)
        {
            PoI = (double[])info.GetValue("PoI", typeof(double[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PoI", PoI.ToString());
        }
    }
}

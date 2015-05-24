using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KNNonAir.Access
{
    [Serializable]
    public class NPITableInfo : ISerializable
    {
        public Dictionary<int, Tuple<int, double>> CountDiameterTable { get; set; }
        public Dictionary<int, Dictionary<int, Tuple<double, double>>> MinMaxTable { get; set; }

        public NPITableInfo() { }

        public NPITableInfo(Dictionary<int, Tuple<int, double>> countDiameterTable, Dictionary<int, Dictionary<int, Tuple<double, double>>> minMaxTable)
        {
            CountDiameterTable = countDiameterTable;
            MinMaxTable = minMaxTable;
        }

        public NPITableInfo(SerializationInfo info, StreamingContext context)
        {
            CountDiameterTable = (Dictionary<int, Tuple<int, double>>)info.GetValue("CountDiameterTable", typeof(Dictionary<int, Tuple<int, double>>));
            MinMaxTable = (Dictionary<int, Dictionary<int, Tuple<double, double>>>)info.GetValue("MinMaxTable", typeof(Dictionary<int, Dictionary<int, Tuple<double, double>>>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CountDiameterTable", CountDiameterTable);
            info.AddValue("MinMaxTable", MinMaxTable);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KNNonAir.Access
{
    [Serializable]
    public class EBTableInfo : ISerializable
    {
        public List<VertexInfo> PoIs { get; set; }
        public Dictionary<int, Dictionary<int, double>> MinTable { get; set; }
        public Dictionary<int, Tuple<int, double>> MaxCountTable { get; set; }

        public EBTableInfo() { }

        public EBTableInfo(List<VertexInfo> pois, Dictionary<int, Dictionary<int, double>> minTable, Dictionary<int, Tuple<int, double>> maxCountTable)
        {
            PoIs = pois;
            MinTable = minTable;
            MaxCountTable = maxCountTable;
        }

        public EBTableInfo(SerializationInfo info, StreamingContext context)
        {
            MinTable = (Dictionary<int, Dictionary<int, double>>)info.GetValue("MinTable", typeof(Dictionary<int, Dictionary<int, double>>));
            MaxCountTable = (Dictionary<int, Tuple<int, double>>)info.GetValue("MaxCountTable", typeof(Dictionary<int, Tuple<int, double>>));
            PoIs = (List<VertexInfo>)info.GetValue("PoIs", typeof(List<VertexInfo>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MinTable", MinTable);
            info.AddValue("MaxCountTable", MaxCountTable);
            info.AddValue("PoIs", PoIs);
        }
    }
}

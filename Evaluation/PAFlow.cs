using System;
using System.Collections.Generic;
using KNNonAir.Domain.Context;

namespace Evaluation
{
    class PAFlow : Flow
    {
        public PAFlow(Model model, string[] args) : base(model, args)
        {
            NVD_PATH = _args[0];
            REGION_NUMBER = Convert.ToInt16(_args[1]);
            ALGORITHM_CATEGORY = _args[2];
            K = Convert.ToInt16(_args[3]);
            PACKET_SIZE = Convert.ToDouble(_args[4]);

            Console.WriteLine(String.Join("_", args));
        }

        public override void SearchKNN()
        {
            _model.AddNVD(NVD_PATH);
            _model.Partition(REGION_NUMBER);
            _model.InitializeAlgorithm(ALGORITHM_CATEGORY);
            _model.GenerateIndex();
            _model.ComputeTable();
            _model.SearchKNN(K);
        }

        public override void Evaluate()
        {
            INDEX_SIZE = _model.GetSize(_model.PA.ShortcutNetwork, PACKET_SIZE);
            TABLE_SIZE = _model.GetSize(_model.PA.PATable, PACKET_SIZE);
            REGIONS_SIZE = _model.GetSize(_model.Regions, PACKET_SIZE);
            TUNING_SIZE = _model.GetSize(_model.PA.Tuning, PACKET_SIZE);
            LATENCY_SIZE = _model.GetSize(_model.PA.Latency, PACKET_SIZE);

            Tuning.Add(INDEX_SIZE + TABLE_SIZE + TUNING_SIZE);
            Latency.Add(INDEX_SIZE + TABLE_SIZE + LATENCY_SIZE); 
        }

        public override void OutputResult(int frequency)
        {
            BROCAST_LENGTH = (INDEX_SIZE + TABLE_SIZE) * REGION_NUMBER + REGIONS_SIZE;

            List<string> argList = new List<string>();
            argList.Add(ALGORITHM_CATEGORY);
            argList.Add(NVD_PATH.Substring(0, 2));
            argList.Add(REGION_NUMBER.ToString());
            argList.Add(K.ToString());
            argList.Add(PACKET_SIZE.ToString());
            OUTPUT_PATH = String.Join("_", argList) + ".txt";

            base.OutputResult(frequency);
        }
    }
}

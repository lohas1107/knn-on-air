using System;
using System.Collections.Generic;
using KNNonAir.Domain.Context;

namespace Evaluation
{
    class EBFlow : Flow
    {
        public EBFlow(Model model, string[] args) : base(model, args)
        {
            NVD_PATH = _args[0];
            REGION_NUMBER = Convert.ToInt16(_args[1]);
            ALGORITHM_CATEGORY = _args[2];
            EBTABLE_PATH = _args[3];
            K = Convert.ToInt16(_args[4]);
            PACKET_SIZE = Convert.ToDouble(_args[5]);

            Console.WriteLine(String.Join("_", args));
        }

        public override void SearchKNN()
        {
            _model.AddNVD(NVD_PATH);
            _model.Partition(REGION_NUMBER);
            _model.InitializeStrategy(ALGORITHM_CATEGORY);
            _model.GenerateIndex();
            _model.AddEBTable(EBTABLE_PATH);
            _model.SearchKNN(K);
        }

        public override void Evaluate()
        {
            INDEX_SIZE = _model.GetSize(_model.EB.VQTree, PACKET_SIZE);
            TABLE_SIZE = _model.GetSize(_model.EB, PACKET_SIZE);
            REGIONS_SIZE = _model.GetSize(_model.Regions, PACKET_SIZE);
            TUNING_SIZE = _model.GetSize(_model.EB.Tuning, PACKET_SIZE);
            LATENCY_SIZE = _model.GetSize(_model.EB.Latency, PACKET_SIZE) + _model.GetSize(_model.EB.Overflow, PACKET_SIZE);

            Tuning.Add(INDEX_SIZE + TABLE_SIZE + TUNING_SIZE);
            Latency.Add(INDEX_SIZE + TABLE_SIZE + LATENCY_SIZE);            
        }

        public override void OutputResult(int frequency)
        {
            BROCAST_LENGTH = INDEX_SIZE + TABLE_SIZE + REGIONS_SIZE;

            List<string> argList = new List<string>();
            argList.Add(ALGORITHM_CATEGORY);
            argList.Add(EBTABLE_PATH.Substring(0, 5));
            argList.Add(K.ToString());
            argList.Add(PACKET_SIZE.ToString());
            OUTPUT_PATH = String.Join("_", argList) + ".txt";

            base.OutputResult(frequency);
        }
    }
}

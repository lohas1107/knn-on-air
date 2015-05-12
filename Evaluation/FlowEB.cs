using System;
using System.Collections.Generic;
using KNNonAir.Domain.Context;

namespace Evaluation
{
    class FlowEB : Flow
    {
        public FlowEB(Model model, string[] args) : base(model, args)
        {
            NVD_PATH = _args[0];
            REGION_NUMBER = Convert.ToInt16(_args[1]);
            ALGORITHM_CATEGORY = _args[2];
            EBTABLE_PATH = _args[3];
            K = Convert.ToInt16(_args[4]);
            PACKET_SIZE = Convert.ToDouble(_args[5]);

            Console.WriteLine(String.Join("_", args));
        }

        public override void Initialize()
        {
            _model.AddNVD(NVD_PATH);
            _model.Partition(REGION_NUMBER);
            _model.InitializeAlgorithm(ALGORITHM_CATEGORY);
            _model.GenerateIndex();
            _model.AddEBTable(EBTABLE_PATH);
           
            INDEX_SIZE = _model.GetSize(_model.EB.VQTree, PACKET_SIZE);
            TABLE_SIZE = _model.GetSize(_model.EB, PACKET_SIZE);
            REGIONS_SIZE = _model.GetSize(_model.Regions, PACKET_SIZE);
        }

        public override void SearchKNN()
        {
            _model.SearchKNN(K);
        }

        public override void Evaluate()
        {
            TUNING_SIZE = _model.GetSize(_model.EB.Tuning, PACKET_SIZE);
            LATENCY_SIZE = _model.GetSize(_model.EB.Latency, PACKET_SIZE) + _model.GetSize(_model.EB.Overflow, PACKET_SIZE);

            Tuning.Add(INDEX_SIZE + TABLE_SIZE + TUNING_SIZE);
            Latency.Add(INDEX_SIZE + TABLE_SIZE + LATENCY_SIZE);            
        }

        public override void OutputResult(int frequency)
        {
            BROCAST_LENGTH = INDEX_SIZE + TABLE_SIZE + REGIONS_SIZE;

            base.OutputResult(frequency);
        }
    }
}

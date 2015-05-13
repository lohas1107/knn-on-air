using System;
using System.Collections.Generic;
using KNNonAir.Domain.Context;

namespace Evaluation
{
    class FlowPA : Flow
    {
        public FlowPA(Model model, string[] args) : base(model, args)
        {
            NVD_PATH = _args[0];
            REGION_NUMBER = Convert.ToInt16(_args[1]);
            ALGORITHM_CATEGORY = _args[2];
            K = Convert.ToInt16(_args[3]);
            PACKET_SIZE = Convert.ToDouble(_args[4]);

            Console.WriteLine(String.Join("_", args));
        }

        public override void Initialize()
        {
            _model.AddNVD(NVD_PATH);
            _model.Partition(REGION_NUMBER);
            _model.InitializeAlgorithm(ALGORITHM_CATEGORY);
            _model.GenerateIndex();
            _model.ComputeTable();

            INDEX_SIZE = _model.GetSize(_model.PA.ShortcutNetwork, PACKET_SIZE);
            TABLE_SIZE = _model.GetSize(_model.PA.PATable, PACKET_SIZE);
            REGIONS_SIZE = _model.GetSize(_model.Regions, PACKET_SIZE);
        }

        public override void SearchKNN()
        {
            _model.SearchKNN(K);
        }

        public override void Evaluate()
        {
            TUNING_SIZE = _model.GetSize(_model.PA.Tuning, PACKET_SIZE);
            LATENCY_SIZE = _model.GetSize(_model.PA.Latency, PACKET_SIZE);

            int slot = 0;
            if (_model.PA.End < _model.PA.Start) slot = _model.PA.End + REGION_NUMBER - _model.PA.Start + 1;
            else slot = _model.PA.End - _model.PA.Start + 1;

            Tuning.Add(INDEX_SIZE + TABLE_SIZE + TUNING_SIZE);
            Latency.Add((INDEX_SIZE + TABLE_SIZE) * slot + LATENCY_SIZE); 
        }

        public override void OutputResult(int frequency)
        {
            BROCAST_LENGTH = (INDEX_SIZE + TABLE_SIZE) * REGION_NUMBER + REGIONS_SIZE;

            base.OutputResult(frequency);
        }
    }
}

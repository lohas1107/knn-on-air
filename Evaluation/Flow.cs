using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KNNonAir.Domain.Context;

namespace Evaluation
{
    abstract class Flow
    {
        protected Model _model;
        protected string[] _args;

        protected string NVD_PATH;
        protected int REGION_NUMBER;
        protected string ALGORITHM_CATEGORY;
        protected string EBTABLE_PATH;
        protected int K;
        protected double PACKET_SIZE;

        protected double BROCAST_LENGTH;
        protected double INDEX_SIZE;
        protected double TABLE_SIZE;
        protected double REGIONS_SIZE;
        protected double TUNING_SIZE;
        protected double LATENCY_SIZE;

        protected string OUTPUT_PATH;

        protected List<double> Tuning { get; set; }
        protected List<double> Latency { get; set; }

        public Flow(Model model, string[] args)
        {
            _model = model;
            _args = args;

            Tuning = new List<double>();
            Latency = new List<double>();
        }

        public abstract void SearchKNN();
        public abstract void Evaluate();

        public virtual void OutputResult(int frequency)
        {
            List<string> argList = new List<string>();
            argList.Add(ALGORITHM_CATEGORY);
            argList.Add(NVD_PATH.Substring(0, 2));
            argList.Add(REGION_NUMBER.ToString());
            argList.Add(K.ToString());
            argList.Add(PACKET_SIZE.ToString());
            OUTPUT_PATH = String.Join("_", argList) + ".txt";

            File.AppendAllText(OUTPUT_PATH, "Brocast Length: " + BROCAST_LENGTH + Environment.NewLine);
            File.AppendAllText(OUTPUT_PATH, "| Latency | Tuning |" + Environment.NewLine);

            for (int i = 0; i < frequency; i++)
            {
                File.AppendAllText(OUTPUT_PATH, Latency[i] + " | ");
                File.AppendAllText(OUTPUT_PATH, Tuning[i] + Environment.NewLine);
            }

            File.AppendAllText(OUTPUT_PATH, "| Average |" + Environment.NewLine);
            File.AppendAllText(OUTPUT_PATH, Latency.Average() + " | ");
            File.AppendAllText(OUTPUT_PATH, Tuning.Average() + Environment.NewLine);
        }
    }
}

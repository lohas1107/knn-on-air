using System;
using System.Linq;
using KNNonAir.Domain.Context;

namespace Evaluation
{
    class Program
    {
        private const int FREQUENCY = 500;

        private static Flow _flow;

        static void Main(string[] args)
        {
            if (args.Count() == 0) return;

            Model model = new Model();

            if (args[2] == "EB") _flow = new FlowEB(model, args);
            else if (args[2] == "PA") _flow = new FlowPA(model, args);

            _flow.Initialize();

            for (int i = 0; i < FREQUENCY; i++)
            {
                _flow.SearchKNN();
                _flow.Evaluate();
            }

            _flow.OutputResult(FREQUENCY);

            Console.WriteLine("Finish!");
            Console.ReadKey();
        }
    }
}

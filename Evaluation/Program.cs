using System;
using System.IO;
using System.Linq;

namespace Evaluation
{
    class Program
    {
        private static EBAlgorithm _eb;

        static void Main(string[] args)
        {
            if (args.Count() == 0) return;

            if (args[0] == "1") _eb = new EBAlgorithm(args.Skip(0).ToArray<String>());
        }
    }
}

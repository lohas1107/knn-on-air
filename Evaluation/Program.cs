using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 0) return;

            File.AppendAllText("test.txt", "456" + Environment.NewLine);

            Console.WriteLine("Finish!");
            Console.ReadLine();
        }
    }
}

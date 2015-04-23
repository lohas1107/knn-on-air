using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Evaluation
{
    class EBAlgorithm
    {
        public EBAlgorithm(string[] args)
        {
            if (args[0] == "1") RunVariousK();
        }

        private void RunVariousK()
        {
            File.AppendAllText("guam_32_128_K.txt", "Guam | 32 frames | 128 bytes per packet" + Environment.NewLine);
        }
    }
}

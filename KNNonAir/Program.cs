using KNNonAir.Domain.Entity;
using KNNonAir.Domain.Service;
using KNNonAir.Presentation;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KNNonAir
{
    delegate void PathNodeHandler(PathTreeNode node);
    delegate void BorderPointHandler(Vertex borderPoint, Edge<Vertex> edge);
    delegate void RegionHandler(Region region);
    delegate void MBRHandler(MBR mbr);

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

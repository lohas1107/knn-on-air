using KNNonAir.Domain.Entity;
using KNNonAir.Domain.Service;
using KNNonAir.Presentation;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KNNonAir
{
    delegate void Handler();
    delegate void VertexListHandler(List<Vertex> vertexs);
    delegate void PathNodeHandler(PathNode node);
    delegate void RegionHandler(Region region);

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

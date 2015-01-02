﻿using GMap.NET;
using GMap.NET.WindowsForms;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KNNonAir
{
    public partial class MainForm : Form
    {
        private RoadNetwork _roadNetwork;

        public MainForm()
        {
            InitializeComponent();

            PointLatLng GUAM = new PointLatLng(13.45, 144.783333);
            InitializeGMap(GUAM, 11);

            _roadNetwork = new RoadNetwork();
        }

        private void InitializeGMap(PointLatLng center, double zoom)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gmap.DragButton = MouseButtons.Left;
            gmap.Position = center;
            gmap.Zoom = zoom;
        }

        private void MouseMoveGMap(object sender, MouseEventArgs e)
        {
            gmapToolStripStatusLabel.Text = gmap.FromLocalToLatLng(e.X, e.Y).ToString();
        }

        private void MouseLeaveGMap(object sender, System.EventArgs e)
        {
            gmapToolStripStatusLabel.Text = string.Empty;
        }

        private void ClickAddRoadsToolStripMenuItem(object sender, System.EventArgs e)
        {
            _roadNetwork.Graph.Clear();
            _roadNetwork.Graph = FileIO.ReadRoadFile();
            DrawRoad();
        }

        private void DrawRoad()
        {
            gmap.Overlays.Clear();

            GMapOverlay routesOverlay = new GMapOverlay("routes");
            List<MapRoute> mapRouteList = _roadNetwork.GetMapRouteList();

            foreach (MapRoute mapRoute in mapRouteList)
            {
                GMapRoute route = new GMapRoute(mapRoute.Points, "");
                route.Stroke.Width = 10;
                route.Stroke.Color = Color.SeaGreen;
                routesOverlay.Routes.Add(route);
            }

            gmap.Overlays.Add(routesOverlay);
        }
    }
}

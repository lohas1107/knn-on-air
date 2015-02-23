using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KNNonAir
{
    public partial class MainForm : Form
    {
        private RoadNetwork _roadNetwork;
        private PresentationModel _presentationModel;
        private GMapOverlay _polyOverlay;
        private GMapOverlay _markersOverlay;

        public MainForm()
        {
            InitializeComponent();

            PointLatLng GUAM = new PointLatLng(13.45, 144.783333);
            InitializeGMap(GUAM, 11);

            _roadNetwork = new RoadNetwork();
            _roadNetwork.LoadRoadsCompleted += DrawRoads;
            _roadNetwork.LoadPoIsCompleted += DrawPoIs;
            _roadNetwork.GenerateNVDCompleted += DrawNVD;

            _presentationModel = new PresentationModel(_roadNetwork);
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
            _roadNetwork.LoadRoads();
        }

        private void DrawRoads()
        {
            gmap.Overlays.Remove(_polyOverlay);
            _polyOverlay = new GMapOverlay("polygons");

            foreach (Edge<Vertex> edge in _roadNetwork.Graph.Edges)
            {
                List<PointLatLng> points = new List<PointLatLng>();
                points.Add(new PointLatLng(edge.Source.Coordinate.Latitude, edge.Source.Coordinate.Longitude));
                points.Add(new PointLatLng(edge.Target.Coordinate.Latitude, edge.Target.Coordinate.Longitude));
                GMapPolygon polygon = new GMapPolygon(points, "");
                polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
                polygon.Stroke = new Pen(Color.FromArgb(80, Color.Red), 3);
                _polyOverlay.Polygons.Add(polygon);
            }

             gmap.Overlays.Add(_polyOverlay);

             gmap.Overlays.Remove(_markersOverlay);
             _markersOverlay = new GMapOverlay("marker");

             foreach (Vertex site in _roadNetwork.GetSideVertexs())
             {
                 GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(site.Coordinate.Latitude, site.Coordinate.Longitude), GMarkerGoogleType.red_dot);
                 _markersOverlay.Markers.Add(marker);
             }

             gmap.Overlays.Add(_markersOverlay);
        }

        private void ClickAddLandMarkToolStripMenuItem(object sender, System.EventArgs e)
        {
            _roadNetwork.LoadPoIs();
        }

        private void DrawPoIs()
        {
            gmap.Overlays.Remove(_markersOverlay);
            _markersOverlay = new GMapOverlay("marker");

            foreach (Vertex site in _roadNetwork.PoIs)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(site.Coordinate.Latitude, site.Coordinate.Longitude), GMarkerGoogleType.red_dot);
                _markersOverlay.Markers.Add(marker);
            }

            gmap.Overlays.Add(_markersOverlay);            
        }

        private void ClickNvdToolStripButton(object sender, System.EventArgs e)
        {
            _roadNetwork.GenerateNVD();
        }

        private void DrawNVD()
        {
            gmap.Overlays.Remove(_polyOverlay);
            _polyOverlay = new GMapOverlay("polygons");

            foreach (Tuple<Color, List<PointLatLng>> edge in _presentationModel.GetNVDEdges())
            {
                GMapPolygon polygon = new GMapPolygon(edge.Item2, "");
                polygon.Fill = new SolidBrush(Color.FromArgb(50, edge.Item1));
                polygon.Stroke = new Pen(edge.Item1, 5);
                _polyOverlay.Polygons.Add(polygon);
            }

            gmap.Overlays.Add(_polyOverlay);
        }
        int i = 0;
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            i++;
            DrawNVD();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using KNNonAir.Access;
using KNNonAir.Domain.Service;

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
            _roadNetwork.LoadPoIsCompleted += DrawMarkers;
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

        private void MouseLeaveGMap(object sender, EventArgs e)
        {
            gmapToolStripStatusLabel.Text = string.Empty;
        }

        private void ClickAddRoadsToolStripMenuItem(object sender, EventArgs e)
        {
            _roadNetwork.LoadRoads();
        }

        private void DrawRoads()
        {
            gmap.Overlays.Remove(_polyOverlay);
            _polyOverlay = new GMapOverlay("polygons");

            foreach (List<PointLatLng> points in _presentationModel.GetRoads())
            {
                SetPolygon(points, Color.Red, 100);
            }
            
            gmap.Overlays.Add(_polyOverlay);

            // 標示孤點
            DrawMarkers(_roadNetwork.GetSideVertexs());
        }

        private void SetPolygon(List<PointLatLng> points, Color color, int alpha)
        {
            GMapPolygon polygon = new GMapPolygon(points, "");
            polygon.Fill = new SolidBrush(Color.FromArgb(alpha, color));
            polygon.Stroke = new Pen(Color.FromArgb(alpha, color), 5);
            _polyOverlay.Polygons.Add(polygon);
        }

        private void ClickAddLandMarkToolStripMenuItem(object sender, EventArgs e)
        {
            _roadNetwork.LoadPoIs();
        }

        private void DrawMarkers(List<Vertex> vertexs)
        {
            gmap.Overlays.Remove(_markersOverlay);
            _markersOverlay = new GMapOverlay("marker");

            foreach (Vertex site in vertexs)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(site.Coordinate.Latitude, site.Coordinate.Longitude), GMarkerGoogleType.red_dot);
                _markersOverlay.Markers.Add(marker);
            }

            gmap.Overlays.Add(_markersOverlay);            
        }

        private void ClickNvdToolStripButton(object sender, EventArgs e)
        {
            _roadNetwork.GenerateNVD();
        }

        private void DrawNVD()
        {
            gmap.Overlays.Remove(_polyOverlay);
            _polyOverlay = new GMapOverlay("polygons");

            foreach (Tuple<Color, List<PointLatLng>> edge in _presentationModel.GetNVDEdges())
            {
                SetPolygon(edge.Item2, edge.Item1, 255);
            }

            gmap.Overlays.Add(_polyOverlay);

            DrawMarkers(_roadNetwork.PoIs);
        }

        private void ClickSaveNVDToolStripMenuItem(object sender, EventArgs e)
        {
            FileIO.SaveNVDFile(Parser.ParseNVD(_roadNetwork.NVD));
        }

        private void ClickAddNVDToolStripMenuItem(object sender, EventArgs e)
        {
            List<NVCInfo> nvcList = FileIO.ReadNVDFile();
            _roadNetwork.NVD = Parser.ParseNVCInfo(nvcList);
            _roadNetwork.PoIs = Parser.ParsePoIInfo(nvcList);
            DrawNVD();
        }
    }
}

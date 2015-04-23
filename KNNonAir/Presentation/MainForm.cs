using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using KNNonAir.Domain.Context;
using KNNonAir.Domain.Entity;
using KNNonAir.Domain.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KNNonAir.Presentation
{
    public partial class MainForm : Form
    {
        private RoadNetwork _roadNetwork;
        private PresentationModel _presentationModel;
        private GMapOverlay _polyOverlay;
        private GMapOverlay _markersOverlay;
        private GMapOverlay _mbrOverlay;
        private GMapOverlay _answerOverlay;
        private int _packetSize;

        public MainForm()
        {
            InitializeComponent();

            PointLatLng GUAM = new PointLatLng(13.45, 144.783333);
            InitializeGMap(GUAM, 11);

            _roadNetwork = new RoadNetwork();
            _presentationModel = new PresentationModel(_roadNetwork);
            _packetSize = Convert.ToInt32(packetToolStripComboBox.SelectedItem);

            dataGridView.Rows.Add(2);
            dataGridView.Rows[0].HeaderCell.Value = "EB";
            dataGridView.Rows[1].HeaderCell.Value = "PA";
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

        #region settings of drawing component
        private void SetPolygon(List<PointLatLng> points, Color color, int alpha, int strokeWidth)
        {
            GMapPolygon polygon = new GMapPolygon(points, "");
            polygon.Fill = new SolidBrush(Color.FromArgb(alpha, color));
            polygon.Stroke = new Pen(Color.FromArgb(alpha, color), strokeWidth);
            _polyOverlay.Polygons.Add(polygon);
        }

        private void DrawLines(List<List<PointLatLng>> lines)
        {
            gmap.Overlays.Remove(_polyOverlay);
            _polyOverlay = new GMapOverlay("polygons");

            foreach (List<PointLatLng> points in lines)
            {
                SetPolygon(points, Color.Red, 100, 5);
            }

            gmap.Overlays.Add(_polyOverlay);
        }

        private void DrawColorLines(List<Tuple<Color, List<PointLatLng>>> nvdEdges)
        {
            gmap.Overlays.Remove(_polyOverlay);
            _polyOverlay = new GMapOverlay("polygons");

            foreach (Tuple<Color, List<PointLatLng>> edge in nvdEdges)
            {
                SetPolygon(edge.Item2, edge.Item1, 255, 5);
            }

            gmap.Overlays.Add(_polyOverlay);
        }

        private void DrawAnswer(Vertex query, List<Vertex> answers)
        {
            gmap.Overlays.Remove(_answerOverlay);
            _answerOverlay = new GMapOverlay("marker");

            GMarkerGoogle markerQuery = new GMarkerGoogle(new PointLatLng(query.Coordinate.Latitude, query.Coordinate.Longitude), GMarkerGoogleType.red_dot);
            _answerOverlay.Markers.Add(markerQuery);

            foreach (Vertex site in answers)
            {
                GMarkerGoogle markerAnswer = new GMarkerGoogle(new PointLatLng(site.Coordinate.Latitude, site.Coordinate.Longitude), GMarkerGoogleType.orange_dot);
                _answerOverlay.Markers.Add(markerAnswer);
            }

            gmap.Overlays.Add(_answerOverlay);
        }

        private void DrawMarkers(List<Vertex> vertexs)
        {
            gmap.Overlays.Remove(_markersOverlay);
            _markersOverlay = new GMapOverlay("marker");

            foreach (Vertex site in vertexs)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(site.Coordinate.Latitude, site.Coordinate.Longitude), GMarkerGoogleType.green_small);
                _markersOverlay.Markers.Add(marker);
            }

            gmap.Overlays.Add(_markersOverlay);
        }

        private void DrawMBRs()
        {
            gmap.Overlays.Remove(_mbrOverlay);
            _mbrOverlay = new GMapOverlay("mbr");

            foreach (List<PointLatLng> points in _presentationModel.GetVQTree())
            {
                SetPolygon(points, Color.Red, 100, 1);
            }

            gmap.Overlays.Add(_mbrOverlay);
        }
        #endregion

        private void ClickAddRoadsToolStripMenuItem(object sender, EventArgs e)
        {
            _roadNetwork.LoadRoads();
            DrawLines(_presentationModel.GetRoads());
            DrawMarkers(_roadNetwork.Road.GetSideVertexs()); // 標示孤點
        }

        private void ClickAddLandMarkToolStripMenuItem(object sender, EventArgs e)
        {
            _roadNetwork.LoadPoIs();
            DrawMarkers(_roadNetwork.PoIs);
        }

        private void ClickNvdToolStripButton(object sender, EventArgs e)
        {
            _roadNetwork.GenerateNVD();
            DrawColorLines(_presentationModel.GetNVDEdges());
            DrawMarkers(_roadNetwork.PoIs);
        }

        private void ClickSaveNVDToolStripMenuItem(object sender, EventArgs e)
        {
            FileIO.SaveNVDFile(Parser.ParseNVD(_roadNetwork.NVD));
        }

        private void ClickAddNVDToolStripMenuItem(object sender, EventArgs e)
        {
            _roadNetwork.AddNVD();
            DrawColorLines(_presentationModel.GetNVDEdges());
        }

        private void ClickPartitionToolStripButton(object sender, EventArgs e)
        {
            _roadNetwork.Partition(Convert.ToInt32(partitionToolStripComboBox.SelectedItem));
            DrawColorLines(_presentationModel.GetRegionEdges());
        }

        private void PacketToolStripComboBoxTextChanged(object sender, EventArgs e)
        {
            _packetSize = Convert.ToInt32(packetToolStripComboBox.SelectedItem);
        }

        private void ClickQuadTreeToolStripButton(object sender, EventArgs e)
        {
            _roadNetwork.GenerateVQTree();
            DrawMBRs();
            dataGridView.Rows[0].Cells[0].Value = _roadNetwork.GetSize(_roadNetwork.VQTree, _packetSize);
        }

        private void ClickTableToolStripButton(object sender, EventArgs e)
        {
            _roadNetwork.ComputeEBTable();
            dataGridView.Rows[0].Cells[1].Value = _roadNetwork.GetSize(_roadNetwork.EBTable, _packetSize);
        }

        private void ClickSearchToolStripButton(object sender, EventArgs e)
        {
            _roadNetwork.SearchKNN(Convert.ToInt32(kToolStripComboBox.SelectedItem));
            DrawMarkers(_roadNetwork.PoIs);
            DrawAnswer(_roadNetwork.QueryPoint, _roadNetwork.Answers);
            dataGridView.Rows[0].Cells[2].Value = _roadNetwork.GetSize(_roadNetwork.Regions, _packetSize);
            dataGridView.Rows[0].Cells[3].Value = _roadNetwork.GetSize(_roadNetwork.Latency, _packetSize);
            dataGridView.Rows[0].Cells[4].Value = _roadNetwork.GetSize(_roadNetwork.Tuning, _packetSize);
        }

        private void ClickShortcutToolStripButton(object sender, EventArgs e)
        {
            _roadNetwork.GenerateSN();
            dataGridView.Rows[1].Cells[0].Value = _roadNetwork.GetSize(_roadNetwork.Shortcut, _packetSize);
        }

        private void ClickPATableToolStripButton(object sender, EventArgs e)
        {
            _roadNetwork.ComputePATable();
            dataGridView.Rows[1].Cells[1].Value = _roadNetwork.GetSize(_roadNetwork.PATable, _packetSize);
        }

        private void ClickSaveEBTableToolStripMenuItem(object sender, EventArgs e)
        {
            _roadNetwork.SaveEBTable();
        }

        private void ClickAddEBTableToolStripMenuItem(object sender, EventArgs e)
        {
            _roadNetwork.AddEBTable();
            dataGridView.Rows[0].Cells[1].Value = _roadNetwork.GetSize(_roadNetwork.EBTable, _packetSize);
        }
    }
}

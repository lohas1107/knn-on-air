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
        private Model _model;
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

            _model = new Model();
            _presentationModel = new PresentationModel(_model);
            _packetSize = Convert.ToInt32(packetToolStripComboBox.SelectedItem);

            dataGridView.Rows.Add(3);
            dataGridView.Rows[0].HeaderCell.Value = "EB";
            dataGridView.Rows[1].HeaderCell.Value = "PA";
            dataGridView.Rows[2].HeaderCell.Value = "NPI";
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

        private void DrawMBRs(List<List<PointLatLng>> mbrs)
        {
            gmap.Overlays.Remove(_mbrOverlay);
            _mbrOverlay = new GMapOverlay("mbr");

            foreach (List<PointLatLng> points in mbrs)
            {
                SetPolygon(points, Color.Red, 100, 1);
            }

            gmap.Overlays.Add(_mbrOverlay);
        }
        #endregion

        // File
        private void ClickReadFileToolStripSplitButton(object sender, EventArgs e)
        {
            _model.AddNVD(null);
            DrawColorLines(_presentationModel.GetNVDEdges());
        }

        private void ClickAddRoadsToolStripMenuItem(object sender, EventArgs e)
        {
            _model.LoadRoads();
            DrawLines(_presentationModel.GetRoads());
            DrawMarkers(_model.Road.GetSideVertexs()); // 標示孤點
        }

        private void ClickAddPoIToolStripMenuItem(object sender, EventArgs e)
        {
            _model.LoadPoIs();
            DrawMarkers(_model.PoIs);
        }

        private void ClickAddNVDToolStripMenuItem(object sender, EventArgs e)
        {
            _model.AddNVD(null);
            DrawColorLines(_presentationModel.GetNVDEdges());
        }

        private void ClickAddEBTableToolStripMenuItem(object sender, EventArgs e)
        {
            _model.AddEBTable(null);
            dataGridView.Rows[0].Cells[1].Value = _model.GetSize(_model.EB, _packetSize);
        }

        private void ClickAddNPITableToolStripMenuItem(object sender, EventArgs e)
        {
            _model.AddNPITable(null);
            dataGridView.Rows[2].Cells[1].Value = _model.GetSize(_model.NPI.CountDiameterTable, _packetSize) + _model.GetSize(_model.NPI.MinMaxTable, _packetSize);
        }

        private void ClickSaveNVDToolStripMenuItem(object sender, EventArgs e)
        {
            FileIO.SaveNVDFile(Parser.ParseNVD(_model.NVD));
        }

        private void ClickSaveEBTableToolStripMenuItem(object sender, EventArgs e)
        {
            _model.SaveEBTable();
        }

        private void ClickSaveNPITableToolStripMenuItem(object sender, EventArgs e)
        {
            _model.SaveNPITable();
        }

        // Parameters
        private void AlgorithmToolStripComboBoxTextChanged(object sender, EventArgs e)
        {
            _model.ChangeAlgorithm(algorithmToolStripComboBox.Text);
        }

        private void PacketToolStripComboBoxTextChanged(object sender, EventArgs e)
        {
            _packetSize = Convert.ToInt32(packetToolStripComboBox.SelectedItem);
        }

        // Action
        private void ClickNvdToolStripButton(object sender, EventArgs e)
        {
            _model.GenerateNVD();
            DrawColorLines(_presentationModel.GetNVDEdges());
            DrawMarkers(_model.PoIs);
        }

        private void ClickPartitionToolStripButton(object sender, EventArgs e)
        {
            _model.InitializeAlgorithm(algorithmToolStripComboBox.Text);

            _model.Partition(Convert.ToInt32(partitionToolStripComboBox.SelectedItem));
            
            if (algorithmToolStripComboBox.Text == "EB") DrawColorLines(_presentationModel.GetRegionEdges(_model.EB.Regions));
            else if (algorithmToolStripComboBox.Text == "PA") DrawColorLines(_presentationModel.GetRegionEdges(_model.PA.Regions));
            else if (algorithmToolStripComboBox.Text == "NPI") DrawColorLines(_presentationModel.GetRegionEdges(_model.NPI.Regions));
        }

        private void ClickIndexToolStripButton(object sender, EventArgs e)
        {
            _model.GenerateIndex();
            if (algorithmToolStripComboBox.Text == "EB") DrawMBRs(_presentationModel.GetMBRs(_model.EB.VQTree.MBRs));
            if (algorithmToolStripComboBox.Text == "NPI") DrawMBRs(_presentationModel.GetMBRs(_model.NPI.Grids));

            dataGridView.Rows[0].Cells[0].Value = _model.GetSize(_model.EB.VQTree, _packetSize);
            dataGridView.Rows[1].Cells[0].Value = _model.GetSize(_model.PA.ShortcutNetwork, _packetSize);
            dataGridView.Rows[2].Cells[0].Value = _model.GetSize(_model.NPI.Grids, _packetSize);
        }

        private void ClickTableToolStripButton(object sender, EventArgs e)
        {
            _model.ComputeTable();
            dataGridView.Rows[0].Cells[1].Value = _model.GetSize(_model.EB, _packetSize);
            dataGridView.Rows[1].Cells[1].Value = _model.GetSize(_model.PA.PATable, _packetSize);
            dataGridView.Rows[2].Cells[1].Value = _model.GetSize(_model.NPI.CountDiameterTable, _packetSize) + _model.GetSize(_model.NPI.MinMaxTable, _packetSize);
        }

        private void ClickSearchToolStripButton(object sender, EventArgs e)
        {
            _model.SearchKNN(Convert.ToInt32(kToolStripComboBox.SelectedItem));
            DrawMarkers(_model.PoIs);
            DrawAnswer(_model.CurrentAlgorithm.QueryPoint, _model.Answers);

            dataGridView.Rows[0].Cells[2].Value = _model.GetSize(_model.EB.Regions, _packetSize);
            dataGridView.Rows[0].Cells[3].Value = _model.GetSize(_model.EB.Latency, _packetSize) + _model.GetSize(_model.EB.Overflow, _packetSize);
            dataGridView.Rows[0].Cells[4].Value = _model.GetSize(_model.EB.Tuning, _packetSize);

            dataGridView.Rows[1].Cells[2].Value = _model.GetSize(_model.PA.Regions, _packetSize);
            dataGridView.Rows[1].Cells[3].Value = _model.GetSize(_model.PA.Latency, _packetSize);
            dataGridView.Rows[1].Cells[4].Value = _model.GetSize(_model.PA.Tuning, _packetSize);
        }
    }
}

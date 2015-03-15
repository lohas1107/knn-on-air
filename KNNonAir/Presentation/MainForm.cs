﻿using GMap.NET;
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


        public MainForm()
        {
            InitializeComponent();

            PointLatLng GUAM = new PointLatLng(13.45, 144.783333);
            InitializeGMap(GUAM, 11);

            _roadNetwork = new RoadNetwork();
            _roadNetwork.LoadPoIsCompleted += DrawMarkers;

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
            DrawLines(_presentationModel.GetRoads(), 5);
            DrawMarkers(_roadNetwork.GetSideVertexs()); // 標示孤點
        }

        private void DrawLines(List<List<PointLatLng>> lines, int strokeWidth)
        {
            gmap.Overlays.Remove(_polyOverlay);
            _polyOverlay = new GMapOverlay("polygons");

            foreach (List<PointLatLng> points in lines)
            {
                SetPolygon(points, Color.Red, 100, strokeWidth);
            }
            
            gmap.Overlays.Add(_polyOverlay);
        }

        private void SetPolygon(List<PointLatLng> points, Color color, int alpha, int strokeWidth)
        {
            GMapPolygon polygon = new GMapPolygon(points, "");
            polygon.Fill = new SolidBrush(Color.FromArgb(alpha, color));
            polygon.Stroke = new Pen(Color.FromArgb(alpha, color), strokeWidth);
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
            DrawColorLines(_presentationModel.GetNVDEdges());
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

        private void ClickQuadTreeToolStripButton(object sender, EventArgs e)
        {
            _roadNetwork.GenerateVQTree();

            gmap.Overlays.Remove(_mbrOverlay);
            _mbrOverlay = new GMapOverlay("mbrs");

            foreach (List<PointLatLng> points in _presentationModel.GetVQTree())
            {
                SetPolygon(points, Color.Red, 100, 1);
            }

            gmap.Overlays.Add(_mbrOverlay);
        }
    }
}
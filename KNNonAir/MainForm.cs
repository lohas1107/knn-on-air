using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Drawing;
using System.Windows.Forms;

namespace KNNonAir
{
    public partial class MainForm : Form
    {
        private RoadNetwork _roadNetwork;
        private PresentationModel _presentationModel;
        private GMapOverlay _routesOverlay;
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
            gmap.Overlays.Remove(_routesOverlay);
            _routesOverlay = new GMapOverlay("routes");

            foreach (MapRoute mapRoute in _roadNetwork.GetRoadMapRouteList())
            {
                GMapRoute route = new GMapRoute(mapRoute.Points, "");
                route.Stroke.Width = 10;
                route.Stroke.Color = Color.SeaGreen;
                _routesOverlay.Routes.Add(route);
            }

            gmap.Overlays.Add(_routesOverlay);
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
            gmap.Overlays.Remove(_routesOverlay);
            _routesOverlay = new GMapOverlay("routes");

            foreach (MapRoute mapRoute in _presentationModel.GetNVCMapRouteList(_roadNetwork.PoIs[0]))
            {
                GMapRoute route = new GMapRoute(mapRoute.Points, "");
                route.Stroke.Width = 10;
                route.Stroke.Color = Color.Blue;
                _routesOverlay.Routes.Add(route);
            }

            gmap.Overlays.Add(_routesOverlay);
        }
    }
}

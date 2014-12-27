using Geo;
using Geo.Geometries;
using Geo.IO.GeoJson;
using QuickGraph;
using System.IO;
using System.Windows.Forms;

namespace KNNonAir
{
    class FileIO
    {
        static string _fileName;

        private static DialogResult OpenGeoJsonFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "geojson files (*.geojson)|*.geojson";
            openFileDialog.RestoreDirectory = true;

            DialogResult result = openFileDialog.ShowDialog();
            _fileName = openFileDialog.FileName;

            return result;
        }

        internal static AdjacencyGraph<Point, Edge<Point>> ReadRoadFile()
        {
            AdjacencyGraph<Point,Edge<Point>> roadGraph = new AdjacencyGraph<Point,Edge<Point>>(false);

            if (OpenGeoJsonFile() == DialogResult.Cancel) return roadGraph;

            FeatureCollection geoObject = (FeatureCollection)GeoJson.DeSerialize(File.ReadAllText(_fileName));
            foreach (Feature feature in geoObject.Features)
            {
                if (feature.Geometry.GetType() == new MultiLineString().GetType())
                {
                    MultiLineString multiLingString = feature.Geometry as MultiLineString;
                    foreach (LineString lineString in multiLingString.Geometries)
                    {
                        LoadLineString(roadGraph, lineString);
                    }
                }
                else
                {
                    LineString lineString = feature.Geometry as LineString;
                    LoadLineString(roadGraph, lineString);
                }
            }
            return roadGraph;
        }

        private static void LoadLineString(AdjacencyGraph<Point, Edge<Point>> roadGraph, LineString lineString)
        {
            Point source = null;
            Point target = null;

            foreach (Coordinate coordinate in lineString.Coordinates)
            {
                if (source == null)
                {
                    source = new Point(coordinate.Latitude, coordinate.Longitude);
                    roadGraph.AddVertex(source);
                }
                else
                {
                    target = new Point(coordinate.Latitude, coordinate.Longitude);
                    roadGraph.AddVertex(target);
                    roadGraph.AddEdge(new Edge<Point>(source, target));
                    source = target;
                }
            }
        }
    }
}

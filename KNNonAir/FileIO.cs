using Geo;
using Geo.Geometries;
using Geo.IO.GeoJson;
using QuickGraph;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

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

        internal static AdjacencyGraph<Vertex, Edge<Vertex>> ReadRoadFile()
        {
            AdjacencyGraph<Vertex,Edge<Vertex>> roadGraph = new AdjacencyGraph<Vertex,Edge<Vertex>>(false);

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

        private static void LoadLineString(AdjacencyGraph<Vertex, Edge<Vertex>> roadGraph, LineString lineString)
        {
            Vertex source = null;
            Vertex target = null;

            foreach (Coordinate coordinate in lineString.Coordinates)
            {
                if (source == null)
                {
                    source = new Vertex(coordinate.Latitude, coordinate.Longitude);
                    roadGraph.AddVertex(source);
                }
                else
                {
                    target = new Vertex(coordinate.Latitude, coordinate.Longitude);
                    roadGraph.AddVertex(target);
                    roadGraph.AddEdge(new Edge<Vertex>(source, target));
                    source = target;
                }
            }
        }

        public static List<Vertex> ReadSiteFile()
        {
            if (OpenGeoJsonFile() == DialogResult.Cancel) return null;

            List<Vertex> siteList = new List<Vertex>();

            FeatureCollection geoObject = (FeatureCollection)GeoJson.DeSerialize(File.ReadAllText(_fileName));
            foreach (Feature feature in geoObject.Features)
            {
                Point point = feature.Geometry as Point;
                siteList.Add(new Vertex(point.Coordinate.Latitude, point.Coordinate.Longitude));
            }

            return siteList;
        }
    }
}

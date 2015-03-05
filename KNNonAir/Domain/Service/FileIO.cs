using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Geo;
using Geo.Geometries;
using Geo.IO.GeoJson;
using KNNonAir.Access;
using QuickGraph;

namespace KNNonAir
{
    class FileIO
    {
        private const string GEOJSON_FILE_FILTER = "geojson files (*.geojson)|*.geojson";
        private const string XML_FILE_FILTER = "xml files (*.xml)|*.xml";

        private static string _fileName;

        private static DialogResult OpenFile(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.RestoreDirectory = true;

            DialogResult result = openFileDialog.ShowDialog();
            _fileName = openFileDialog.FileName;

            return result;
        }

        private static DialogResult SaveFile(string filter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            saveFileDialog.RestoreDirectory = true;

            DialogResult result = saveFileDialog.ShowDialog();
            _fileName = saveFileDialog.FileName;

            return result;
        }

        public static List<Edge<Vertex>> ReadRoadFile()
        {
            if (OpenFile(GEOJSON_FILE_FILTER) == DialogResult.Cancel) return null;

            List<Edge<Vertex>> edgeList = new List<Edge<Vertex>>();

            FeatureCollection geoObject = (FeatureCollection)GeoJson.DeSerialize(File.ReadAllText(_fileName));
            foreach (Feature feature in geoObject.Features)
            {
                if (feature.Geometry.GetType() == new MultiLineString().GetType())
                {
                    MultiLineString multiLingString = feature.Geometry as MultiLineString;
                    foreach (LineString lineString in multiLingString.Geometries)
                    {
                        LoadLineString(edgeList, lineString);
                    }
                }
                else
                {
                    LineString lineString = feature.Geometry as LineString;
                    LoadLineString(edgeList, lineString);
                }
            }

            return edgeList;
        }

        private static void LoadLineString(List<Edge<Vertex>> edgeList, LineString lineString)
        {
            Vertex source = null;
            Vertex target = null;

            foreach (Coordinate coordinate in lineString.Coordinates)
            {
                if (source == null)
                {
                    source = new Vertex(coordinate.Latitude, coordinate.Longitude);
                }
                else
                {
                    target = new Vertex(coordinate.Latitude, coordinate.Longitude);
                    edgeList.Add(new Edge<Vertex>(source, target));
                    source = target;
                }
            }
        }

        public static List<Vertex> ReadPoIFile()
        {
            if (OpenFile(GEOJSON_FILE_FILTER) == DialogResult.Cancel) return null;

            List<Vertex> poiList = new List<Vertex>();

            FeatureCollection geoObject = (FeatureCollection)GeoJson.DeSerialize(File.ReadAllText(_fileName));
            foreach (Feature feature in geoObject.Features)
            {
                Point point = feature.Geometry as Point;
                poiList.Add(new Vertex(point.Coordinate.Latitude, point.Coordinate.Longitude));
            }

            return poiList;
        }

        public static void SaveNVDFile(List<NVCInfo> nvd)
        {
            if (SaveFile(XML_FILE_FILTER) == DialogResult.Cancel) return;

            StringWriter writer = new StringWriter(new StringBuilder());
            XmlSerializer serializer = new XmlSerializer(typeof(List<NVCInfo>));
            serializer.Serialize(writer, nvd);
            File.WriteAllText(_fileName, writer.ToString());
        }

        public static List<NVCInfo> ReadNVDFile()
        {
            if (OpenFile(XML_FILE_FILTER) == DialogResult.Cancel) return null;
            
            XmlSerializer serializer = new XmlSerializer(typeof(List<NVCInfo>));
            List<NVCInfo> nvdList = (List<NVCInfo>)serializer.Deserialize(File.OpenText(_fileName));

            return nvdList;
        }
    }
}

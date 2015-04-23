using KNNonAir.Access;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace KNNonAir.Domain.Service
{
    class FileIO
    {
        private const string GEOJSON_FILE_FILTER = "geojson files (*.geojson)|*.geojson";
        private const string XML_FILE_FILTER = "xml files (*.xml)|*.xml";
        private const string JSON_FILE_FILTER = "json files (*.json)|*.json";

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

        public static string ReadGeoJsonFile()
        {
            if (OpenFile(GEOJSON_FILE_FILTER) != DialogResult.OK) return null;
            else return File.ReadAllText(_fileName);
        }

        public static void SaveNVDFile(List<NVCInfo> nvd)
        {
            if (SaveFile(XML_FILE_FILTER) != DialogResult.OK) return;

            StringWriter writer = new StringWriter(new StringBuilder());
            XmlSerializer serializer = new XmlSerializer(typeof(List<NVCInfo>));
            serializer.Serialize(writer, nvd);
            File.WriteAllText(_fileName, writer.ToString());
        }

        public static List<NVCInfo> ReadNVDFile()
        {
            if (OpenFile(XML_FILE_FILTER) != DialogResult.OK) return null;
            
            XmlSerializer serializer = new XmlSerializer(typeof(List<NVCInfo>));
            List<NVCInfo> nvdList = (List<NVCInfo>)serializer.Deserialize(File.OpenText(_fileName));

            return nvdList;
        }

        public static void SaveEBTable(TableInfo countingTable)
        {
            if (SaveFile(JSON_FILE_FILTER) != DialogResult.OK) return;

            string output = JsonConvert.SerializeObject(countingTable);
            File.WriteAllText(_fileName, output);
        }

        public static TableInfo ReadEBTableFile()
        {
            if (OpenFile(JSON_FILE_FILTER) != DialogResult.OK) return null;

            TableInfo table = JsonConvert.DeserializeObject<TableInfo>(File.ReadAllText(_fileName));
            return table;
        }
    }
}

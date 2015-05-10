using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using KNNonAir.Access;
using Newtonsoft.Json;

namespace KNNonAir.Domain.Service
{
    class FileIO
    {
        private const string GEOJSON_FILE_FILTER = "geojson files (*.geojson)|*.geojson";
        private const string XML_FILE_FILTER = "xml files (*.xml)|*.xml";
        private const string JSON_FILE_FILTER = "json files (*.json)|*.json";

        private static string _filepath;

        private static DialogResult OpenFile(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.RestoreDirectory = true;

            DialogResult result = openFileDialog.ShowDialog();
            _filepath = openFileDialog.FileName;

            return result;
        }

        private static DialogResult SaveFile(string filter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            saveFileDialog.RestoreDirectory = true;

            DialogResult result = saveFileDialog.ShowDialog();
            _filepath = saveFileDialog.FileName;

            return result;
        }

        public static string ReadGeoJsonFile()
        {
            if (OpenFile(GEOJSON_FILE_FILTER) != DialogResult.OK) return null;
            else return File.ReadAllText(_filepath);
        }

        public static void SaveNVDFile(List<NVCInfo> nvd)
        {
            if (SaveFile(XML_FILE_FILTER) != DialogResult.OK) return;

            StringWriter writer = new StringWriter(new StringBuilder());
            XmlSerializer serializer = new XmlSerializer(typeof(List<NVCInfo>));
            serializer.Serialize(writer, nvd);
            File.WriteAllText(_filepath, writer.ToString());
        }

        public static List<NVCInfo> ReadNVDFile(string filepath)
        {
            if (filepath == null)
            {
                if (OpenFile(XML_FILE_FILTER) != DialogResult.OK) return null;
            }
            else _filepath = filepath;

            XmlSerializer serializer = new XmlSerializer(typeof(List<NVCInfo>));
            List<NVCInfo> nvdList = (List<NVCInfo>)serializer.Deserialize(File.OpenText(_filepath));
            return nvdList;
        }

        public static void SaveEBTable(EBTableInfo countingTable)
        {
            if (SaveFile(JSON_FILE_FILTER) != DialogResult.OK) return;

            string output = JsonConvert.SerializeObject(countingTable);
            File.WriteAllText(_filepath, output);
        }

        public static EBTableInfo ReadEBTableFile(string filepath)
        {
            if (filepath == null)
            {
                if (OpenFile(JSON_FILE_FILTER) != DialogResult.OK) return null;
            }
            else _filepath = filepath;

            EBTableInfo table = JsonConvert.DeserializeObject<EBTableInfo>(File.ReadAllText(_filepath));
            return table;
        }
    }
}

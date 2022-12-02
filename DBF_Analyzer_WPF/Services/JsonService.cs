using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBF_Analyzer_WPF.Services
{
    //Конвертер файла JSON > C# класс, на выходе получаем List массив с классом Cell
    static class JsonReader
    {
        public static List<Cell> JsonRead(string path)
        {
            using (System.IO.StreamReader fileStream = new System.IO.StreamReader(path))
            {
                string file = fileStream.ReadToEnd();
                var controls = JsonConvert.DeserializeObject<List<Cell>>(file);
                return controls;
            };
        }
    }

    static class JsonSaver
    {
        public static void JsonSave(List<Cell> cells, string path)
        {
            using System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(path);
            var json = JsonConvert.SerializeObject(cells, Formatting.Indented);
            streamWriter.WriteLine(json);
        }
    }
}
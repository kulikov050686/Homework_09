using Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Services
{
    /// <summary>
    /// Класс загрузки и выгрузки данных из файла
    /// </summary>
    public static class FileIOService
    {
        /// <summary>
        /// Сохранить лист в файл формата JSON
        /// </summary>
        /// <param name="PathFile"> Путь к файлу </param>
        /// <param name="listSave"> Сохраняемый лист </param>
        public static void SaveAsJSON(string PathFile, ObservableCollection<MessageLog> listSave)
        {
            using (StreamWriter writer = File.CreateText(PathFile))
            {
                string output = JsonConvert.SerializeObject(listSave, Formatting.Indented);
                writer.Write(output);
            }
        }

        /// <summary>
        /// Загрузить данные в лист из файла формата JSON
        /// </summary>
        /// <param name="PathFile"> Путь к файлу </param>        
        public static ObservableCollection<MessageLog> OpenAsJSON(string PathFile)
        {
            var fileExists = File.Exists(PathFile);

            if (!fileExists)
            {
                File.CreateText(PathFile).Dispose();
                return new ObservableCollection<MessageLog>();
            }

            using (var reader = File.OpenText(PathFile))
            {
                var fileTaxt = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ObservableCollection<MessageLog>>(fileTaxt);
            }
        }

        /// <summary>
        /// Сохранить лист в файл формата XML
        /// </summary>
        /// <param name="PathFile"> Путь к файлу </param>
        /// <param name="listSave"> Сохраняемый лист </param>
        public static void SaveAsXML(string PathFile, ObservableCollection<MessageLog> listSave)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<MessageLog>));

            using (Stream fStream = new FileStream(PathFile, FileMode.Create, FileAccess.Write))
            {
                xmlSerializer.Serialize(fStream, listSave);
            }
        }

        /// <summary>
        /// Загрузить данные в лист из файла формата XML
        /// </summary>
        /// <param name="PathFile"> Путь к файлу </param>        
        public static ObservableCollection<MessageLog> OpenAsXML(string PathFile)
        {
            var fileExists = File.Exists(PathFile);

            if (!fileExists)
            {
                File.CreateText(PathFile).Dispose();
                return new ObservableCollection<MessageLog>();
            }

            ObservableCollection<MessageLog> Temp = new ObservableCollection<MessageLog>();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<MessageLog>));

            using (Stream fStream = new FileStream(PathFile, FileMode.Open, FileAccess.Read))
            {
                Temp = xmlSerializer.Deserialize(fStream) as ObservableCollection<MessageLog>;
            }

            return Temp;
        }

        /// <summary>
        /// Чтение данных из текстового файла
        /// </summary>
        /// <param name="PathFile"></param>        
        public static string OpenAsTXT(string PathFile)
        {
            var fileExists = File.Exists(PathFile);

            if (!fileExists)
            {
                return null;
            }

            string Temp;

            using (StreamReader sr = new StreamReader(PathFile, System.Text.Encoding.Default))
            {
                Temp = sr.ReadToEnd();
            }

            return Temp; ;
        }
    }
}

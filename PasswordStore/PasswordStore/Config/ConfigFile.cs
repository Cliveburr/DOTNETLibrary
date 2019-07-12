using System;
using System.IO;
using System.Xml.Serialization;

namespace PasswordStore.Config
{
    public static class ConfigFile
    {
        public static ConfigData Data { get; private set; }

        private static string _filePath;

        private static bool DetectFileLocation()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PasswordStore.config");
            if (File.Exists(_filePath))
            {
                return true;
            }

            _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PasswordStore", "PasswordStore.config");
            return File.Exists(_filePath);
        }

        public static void Load()
        {
            if (DetectFileLocation())
            {
                using (var stream = File.OpenRead(_filePath))
                {
                    var serializer = new XmlSerializer(typeof(ConfigData));
                    Data = serializer.Deserialize(stream) as ConfigData;
                }
            }
            else
            {
                Data = new ConfigData();
            }
            Data.InitializeData();
        }

        public static void Save()
        {
            var folder = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (var writer = new StreamWriter(_filePath))
            {
                var serializer = new XmlSerializer(typeof(ConfigData));
                serializer.Serialize(writer, Data);
                writer.Flush();
            }
        }
    }
}
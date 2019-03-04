using System;
using System.IO;
using System.Xml.Serialization;

namespace PasswordStore.Config
{
    public static class ConfigFile
    {
        public static ConfigData Data { get; private set; }

        private static string FilePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PasswordStore.config");
            }
        }


        public static void Load()
        {
            if (!File.Exists(FilePath))
            {
                Data = new ConfigData();
            }

            using (var stream = File.OpenRead(FilePath))
            {
                var serializer = new XmlSerializer(typeof(ConfigData));
                Data = serializer.Deserialize(stream) as ConfigData;
                Data.InitializeData();
            }
        }

        public static void Save()
        {
            using (var writer = new StreamWriter(FilePath))
            {
                var serializer = new XmlSerializer(typeof(ConfigData));
                serializer.Serialize(writer, Data);
                writer.Flush();
            }
        }
    }
}
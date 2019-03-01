using System;
using System.IO;
using System.Xml.Serialization;

namespace PasswordStore.Config
{
    public static class ConfigFile
    {
        private static string FilePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PasswordStore.config");
            }
        }


        public static ConfigData Load()
        {
            if (!File.Exists(FilePath))
            {
                return new ConfigData();
            }

            using (var stream = File.OpenRead(FilePath))
            {
                var serializer = new XmlSerializer(typeof(ConfigData));
                return serializer.Deserialize(stream) as ConfigData;
            }
        }

        public static void Save(ConfigData model)
        {
            using (var writer = new StreamWriter(FilePath))
            {
                var serializer = new XmlSerializer(typeof(ConfigData));
                serializer.Serialize(writer, model);
                writer.Flush();
            }
        }
    }
}
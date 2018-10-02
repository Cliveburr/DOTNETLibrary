using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RefineryBoard.Data
{
    [Serializable()]
    public class AppData : ISerializable, IDisposable
    {
        public const string NAME_OF_FILE = @"RefineryBorad.data";

        public ContentData Content { get; set; }

        public AppData()
        {
            Content = new ContentData();
            Content.CheckAndDefaultConfigs();
        }

        public AppData(SerializationInfo info, StreamingContext context)
        {
            Content = (ContentData)info.GetValue("ContentData", typeof(ContentData));
            Content.CheckAndDefaultConfigs();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ContentData", Content);
        }

        private static string GetStore()
        {
            //return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NAME_OF_FILE);
        }

        public static AppData Open()
        {
            var file = GetStore();

            if (!File.Exists(file))
                return new AppData();

            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binary = new BinaryFormatter();
                binary.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
                try
                {
                    return (AppData)binary.Deserialize(stream);
                }
                catch
                {
                    return new AppData();
                }
            }
        }

        public void Save()
        {
            var file = GetStore();

            using (var stream = File.Open(file, FileMode.OpenOrCreate, FileAccess.Write))
            {
                BinaryFormatter binary = new BinaryFormatter();
                binary.Serialize(stream, this);
            }
        }

        public void Dispose()
        {
            Save();
        }
    }

    sealed class AllowAllAssemblyVersionsDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            String currentAssembly = Assembly.GetExecutingAssembly().FullName;

            // In this case we are always using the current assembly
            assemblyName = currentAssembly;

            // Get the type using the typeName and assemblyName
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

            return typeToDeserialize;
        }
    }
}
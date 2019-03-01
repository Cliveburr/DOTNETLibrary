using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace PasswordStore.User
{
    [Serializable()]
    public class UserFile : ISerializable
    {
        public UserData Data;

        public UserFile()
        {
            Data = new UserData();
            Data.InitializeData();
        }

        public UserFile(UserData data)
        {
            Data = data;
        }

        public UserFile(SerializationInfo info, StreamingContext context)
        {
            Data = (UserData)info.GetValue("Data", typeof(UserData));
            Data.InitializeData();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", Data);
        }

        public void Save(string fileName, string password)
        {
            var path = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var ms = new MemoryStream();
            var me = new MemoryStream();
            FileStream stream = null;
            CryptoStream cs = null;
            byte[] buffer = null;

            try
            {
                var binary = new BinaryFormatter();
                binary.Serialize(ms, this);

                buffer = ms.GetBuffer();

                var pdb = new Rfc2898DeriveBytes(password, Salt());
                var alg = Rijndael.Create();
                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);

                cs = new CryptoStream(me, alg.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(buffer, 0, buffer.Length);

                File.WriteAllBytes(fileName, me.GetBuffer());
            }
            catch (Exception err)
            {
                Program.ErrorHandle(new StoreException("Error loading passwords: {0}{0}{1}", Environment.NewLine, err.Message));
            }
            finally
            {
                if (cs != null)
                    cs.Close();

                if (stream != null)
                    stream.Close();

                if (ms != null)
                    ms.Close();

                if (me != null)
                    me.Close();
            }
        }

        public static UserFile Open(string fileName, string password)
        {
            if (!File.Exists(fileName))
            {
                var userFile = new UserFile();
                userFile.Save(fileName, password);
                return userFile;
            }

            UserFile passwords = null;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = null;
            byte[] buffer = null;

            var binary = new BinaryFormatter();
            binary.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
            try
            {
                buffer = File.ReadAllBytes(fileName);

                var pdb2 = new Rfc2898DeriveBytes(password, Salt());
                var alg = Rijndael.Create();
                alg.Key = pdb2.GetBytes(32);
                alg.IV = pdb2.GetBytes(16);

                cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(buffer, 0, buffer.Length);

                ms.Seek(0, SeekOrigin.Begin);
                passwords = (UserFile)binary.Deserialize(ms);
            }
            catch
            {
                passwords = null;
            }
            finally
            {
                cs = null;
                ms = null;
            }
            return passwords;
        }

        private static byte[] Salt()
        {
            return new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76, 0x54};
        }
    }
}
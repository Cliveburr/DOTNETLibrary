using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordTextStore.Crypt
{
    [Serializable()]
    public class CryptFile : ISerializable
    {
        private static string FILE_NAME = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PasswordTextStore.data");

        public CryptData Data;

        public CryptFile()
        {
            Data = new CryptData();
            Data.InitializeData();
        }

        public CryptFile(SerializationInfo info, StreamingContext context)
        {
            Data = (CryptData)info.GetValue("Data", typeof(CryptData));
            Data.InitializeData();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Data", Data);
        }

        public void Save(string password)
        {
            string path = Path.GetDirectoryName(FILE_NAME);
            
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

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

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, Salt());
                Rijndael alg = Rijndael.Create();
                alg.Key = pdb.GetBytes(32);
                alg.IV = pdb.GetBytes(16);

                cs = new CryptoStream(me, alg.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(buffer, 0, buffer.Length);

                File.WriteAllBytes(FILE_NAME, me.GetBuffer());
            }
            catch (Exception err)
            {
                Program.ErrorHandle(new StoreException("Error loading data: {0}{0}{1}", Environment.NewLine, err.Message));
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

        public static CryptFile Open(string password)
        {
            if (!File.Exists(FILE_NAME))
                return new CryptFile();

            CryptFile data = null;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = null;
            byte[] buffer = null;

            BinaryFormatter binary = new BinaryFormatter();
            binary.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
            try
            {
                buffer = File.ReadAllBytes(FILE_NAME);

                Rfc2898DeriveBytes pdb2 = new Rfc2898DeriveBytes(password, Salt());
                Rijndael alg = Rijndael.Create();
                alg.Key = pdb2.GetBytes(32);
                alg.IV = pdb2.GetBytes(16);

                cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(buffer, 0, buffer.Length);

                ms.Seek(0, SeekOrigin.Begin);
                data = (CryptFile)binary.Deserialize(ms);
            }
            catch
            {
                data = null;
            }
            finally
            {
                cs = null;
                ms = null;
            }
            return data;
        }

        private static byte[] Salt()
        {
            return new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76, 0x54};
        }
    }
}
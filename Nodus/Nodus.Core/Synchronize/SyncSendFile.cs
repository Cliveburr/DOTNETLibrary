using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Synchronize
{
    public class SyncSendFile : IDisposable
    {
        public string File { get; private set; }
        public int BufferSize { get; private set; }
        public FileStream Stream { get; private set; }
        public int ChunksCount { get; private set; }

        public SyncSendFile(string file, int bufferSize = 1048576)
        {
            File = file;
            BufferSize = bufferSize;
            Stream = System.IO.File.Open(File, FileMode.Open, FileAccess.Read);
            ChunksCount = (int)(Stream.Length / BufferSize) + 1;
        }

        public FileChunk Read(int chunkPosition)
        {
            byte[] buffer = new byte[BufferSize];
            int iniChunk = chunkPosition * BufferSize;

            Stream.Position = iniChunk;
            int readed = Stream.Read(buffer, 0, BufferSize);

            return new FileChunk(chunkPosition, buffer, readed);
        }

        public byte[] CheckSum()
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(File))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }
        }
    }
}

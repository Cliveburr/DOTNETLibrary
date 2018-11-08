using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Synchronize
{
    public class SyncReceiveFile
    {
        public int Chunks { get; private set; }
        public string File { get; private set; }
        public int BufferSize { get; private set; }
        public FileStream Stream { get; private set; }
        public bool[] ChunksCheck { get; private set; }

        public SyncReceiveFile(string file)
        {
            File = file;
            BufferSize = 1048576;
        }

        public void Open(int chunksCount)
        {
            Open(chunksCount, 1048576);
        }

        public void Open(int chunksCount, int bufferSize)
        {
            ChunksCheck = new bool[chunksCount];
            BufferSize = bufferSize;
            Stream = System.IO.File.Open(File, FileMode.Create, FileAccess.Write);
        }

        public void Write(int chunkPosition, byte[] chunk, int length)
        {
            ChunksCheck[chunkPosition] = true;
            int iniTrunk = chunkPosition * BufferSize;
            Stream.Position = iniTrunk;
            Stream.Write(chunk, 0, length);
        }

        public void Close()
        {
            Stream.Close();
        }

        public int[] GetMissingChunks()
        {
            List<int> tr = new List<int>();
            for (int i = 0; i < ChunksCheck.Length; i++)
            {
                if (!ChunksCheck[i])
                    tr.Add(i);
            }
            return tr.ToArray();
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
    }
}
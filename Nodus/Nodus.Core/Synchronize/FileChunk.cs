using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Synchronize
{
    public class FileChunk
    {

        public int ChunkPosition { get; private set; }
        public byte[] Chunk { get; private set; }
        public int Length { get; private set; }

        public FileChunk(int chunkPosition, byte[] chunk, int length)
        {
            ChunkPosition = chunkPosition;
            Chunk = chunk;
            Length = length;
        }
    }
}
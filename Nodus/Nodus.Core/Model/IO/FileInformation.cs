using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Model
{
    [DataContract]
    public class FileInformation
    {
        [DataMember]
        public string Path { get; private set; }

        [DataMember]
        public DateTime CreationTime { get; private set; }

        [DataMember]
        public DateTime LastWriteTime { get; private set; }

        [DataMember]
        public long Length { get; private set; }

        [DataMember]
        public bool Exists { get; private set; }

        public FileInformation(string path)
        {
            var fileInfo = new FileInfo(path);

            Path = path;
            CreationTime = fileInfo.CreationTimeUtc;
            LastWriteTime = fileInfo.LastWriteTimeUtc;
            Length = fileInfo.Exists ? fileInfo.Length : 0;
            Exists = fileInfo.Exists;
        }

        public override bool Equals(object toc)
        {
            if (!(toc is FileInformation))
                return false;

            var tocr = (FileInformation)toc;
            return (
                CreationTime == tocr.CreationTime
                && LastWriteTime == tocr.LastWriteTime
                && Length == tocr.Length
                );
        }

        public static bool operator ==(FileInformation r1, FileInformation r2)
        {
            var r1n = (object)r1 == null;
            var r2n = (object)r2 == null;
            if (r1n || r2n)
                return r1n & r2n;
            else
                return r1.Equals(r2);
        }

        public static bool operator !=(FileInformation r1, FileInformation r2)
        {
            var r1n = (object)r1 == null;
            var r2n = (object)r2 == null;
            if (r1n || r2n)
                return !(r1n & r2n);
            else
                return !r1.Equals(r2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
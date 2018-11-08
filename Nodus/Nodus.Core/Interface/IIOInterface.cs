using Nodus.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Interface
{
    [ServiceContract()]
    public interface IIOInterface
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void RouteTo(string host, int port);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool CreateDirectory(string path);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool DeleteDirectory(string path, string searhPattern, bool recursive);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool EmptyDirectory(string path, string searhPattern, bool recursive);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool SynchronizeFile(int hoop, string sourceFile, string remoteFolder);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        FileInformation[] FileInfo(string sourceFile);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        string CreateFile(string file, int chunksCount, bool overwrite);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void Write(string token, int chunkPosition, byte[] chunk, int length);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        int[] GetMissingChunks(string token);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void Close(string token);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        byte[] CheckSum(string token);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void EqualizeFiles(FileInformation[] files, string sourceFolder, string targetFolder);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void ExtractFolder(string zipFile);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        int SynchronizeFolder(int hoop, string sourceFolder, string remoteFolder, bool recursive, string[] avoidPatterns, bool processExclude, string[] avoidExcludePatterns);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void DeleteFiles(FileInformation[] files);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        string SpecialVars(string path);
    }
}
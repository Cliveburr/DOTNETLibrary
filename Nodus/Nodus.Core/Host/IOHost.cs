using Nodus.Core.Helper;
using Nodus.Core.Interface;
using Nodus.Core.Model;
using Nodus.Core.Service;
using Nodus.Core.Synchronize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using IO = System.IO;

namespace Nodus.Core.Host
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    internal class IOHost : IIOInterface
    {
        private IOService _route = null;
        public SyncController Synchronize { get; private set; }

        public IOHost()
        {
            Synchronize = new SyncController();
            OperationContext.Current.InstanceContext.Closed += InstanceContext_Closed;
            OperationContext.Current.InstanceContext.Faulted += InstanceContext_Closed;
        }

        private void InstanceContext_Closed(object sender, EventArgs e)
        {
            if (_route != null)
            {
                _route.Close();
                _route = null;
            }
        }

        public void RouteTo(string host, int port)
        {
            if (_route == null)
            {
                IOService troute = null;
                try
                {
                    troute = new IOService(TcpDefaults.Binding(), new EndpointAddress($"net.tcp://{host}:{port}/Nodus/IO"));
                }
                catch (Exception err)
                {
                    throw new ServiceFault($@"Error routing from ""{Environment.MachineName}"" to ""{host}""!", err);
                }
                _route = troute;
            }
            else
                _route.RouteTo(host, port);
        }

        public bool CreateDirectory(string path)
        {
            if (_route == null)
                return Helper.IO.CreateDirectory(path);
            else
                return _route.CreateDirectory(path);
        }

        public bool DeleteDirectory(string path, string searhPattern, bool recursive)
        {
            if (_route == null)
                return Helper.IO.DeleteDirectory(path, searhPattern, recursive);
            else
                return _route.DeleteDirectory(path, searhPattern, recursive);
        }

        public bool EmptyDirectory(string path, string searhPattern, bool recursive)
        {
            if (_route == null)
                return Helper.IO.EmptyDirectory(path, searhPattern, recursive);
            else
                return _route.EmptyDirectory(path, searhPattern, recursive);
        }

        public FileInformation[] FileInfo(string path)
        {
            if (_route == null)
            {
                return Helper.IO.ListFileInfo(path);
            }
            else
                return _route.FileInfo(path);
        }

        public bool SynchronizeFile(int hoopCount, string sourceFile, string remoteFolder)
        {
            if (_route == null)
            {
                throw new ServiceFault("Invalid hoop with out route!");
            }
            else
                return _route.SynchronizeFile(hoopCount, sourceFile, remoteFolder);
        }

        public string CreateFile(string file, int chunksCount, bool overwrite)
        {
            if (_route == null)
            {
                if (IO.File.Exists(file))
                {
                    if (overwrite)
                        IO.File.Delete(file);
                    else
                        throw new ServiceFault("File already exists!");
                }

                string directory = IO.Path.GetDirectoryName(file);
                if (!IO.Directory.Exists(directory))
                    IO.Directory.CreateDirectory(directory);

                string token = Synchronize.GetFreeToken();
                var item = new SyncItem
                {
                    Token = token,
                    File = new SyncReceiveFile(file)
                };
                item.File.Open(chunksCount);
                Synchronize.Items.Add(item);

                return token;
            }
            else
                return _route.CreateFile(file, chunksCount, overwrite);
        }

        public void Write(string token, int chunkPosition, byte[] chunk, int length)
        {
            if (_route == null)
            {
                var item = Synchronize.GetItem(token);
                item.File.Write(chunkPosition, chunk, length);
            }
            else
                _route.Write(token, chunkPosition, chunk, length);
        }

        public int[] GetMissingChunks(string token)
        {
            if (_route == null)
            {
                var item = Synchronize.GetItem(token);
                return item.File.GetMissingChunks();
            }
            else
                return _route.GetMissingChunks(token);
        }

        public void Close(string token)
        {
            if (_route == null)
            {
                var item = Synchronize.GetItem(token);
                item.File.Close();
            }
            else
                _route.Close(token);
        }

        public byte[] CheckSum(string token)
        {
            if (_route == null)
            {
                var item = Synchronize.GetItem(token);
                using (var md5 = MD5.Create())
                {
                    using (var stream = IO.File.OpenRead(item.File.File))
                    {
                        return md5.ComputeHash(stream);
                    }
                }
            }
            else
                return _route.CheckSum(token);
        }

        public void EqualizeFiles(FileInformation[] files, string sourceFolder, string targetFolder)
        {
            if (_route == null)
            {
                if (!IO.Directory.Exists(targetFolder))
                    throw new ServiceFault("Directory not exists!");

                var targetfiles = IO.Directory.GetFiles(targetFolder, "*.*", IO.SearchOption.AllDirectories)
                    .Select(f => f.ToLower());

                var sourceFolderLenght = sourceFolder.Length;
                foreach (var file in files)
                {
                    var relativeSourceFolder = file.Path.Substring(sourceFolderLenght);
                    var relativeTargetFile = Helper.IO.PathCombine(targetFolder, relativeSourceFolder).ToLower();
                    var inTarget = targetfiles.FirstOrDefault(t => t == relativeTargetFile);
                    if (inTarget == null)
                        continue;

                    var targetInfo = new IO.FileInfo(inTarget);

                    var attrs = targetInfo.Attributes;
                    targetInfo.Attributes = IO.FileAttributes.Normal;
                    targetInfo.CreationTimeUtc = file.CreationTime;
                    targetInfo.LastWriteTimeUtc = file.LastWriteTime;
                    targetInfo.Attributes = attrs;
                }
            }
            else
                _route.EqualizeFiles(files, sourceFolder, targetFolder);
        }

        public void ExtractFolder(string zipFile)
        {
            if (_route == null)
            {
                if (!IO.File.Exists(zipFile))
                    throw new ServiceFault("File not exists!");

                string folder = IO.Path.GetDirectoryName(zipFile);

                SevenZip zip = new SevenZip(zipFile);
                zip.Test();
                zip.Extract(folder);

                IO.File.Delete(zipFile);
            }
            else
                _route.ExtractFolder(zipFile);
        }

        public int SynchronizeFolder(int hoop, string sourceFolder, string remoteFolder, bool recursive, string[] avoidPatterns, bool processExclude, string[] avoidExcludePatterns)
        {
            if (_route == null)
            {
                throw new ServiceFault("Invalid hoop with out route!");
            }
            else
                return _route.SynchronizeFolder(hoop, sourceFolder, remoteFolder, recursive, avoidPatterns, processExclude, avoidExcludePatterns);
        }

        public void DeleteFiles(FileInformation[] files)
        {
            if (_route == null)
            {
                Helper.IO.DeleteFiles(files.Select(f => f.Path).ToArray());
            }
            else
                _route.DeleteFiles(files);
        }

        public string SpecialVars(string path)
        {
            if (_route == null)
            {
                return Helper.IO.SpecialVars(path);
            }
            else
                return _route.SpecialVars(path);
        }
    }
}
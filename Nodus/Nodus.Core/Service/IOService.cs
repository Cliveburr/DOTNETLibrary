using Nodus.Core.Helper;
using Nodus.Core.Interface;
using Nodus.Core.Model;
using Nodus.Core.Synchronize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nodus.Core.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    internal class IOService : ClientBase<IIOInterface>, IIOInterface, IDisposable
    {
        private List<SynchronizeReport> _report = null;

        internal IOService(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
            _report = new List<SynchronizeReport>();
        }

        public void RouteTo(string host, int port)
        {
            Channel.RouteTo(host, port);
        }

        public bool CreateDirectory(string path)
        {
            return Channel.CreateDirectory(path);
        }

        public bool DeleteDirectory(string path, string searhPattern = "*.*", bool recursive = true)
        {
            return Channel.DeleteDirectory(path, searhPattern, recursive);
        }

        public bool EmptyDirectory(string path, string searhPattern = "*.*", bool recursive = true)
        {
            return Channel.EmptyDirectory(path, searhPattern, recursive);
        }

        public FileInformation[] FileInfo(string sourceFile)
        {
            return Channel.FileInfo(sourceFile);
        }

        public bool SynchronizeFile(int hoop, string sourceFile, string remoteFolder)
        {
            if (hoop < 0)
                throw new ServiceFault("Hoop invalid!");
            if (hoop == 0)
            {
                if (!System.IO.File.Exists(sourceFile))
                    throw new ServiceFault($@"Source file on server ""{Environment.MachineName}"" not exists! Path: ""{sourceFile}""");

                var remoteFile = IO.PathCombine(remoteFolder, System.IO.Path.GetFileName(sourceFile));

                var sourceInfo = new FileInformation(sourceFile);
                var remoteInfo = Channel.FileInfo(remoteFile).FirstOrDefault();
                var hasDiff = true;
                var report = new SynchronizeReport
                {
                    DateTime = DateTime.Now,
                    SourcePath = sourceFile,
                    TargetPath = remoteFile
                };
                _report.Add(report);

                if (remoteInfo?.Exists ?? false)
                {
                    hasDiff = sourceInfo != remoteInfo;
                    report.Status = hasDiff ? SynchornizeReportStatus.Updated : SynchornizeReportStatus.NotModified;
                }
                else
                {
                    report.Status = SynchornizeReportStatus.Created;
                }

                if (!hasDiff)
                    return false;

                using (var stream = new SyncSendFile(sourceFile))
                {
                    var token = Channel.CreateFile(remoteFile, stream.ChunksCount, true);

                    for (int i = 0; i < stream.ChunksCount; i++)
                    {
                        var chunk = stream.Read(i);
                        Channel.Write(token, chunk.ChunkPosition, chunk.Chunk, chunk.Length);
                    }

                    int tryReset = 0;
                    do
                    {
                        int[] miss = Channel.GetMissingChunks(token);
                        if (miss.Length == 0) break;

                        if (tryReset > 2)
                            throw new ServiceFault("Can't sent the file!");

                        for (int i = 0; i < miss.Length; i++)
                        {
                            int trunkPosition = miss[i];
                            var chunk = stream.Read(trunkPosition);
                            Channel.Write(token, chunk.ChunkPosition, chunk.Chunk, chunk.Length);
                        }

                        tryReset++;
                    } while (true);

                    Channel.Close(token);
                    stream.Dispose();

                    byte[] targetCheckSum = Channel.CheckSum(token);
                    byte[] sourceCheckSum = stream.CheckSum();

                    if (!targetCheckSum.SequenceEqual(sourceCheckSum))
                        throw new ServiceFault("CheckSum fail!");
                }

                Channel.EqualizeFiles(new FileInformation[] { sourceInfo }, System.IO.Path.GetDirectoryName(sourceFile), remoteFolder);

                return true;
            }
            else
                return Channel.SynchronizeFile(hoop - 1, sourceFile, remoteFolder);
        }

        public string CreateFile(string file, int chunksCount, bool overwrite)
        {
            return Channel.CreateFile(file, chunksCount, overwrite);
        }

        public void Write(string token, int chunkPosition, byte[] chunk, int length)
        {
            Channel.Write(token, chunkPosition, chunk, length);
        }

        public int[] GetMissingChunks(string token)
        {
            return Channel.GetMissingChunks(token);
        }

        public void Close(string token)
        {
            Channel.Close(token);
        }

        public byte[] CheckSum(string token)
        {
            return Channel.CheckSum(token);
        }

        public void EqualizeFiles(FileInformation[] files, string sourceFolder, string targetFolder)
        {
            Channel.EqualizeFiles(files, sourceFolder, targetFolder);
        }

        public int SynchronizeFolder(int hoop, string sourceFolder, string remoteFolder, bool recursive, string[] avoidPatterns, bool processExclude, string[] avoidExcludePatterns)
        {
            if (hoop < 0)
                throw new ServiceFault("Hoop invalid!");
            if (hoop == 0)
            {
                if (!System.IO.Directory.Exists(sourceFolder))
                    throw new ServiceFault("Source folder not exists! Path: {0}", sourceFolder);

                sourceFolder = IO.SpecialVars(sourceFolder).ToLower();
                remoteFolder = Channel.SpecialVars(remoteFolder).ToLower();

                var sourceFiles = System.IO.Directory.GetFiles(sourceFolder, "*", System.IO.SearchOption.AllDirectories)
                    .Select(f => new FileInformation(f));
                var remoteFiles = Channel.FileInfo(remoteFolder);

                var avoidPatternsRegex = avoidPatterns == null ?
                    new Regex[0] :
                    avoidPatterns.Select(a => new Regex(a));

                var sourcePathLenght = sourceFolder.Length;
                var diff = sourceFiles.Where(f =>
                {
                    foreach (var avoid in avoidPatternsRegex)
                    {
                        if (avoid.IsMatch(f.Path))
                            return false;
                    }

                    var relativeSourcePath = f.Path.Substring(sourcePathLenght);
                    var relativeTargetPath = IO.PathCombine(remoteFolder, relativeSourcePath);

                    var report = new SynchronizeReport
                    {
                        DateTime = DateTime.Now,
                        SourcePath = f.Path,
                        TargetPath = relativeTargetPath
                    };
                    _report.Add(report);

                    var inTarget = remoteFiles.FirstOrDefault(t => t.Path == relativeTargetPath);
                    if (inTarget != null)
                    {
                        var hasDiff = f != inTarget;
                        report.Status = hasDiff ? SynchornizeReportStatus.Updated : SynchornizeReportStatus.NotModified;
                        return hasDiff;
                    }
                    else
                    {
                        report.Status = SynchornizeReportStatus.Created;
                        return true;
                    }
                }).ToList();

                if (diff.Count() > 0)
                {
                    using (var tempFolder = Helper.Temp.TempController.GetTempFolder())
                    {
                        var sourceSyncZip = IO.PathCombine(tempFolder.Folder, "SyncZipFile.7z");
                        var zip = new SevenZip(sourceSyncZip);
                        zip.WorkingDirectory = tempFolder.Folder;
                        zip.AddFiles(diff.Select(d =>
                            d.Path.Substring(0, sourcePathLenght + 1) + "*" + d.Path.Substring(sourcePathLenght + 1)).ToArray());

                        var syncZipTarget = IO.PathCombine(remoteFolder, "SyncZipFile.7z");

                        using (var syncSendFile = new SyncSendFile(sourceSyncZip))
                        {
                            var token = Channel.CreateFile(syncZipTarget, syncSendFile.ChunksCount, true);

                            for (int i = 0; i < syncSendFile.ChunksCount; i++)
                            {
                                var chunk = syncSendFile.Read(i);
                                Channel.Write(token, chunk.ChunkPosition, chunk.Chunk, chunk.Length);
                            }

                            int tryReset = 0;
                            do
                            {
                                int[] miss = Channel.GetMissingChunks(token);
                                if (miss.Length == 0) break;

                                if (tryReset > 2)
                                    throw new ServiceFault("Can't sent the file!");

                                for (int i = 0; i < miss.Length; i++)
                                {
                                    int trunkPosition = miss[i];
                                    var chunk = syncSendFile.Read(trunkPosition);
                                    Channel.Write(token, chunk.ChunkPosition, chunk.Chunk, chunk.Length);
                                }

                                tryReset++;
                            } while (true);

                            Channel.Close(token);
                            syncSendFile.Dispose();

                            byte[] targetCheckSum = Channel.CheckSum(token);
                            byte[] sourceCheckSum = syncSendFile.CheckSum();

                            if (!targetCheckSum.SequenceEqual(sourceCheckSum))
                                throw new ServiceFault("CheckSum fail!");
                        }

                        Channel.ExtractFolder(syncZipTarget);
                        Channel.EqualizeFiles(diff.ToArray(), sourceFolder, remoteFolder);
                    }
                }

                if (!processExclude)
                    return diff.Count();

                var avoidExcludePatternsRegex = avoidExcludePatterns == null ?
                    new Regex[0] :
                    avoidExcludePatterns.Select(a => new Regex(a));

                var remotePathLenght = remoteFolder.Length;
                var diffExclude = remoteFiles.Where(f =>
                {
                    foreach (var avoid in avoidExcludePatternsRegex)
                    {
                        if (avoid.IsMatch(f.Path))
                            return false;
                    }

                    var relativeRemotePath = f.Path.Substring(remotePathLenght);
                    var inSource = sourceFiles.Where(t => t.Path.EndsWith(relativeRemotePath)).FirstOrDefault();
                    if (inSource != null)
                        return false;
                    else
                    {
                        var report = new SynchronizeReport
                        {
                            DateTime = DateTime.Now,
                            SourcePath = IO.PathCombine(sourceFolder, relativeRemotePath.Trim('\\')),
                            TargetPath = f.Path,
                            Status = SynchornizeReportStatus.Deleted
                        };
                        _report.Add(report);

                        return true;
                    }
                }).ToArray();

                if (diffExclude.Length > 0)
                    Channel.DeleteFiles(diffExclude);

                return diff.Count() + diffExclude.Length;
            }
            else
                return Channel.SynchronizeFolder(hoop - 1, sourceFolder, remoteFolder, recursive, avoidPatterns, processExclude, avoidExcludePatterns);
        }

        public void ExtractFolder(string zipFile)
        {
            Channel.ExtractFolder(zipFile);
        }

        public void DeleteFiles(FileInformation[] files)
        {
            Channel.DeleteFiles(files);
        }

        public string SpecialVars(string path)
        {
            return Channel.SpecialVars(path);
        }
    }
}
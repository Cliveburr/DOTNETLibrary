using Nodus.Core.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Client
{
    public partial class NodusClient
    {
        public bool SynchronizeFile(string sourceFile, string remoteFolder)
        {
            return _IO.SynchronizeFile(0, sourceFile, remoteFolder);
        }

        public bool SynchronizeFile(Hoop sourceHoop, string sourceFile, string remoteFolder)
        {
            return _IO.SynchronizeFile(sourceHoop.Index + 1, sourceFile, remoteFolder);
        }

        public int SynchronizeFolder(string sourceFolder, string remoteFolder, bool recursive = true, string[] avoidPatterns = null, bool processExclude = true, string[] avoidExcludePatterns = null)
        {
            return _IO.SynchronizeFolder(0, sourceFolder, remoteFolder, recursive, avoidPatterns, processExclude, avoidExcludePatterns);
        }

        public int SynchronizeFolder(Hoop hoop, string sourceFolder, string remoteFolder, bool recursive = true, string[] avoidPatterns = null, bool processExclude = true, string[] avoidExcludePatterns = null)
        {
            return _IO.SynchronizeFolder(hoop.Index + 1, sourceFolder, remoteFolder, recursive, avoidPatterns, processExclude, avoidExcludePatterns);
        }
    }
}
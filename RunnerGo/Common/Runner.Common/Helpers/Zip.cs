﻿using System.IO.Compression;

namespace Runner.Common.Helpers
{
    public static class Zip
    {
        public static void Descompat(byte[] content, string path)
        {
            using (var memoryStream = new MemoryStream(content))
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName.EndsWith('/') || entry.FullName.EndsWith('\\'))
                    {
                        continue;
                    }

                    var fileFullPath = Path.Combine(path, entry.FullName);
                    var fileDirectory = Path.GetDirectoryName(fileFullPath)!;

                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    entry.ExtractToFile(fileFullPath);
                }
            }
        }
    }
}

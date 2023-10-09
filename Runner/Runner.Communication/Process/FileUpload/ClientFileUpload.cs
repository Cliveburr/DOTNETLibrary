using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communication.Model;
using Runner.Communication.Process.FileUpload.Model;
using Runner.Communication.Process.Services;

namespace Runner.Communication.FileUpload
{
    public class ClientFileUpload
    {
        public string LocalFilePath { get; private set; }
        public string DestineFilePath { get; private set; }
        public bool Overwrite { get; private set; }
        public int PartSizeMB { get; private set; }

        private SocketCon _socket;

        public ClientFileUpload(SocketCon socket, string localFilePath, string destineFilePath, bool overwrite = false, int partSizeMB = 10)
        {
            _socket = socket;
            LocalFilePath = localFilePath;
            DestineFilePath = destineFilePath;
            Overwrite = overwrite;
            PartSizeMB = partSizeMB;
        }

        private async Task<byte[]> SendMessage(byte[] data)
        {
            var requestMessage = Message.Create(MessageType.FileUpload, data);
            var responseMessage = await _socket.SendRequest(requestMessage);
            if (!(responseMessage.Head.Type == MessageType.Any ||
                responseMessage.Head.Type == MessageType.FileUpload))
            {
                throw new Exception("Invalid response MessageType FileUpload!");
            }
            var responseModel = ResponseModel.Parse(responseMessage.Data);
            if (responseModel.IsSuccess && responseModel.Result != null)
            {
                return responseModel.Result;
            }
            else
            {
                var err = responseModel.Result != null ?
                    Encoding.UTF8.GetString(responseModel.Result) :
                    "Result empty!";
                throw new Exception("FileUpload - " + err);
            }
        }

        private async Task<InitiateUploadResponse> Initiate(InitiateUploadRequest request)
        {
            var data = await SendMessage(request.GetBytes());
            return InitiateUploadResponse.Parse(data);
        }

        private async Task UploadPart(UploadPartRequest request)
        {
            _ = await SendMessage(request.GetBytes());
        }

        private async Task Complete(CompleteUploadRequest request)
        {
            _ = await SendMessage(request.GetBytes());
        }

        private async Task Cancel(CancelUploadRequest request)
        {
            _ = await SendMessage(request.GetBytes());
        }

        public async Task Upload(int retries = 3, Action<string>? log = null)
        {
            var fileInfo = new FileInfo(LocalFilePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(LocalFilePath);
            }

            log?.Invoke("InitiateUpload");
            var initiateUploadResponse = await Initiate(new InitiateUploadRequest
            {
                DestineFilePath = DestineFilePath,
                Overwrite = Overwrite,
                FileLength = fileInfo.Length
            });
            var fileId = initiateUploadResponse.FileId;
            log?.Invoke("FileId: " + fileId);

            try
            {
                var splitedSizeMaxParts = (int)Math.Ceiling((decimal)(fileInfo.Length / 10000));
                var megaPartSize = PartSizeMB * (long)Math.Pow(2, 20);
                var partSize = Math.Max(splitedSizeMaxParts, megaPartSize);

                var parts = new List<UploadPartRequest>();
                long missingLenght = fileInfo.Length;

                long filePosition = 0;
                while (missingLenght > 0)
                {
                    var thisPartSize = Math.Min(missingLenght, partSize);

                    parts.Add(new UploadPartRequest
                    {
                        FileId = fileId,
                        PartSize = thisPartSize,
                        FilePosition = filePosition
                    });

                    filePosition += thisPartSize;
                    missingLenght -= thisPartSize;
                }

                using (var readStream = File.OpenRead(LocalFilePath))
                {
                    foreach (var part in parts)
                    {
                        part.Data = new byte[part.PartSize];
                        readStream.Position = part.FilePosition;
                        var dataRead = readStream.Read(part.Data, 0, part.Data.Length);
                        if (dataRead != part.Data.Length)
                        {
                            throw new Exception($"Error reading file position: {part.FilePosition}, length: {part.PartSize}, file: \"{LocalFilePath}\"");
                        }
                        using (var md5 = System.Security.Cryptography.MD5.Create())
                        {
                            var hashBytes = md5.ComputeHash(part.Data);
                            part.Checksum = Convert.ToHexString(hashBytes);
                        }

                        var retry = 0;
                        do
                        {
                            try
                            {
                                log?.Invoke($"UploadPart - Position: {part.FilePosition}, Length: {part.PartSize}");
                                await UploadPart(part);
                                break;
                            }
                            catch
                            {
                                if (++retry == retries)
                                {
                                    throw;
                                }
                                await Task.Delay(1000);
                            }
                        } while (true);

                        part.Data = null;
                    }
                }

                using (var readStream = File.OpenRead(LocalFilePath))
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    var hashBytes = md5.ComputeHash(readStream);
                    var checksum = Convert.ToHexString(hashBytes);

                    log?.Invoke("CompleteUpload");
                    await Complete(new CompleteUploadRequest
                    {
                        FileId = fileId,
                        Checksum = checksum
                    });
                }
            }
            catch
            {
                await Cancel(new CancelUploadRequest
                {
                    FileId = fileId
                });
                throw;
            }
        }

        public async Task DeleteFolder(DeleteFolderRequest request)
        {
            _ = await SendMessage(request.GetBytes());
        }
    }
}

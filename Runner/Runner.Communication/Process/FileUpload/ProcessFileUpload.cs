using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communication.Helpers;
using Runner.Communication.Model;
using Runner.Communication.Process.FileUpload.Model;
using Runner.Communication.Process.Services;

namespace Runner.Communication.Process.FileUpload
{
    public class ProcessFileUpload
    {
        private Server _server;
        private Dictionary<string, FileUploadControl> _controls;

        public ProcessFileUpload(Server server)
        {
            _server = server;
            _controls = new Dictionary<string, FileUploadControl>();
        }

        public Task<Message> ProcessRequest(Message message)
        {
            try
            {
                var reader = new BytesReader(message.Data);

                var messageType = reader.ReadUInt16Enum<FileUploadMessageType>();
                switch (messageType)
                {
                    case FileUploadMessageType.Initiate:
                        {
                            var request = InitiateUploadRequest.Parse(reader);
                            var responseData = InitiateUpload(request);
                            var response = new ResponseModel
                            {
                                IsSuccess = true,
                                Result = responseData.GetBytes()
                            };
                            return Task.FromResult(Message.Create(MessageType.FileUpload, response.GetBytes()));
                        }
                    case FileUploadMessageType.UploadPart:
                        {
                            var request = UploadPartRequest.Parse(reader);
                            UploadPart(request);
                            var response = new ResponseModel
                            {
                                IsSuccess = true,
                                Result = new byte[0]
                            };
                            return Task.FromResult(Message.Create(MessageType.FileUpload, response.GetBytes()));
                        }
                    case FileUploadMessageType.Complete:
                        {
                            var request = CompleteUploadRequest.Parse(reader);
                            CompleteUpload(request);
                            var response = new ResponseModel
                            {
                                IsSuccess = true,
                                Result = new byte[0]
                            };
                            return Task.FromResult(Message.Create(MessageType.FileUpload, response.GetBytes()));
                        }
                    case FileUploadMessageType.Cancel:
                        {
                            var request = CancelUploadRequest.Parse(reader);
                            CancelUpload(request);
                            var response = new ResponseModel
                            {
                                IsSuccess = true,
                                Result = new byte[0]
                            };
                            return Task.FromResult(Message.Create(MessageType.FileUpload, response.GetBytes()));
                        }
                    case FileUploadMessageType.DeleteFolder:
                        {
                            var request = DeleteFolderRequest.Parse(reader);
                            DeleteFolder(request);
                            var response = new ResponseModel
                            {
                                IsSuccess = true,
                                Result = new byte[0]
                            };
                            return Task.FromResult(Message.Create(MessageType.FileUpload, response.GetBytes()));
                        }
                    default:
                        throw new Exception($"Invalid FileUploadMessageType: {messageType}");
                }
            }
            catch (Exception err)
            {
                var response = new ResponseModel
                {
                    IsSuccess = false,
                    Result = Encoding.UTF8.GetBytes(err.ToString())
                };

                return Task.FromResult(Message.Create(MessageType.FileUpload, response.GetBytes()));
            }
        }


        private void ReleaseTimeouts()
        {
            var timeout = DateTime.Now.AddMinutes(-3);
            lock (_controls)
            {
                var controlsTimeouts = _controls
                    .Where(c => c.Value.LastIteration < timeout)
                    .Select(c => c.Value)
                    .ToList();

                foreach (var control in controlsTimeouts)
                {
                    Release(control);
                }
            }
        }

        private InitiateUploadResponse InitiateUpload(InitiateUploadRequest req)
        {
            ReleaseTimeouts();

            if (string.IsNullOrEmpty(_server.FileUploadDirectory))
            {
                throw new Exception("FileUploadDirectory invalid!");
            }

            var destineFilePath = Path.Combine(_server.FileUploadDirectory, req.DestineFilePath);
            var destineFilePathUpload = destineFilePath + ".upload";

            if (File.Exists(destineFilePath) && !req.Overwrite)
            {
                throw new Exception("File already exists!");
            }

            if (_controls.Any(c => c.Value.DestineFilePath == destineFilePath))
            {
                throw new Exception("Already upload for this file!");
            }

            if (File.Exists(destineFilePathUpload))
            {
                File.Delete(destineFilePathUpload);
            }

            var directory = Path.GetDirectoryName(destineFilePathUpload);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var writeStream = File.OpenWrite(destineFilePathUpload);
            writeStream.SetLength(req.FileLength);

            var fileId = Guid.NewGuid().ToString();
            lock (_controls)
            {
                while (_controls.ContainsKey(fileId))
                {
                    fileId = Guid.NewGuid().ToString();
                }
                _controls.TryAdd(fileId, new FileUploadControl
                {
                    FileId = fileId,
                    DestineFilePath = destineFilePath,
                    DestineFilePathUpload = destineFilePathUpload,
                    LastIteration = DateTime.Now,
                    Stream = writeStream,
                });
            }

            return new InitiateUploadResponse
            {
                FileId = fileId
            };
        }

        private void UploadPart(UploadPartRequest req)
        {
            ReleaseTimeouts();

            if (req.Data == null)
            {
                throw new Exception("Invalid upload data part!");
            }

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hashBytes = md5.ComputeHash(req.Data);
                var dataChecksum = Convert.ToHexString(hashBytes);
                if (req.Checksum != dataChecksum)
                {
                    throw new Exception("Checksum fail!");
                }
            }

            if (!_controls.ContainsKey(req.FileId))
            {
                throw new Exception("FileId invalid!");
            }
            var control = _controls[req.FileId];

            lock (control)
            {
                control.LastIteration = DateTime.Now;
                control.Stream.Position = req.FilePosition;
                control.Stream.Write(req.Data, 0, req.Data.Length);
            }
        }

        private void CompleteUpload(CompleteUploadRequest req)
        {
            ReleaseTimeouts();

            if (!_controls.ContainsKey(req.FileId))
            {
                throw new Exception("FileId invalid!");
            }
            var control = _controls[req.FileId];

            lock (control)
            {
                control.Stream.Close();

                using (var readStream = File.OpenRead(control.DestineFilePathUpload))
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    var hashBytes = md5.ComputeHash(readStream);
                    var dataChecksum = Convert.ToHexString(hashBytes);
                    if (req.Checksum != dataChecksum)
                    {
                        throw new Exception("Checksum fail!");
                    }
                }

                File.Move(control.DestineFilePathUpload, control.DestineFilePath, true);

                _controls.Remove(req.FileId);
            }
        }

        private void CancelUpload(CancelUploadRequest req)
        {
            ReleaseTimeouts();

            if (!_controls.ContainsKey(req.FileId))
            {
                throw new Exception("FileId invalid!");
            }
            var control = _controls[req.FileId];

            Release(control);
        }

        private void Release(FileUploadControl control)
        {
            lock (control)
            {
                if (!_controls.ContainsKey(control.FileId))
                {
                    return;
                }

                control.Stream.Close();
                File.Delete(control.DestineFilePathUpload);
                _controls.Remove(control.FileId);
            }
        }

        private void DeleteFolder(DeleteFolderRequest req)
        {
            if (string.IsNullOrEmpty(_server.FileUploadDirectory))
            {
                throw new Exception("FileUploadDirectory invalid!");
            }

            var fullFolder = Path.Combine(_server.FileUploadDirectory, req.Folder);

            if (!Directory.Exists(fullFolder))
            {
                return;
            }

            int count = 0;
            while (Directory.Exists(fullFolder) && count++ < 3)
            {
                try
                {
                    Directory.Delete(fullFolder, true);
                }
                catch
                {
                    Thread.Sleep(0);
                    try
                    {
                        Directory.Delete(fullFolder, true);
                    }
                    catch
                    {
                    }
                }
            }

            Thread.Sleep(0);
            if (Directory.Exists(fullFolder))
            {
                throw new Exception("Can't delete directory: " + fullFolder);
            }
        }
    }
}

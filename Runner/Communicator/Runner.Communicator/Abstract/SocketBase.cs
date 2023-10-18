using Runner.Communicator.Helpers;
using Runner.Communicator.Model;
using Runner.Communicator.Process.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Runner.Communicator.Abstract
{
    public abstract class SocketBase : IDisposable
    {
        public delegate void OnErrorDelegate(object sender, Exception err);
        public event OnErrorDelegate? OnError;

        public int Timeout { get; set; }
        public CancellationToken CancellationToken { get => _cancellationTokenSource.Token; }

        protected CancellationTokenSource _cancellationTokenSource;
        protected abstract bool IsConnected();
        protected abstract Task DoConnectAsync(CancellationToken cancellationToken);
        protected abstract Task DoSendAsync(CancellationToken cancellationToken, byte[] data);
        protected abstract Task<byte[]> DoReadAsync(CancellationToken cancellationToken, uint length);
        protected abstract Task<byte[]?> DoProcessRequest(byte[] data, MessagePort port);

        private ushort _id;
        private object _lockId = new object();
        private MessageQueueProcess<MessageStore> _messageToSend;
        private MessageQueueProcess<Message> _messageToProccess;
        private List<MessageStore> _waitingToRespond;
        private object _lockConnect = new object();
        private ManualResetEvent? _waitConnect;

        protected SocketBase(int timeout, CancellationToken cancellationToken)
        {
            Timeout = timeout;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _id = 1;
            _messageToSend = new MessageQueueProcess<MessageStore>(InnerSendMessage);
            _messageToProccess = new MessageQueueProcess<Message>(InnerProcessMessage);
            _waitingToRespond = new List<MessageStore>();
        }

        public virtual void Dispose()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private ushort GetNextId()
        {
            lock(_lockId)
            {
                if (_id == ushort.MaxValue)
                {
                    _id = 1;
                    return ushort.MaxValue;
                }
                else
                {
                    return _id++;
                }
            }
        }

        public async Task ConnectAsync()
        {
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            await DoConnectAsync(timeoutCancellation.Token);
            StartReceive();
            lock (_lockConnect)
            {
                _waitConnect?.Set();
                _waitConnect = null;
            }
        }

        private Task<bool> CheckConnectedAsync()
        {
            if (_waitConnect == null)
            {
                if (IsConnected())
                {
                    return Task.FromResult(true);
                }
                else
                {
                    lock (_lockConnect)
                    {
                        if (_waitConnect == null)
                        {
                            _waitConnect = new ManualResetEvent(false);
                            Task.Run(ConnectAsync);
                        }
                    }
                }
            }
            return Task.Run(() =>
            {
                _waitConnect?.WaitOne();
                return IsConnected();
            });
        }

        public void StartReceive()
        {
            Task.Run(StartReceiveAsync);
        }

        private async Task StartReceiveAsync()
        {
            while (IsConnected())
            {
                try
                {
                    var message = await InnerReceiveMessage();
                    _messageToProccess.Enqueue(message);
                }
                catch (Exception err)
                {
                    _ = Task.Run(() => OnError?.Invoke(this, err));
                }
            }
        }

        private async Task InnerSendMessage(MessageStore messageStore)
        {
            if (!await CheckConnectedAsync())
            {
                messageStore.Error(new Exception("Not connected!"));
                return;
            }
            var data = messageStore.Message.Head
                .GetBytes()
                .Concat(messageStore.Message.Data)
                .ToArray();
            try
            {
                await DoSendAsync(messageStore.CancellationToken, data);
            }
            catch (Exception err)
            {
                messageStore.Error(err);
                return;
            }
            if (!messageStore.Message.Head.IsResponse)
            {
                if (messageStore.WaitReponse)
                {
                    _waitingToRespond.Add(messageStore);
                    messageStore.SetTimeout(MessageStore_OnTimeout);
                }
                else
                {
                    messageStore.Release();
                }
            }
        }

        private void MessageStore_OnTimeout(MessageStore messageStore, Exception? err)
        {
            var found = _waitingToRespond
                    .FirstOrDefault(fr => fr.Message.Head.Id == messageStore.Message.Head.Id);
            if (found != null)
            {
                _waitingToRespond.Remove(found);
                if (err != null)
                {
                    var data = Encoding.UTF8.GetBytes(err.ToString());
                    found.Release(data);
                }
                else
                {
                    found.Release();
                }
            }
        }

        private async Task InnerProcessMessage(Message message)
        {
            if (message.Head.IsResponse)
            {
                var found = _waitingToRespond
                    .FirstOrDefault(fr => fr.Message.Head.Id ==  message.Head.Id);
                if (found != null)
                {
                    _waitingToRespond.Remove(found);
                    found.Release(message.Data);
                }
            }
            else
            {
                try
                {
                    var data = await DoProcessRequest(message.Data, message.Head.Port);
                    if (data != null)
                    {
                        var response = new Message(data,
                            message.Head.Id,
                            message.Head.Port,
                            true,
                            true);
                        var responseStore = new MessageStore(response, _cancellationTokenSource);
                        _messageToSend.Enqueue(responseStore);
                    }
                }
                catch (TargetInvocationException err)
                {
                    var data = Encoding.UTF8.GetBytes(err.InnerException!.ToString());
                    var response = new Message(data,
                            message.Head.Id,
                            message.Head.Port,
                            true,
                            false);
                    var responseStore = new MessageStore(response, _cancellationTokenSource);
                    _messageToSend.Enqueue(responseStore);
                }
                catch (Exception err)
                {
                    var data = Encoding.UTF8.GetBytes(err.ToString());
                    var response = new Message(data,
                            message.Head.Id,
                            message.Head.Port,
                            true,
                            false);
                    var responseStore = new MessageStore(response, _cancellationTokenSource);
                    _messageToSend.Enqueue(responseStore);
                }
            }
        }

        private async Task<Message> InnerReceiveMessage()
        {
            if (!await CheckConnectedAsync())
            {
                new Exception("Not connected!");
            }
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            var headData = await DoReadAsync(timeoutCancellation.Token, (uint)MessageHead.HeadLenght);
            var head = MessageHead.Parse(headData);
            var data = await DoReadAsync(timeoutCancellation.Token, head.DataLenght);
            return new Message(head, data);
        }

        public Task Send(byte[] data, MessagePort port)
        {
            var message = new Message(data, GetNextId(), port, false, false);
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            var messageStore = new MessageStore(message, timeoutCancellation);
            _messageToSend.Enqueue(messageStore);
            return messageStore.WaitAsync();
        }

        public Task<byte[]> SendAndReceive(byte[] data, MessagePort port)
        {
            var message = new Message(data, GetNextId(), port, false, false);
            var timeoutCancellation = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
            if (Timeout > 0)
            {
                timeoutCancellation.CancelAfter(Timeout);
            }
            var messageStore = new MessageStore(message, timeoutCancellation);
            messageStore.WaitReponse = true;
            _messageToSend.Enqueue(messageStore);
            return messageStore.WaitDataAsync();
        }
    }
}

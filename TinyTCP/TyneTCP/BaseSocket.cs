using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TyneTCP.Message;

namespace TyneTCP
{
    public abstract class BaseSocket : IDisposable
    {
        public const int BufferSize = 1024;

        public System.Net.Sockets.Socket Socket { get; protected set; }

        protected abstract void OnReceived(byte[] bytes);
        protected abstract void OnSent(byte[] bytes);

        private List<MessageReceiver> _messages;

        public BaseSocket()
        {
            _messages = new List<MessageReceiver>();
        }

        public void Dispose()
        {
            Socket?.Dispose();
            Socket = null;
        }

        protected void BeginReceive()
        {
            try
            {
                var buffer = new byte[BufferSize];

                Socket.BeginReceive(buffer, 0, BufferSize, 0, new AsyncCallback(ReceiveCallback), buffer);
            }
            catch (Exception err)
            {
                Task.Run(() => HandleReceiveError(err));
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var buffer = (byte[])ar.AsyncState;
                var bytesRead = Socket.EndReceive(ar);

                var package = new Package(buffer, bytesRead);
                
                var receiver = _messages
                    .FirstOrDefault(m => m.Message.Id == package.MessageId);
                if (receiver == null)
                {
                    receiver = new MessageReceiver(package);
                }
                else
                {
                    receiver.AddPackage(package);
                }

                if (receiver.IsFinished())
                {
                    receiver.ComputatePayload();

                    _messages.Remove(receiver);

                    Task.Run(() => OnReceived(receiver.Message.Payload));
                }
            }
            catch (Exception err)
            {
                Task.Run(() => HandleReceiveError(err));
            }

            BeginReceive();
        }

        protected virtual void HandleReceiveError(Exception err)
        {
            Console.Write(err.ToString());
        }

        protected void BeginSend(byte[] bytes)
        {
            try
            {
                Socket.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(SendCallback), bytes);
            }
            catch (Exception err)
            {
                Task.Run(() => HandleSendError(err));
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var bytes = ar.AsyncState as byte[];

                var bytesSent = Socket.EndSend(ar);
                if (bytesSent != bytes.Length)
                {
                    throw new Exception("something wrong!");
                }

                OnSent(bytes);
            }
            catch (Exception err)
            {
                Task.Run(() => HandleSendError(err));
            }
        }

        protected virtual void HandleSendError(Exception err)
        {
            Console.Write(err.ToString());
        }
    }
}
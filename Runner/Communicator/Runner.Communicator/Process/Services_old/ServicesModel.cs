//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Runner.Communicator.Helpers;
//using Runner.Communicator.Model;

//namespace Runner.Communicator.Process.Services
//{
//    public class RequestModel
//    {
//        public string? InterfaceFullName { get; set; }
//        public string? Method { get; set; }
//        public byte[]?[]? Args { get; set; }

//        public static RequestModel Parse(byte[] buffer)
//        {
//            var reader = new BytesReader(buffer);
//            var interfaceFullName = reader.ReadString();
//            var method = reader.ReadString();
//            var args = reader.ReadUInt32DoubleBytes();

//            return new RequestModel
//            {
//                InterfaceFullName = interfaceFullName,
//                Method = method,
//                Args = args
//            };
//        }

//        public byte[] GetBytes()
//        {
//            return new BytesWriter()
//                .WriteString(InterfaceFullName)
//                .WriteString(Method)
//                .WriteUInt32DoubleBytes(Args)
//                .GetBytes();
//        }
//    }

//    public class ResponseModel
//    {
//        public bool IsSuccess { get; set; }
//        public byte[]? Result { get; set; }

//        public static ResponseModel Parse(byte[] buffer)
//        {
//            var reader = new BytesReader(buffer);
//            var isSuccess = reader.ReadBool();
//            var result = reader.ReadUInt32Bytes();

//            return new ResponseModel
//            {
//                IsSuccess = isSuccess,
//                Result = result
//            };
//        }

//        public byte[] GetBytes()
//        {
//            return new BytesWriter()
//                .WriteBool(IsSuccess)
//                .WriteUInt32Bytes(Result)
//                .GetBytes();
//        }
//    }
//}

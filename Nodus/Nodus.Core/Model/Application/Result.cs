using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Model.Application
{
    [Serializable]
    [DataContract]
    public class Result
    {
        [DataMember]
        public string Messager { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Model
{
    [DataContract]
    public class SynchronizeReport
    {
        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public string SourcePath { get; set; }

        [DataMember]
        public string TargetPath { get; set; }

        [DataMember]
        public SynchornizeReportStatus Status { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Model
{
    [DataContract]
    public enum SynchornizeReportStatus
    {
        [EnumMember]
        NotModified = 0,

        [EnumMember]
        Created = 1,

        [EnumMember]
        Updated = 2,

        [EnumMember]
        Deleted = 3
    }
}
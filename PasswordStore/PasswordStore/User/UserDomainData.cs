using System;
using System.Collections.Generic;

namespace PasswordStore.User
{
    [Serializable]
    public class UserDomainData
    {
        public uint DomainId { get; set; }
        public string Alias { get; set; }
        public string Info { get; set; }
        public string Group { get; set; }
        public uint PasswordId { get; set; }
        public List<UserDomainHistoryData> History { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace PasswordStore.User
{
    [Serializable()]
    public class UserData
    {
        public uint DomainsIndex { get; set; }
        public List<UserDomainData> Domains { get; set; }
        public List<string> GroupOrder { get; set; }

        public void InitializeData()
        {
            Domains = Domains ?? new List<UserDomainData>();
            GroupOrder = GroupOrder ?? new List<string>();
        }
    }
}
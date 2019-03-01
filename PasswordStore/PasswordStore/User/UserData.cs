using System;
using System.Collections.Generic;

namespace PasswordStore.User
{
    [Serializable()]
    public class UserData
    {
        public uint PasswordIndex { get; set; }
        public List<PasswordData> Passwords { get; set; }
        public uint DomainsIndex { get; set; }
        public List<UserDomainData> Domains { get; set; }

        public void InitializeData()
        {
            Passwords = Passwords ?? new List<PasswordData>();
            Domains = Domains ?? new List<UserDomainData>();
        }
    }
}
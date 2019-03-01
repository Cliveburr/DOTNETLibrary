using System;

namespace PasswordStore.User
{
    [Serializable]
    public class UserDomainHistoryData
    {
        public string Value { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
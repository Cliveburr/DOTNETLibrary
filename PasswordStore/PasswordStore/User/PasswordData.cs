using System;

namespace PasswordStore.User
{
    [Serializable()]
    public class PasswordData
    {
        public uint PasswordId { get; set; }
        public string Alias { get; set; }
        public string Value { get; set; }
    }
}
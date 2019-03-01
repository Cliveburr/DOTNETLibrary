using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordStore.User
{
    [Serializable()]
    public class PasswordData
    {
        public uint PasswordId { get; set; }
        public string Alias { get; set; }
        public string Value { get; private set; }

        //public PasswordData()
        //{
        //    this.PasswordsId = Program.Passwords.Data.GetPasswordsIndex();
        //    this.Associations = new List<uint>();
        //    this.History = new List<PasswordHistoryData>();
        //}

        //private PasswordData(uint passworsdId)
        //{
        //    this.PasswordsId = passworsdId;
        //}

        //public void SetValue(string value)
        //{
        //    this.History.Add(new PasswordHistoryData
        //    {
        //        Value = value,
        //        CreatedDateTime = DateTime.Now
        //    });

        //    this.Value = value;
        //}

        //public PasswordData Clone()
        //{
        //    return new PasswordData(this.PasswordsId)
        //    {
        //        Alias = this.Alias,
        //        Associations = (from a in this.Associations
        //                        select a).ToList(),
        //        History = (from h in this.History
        //                   select h.Clone()).ToList(),
        //        Value = this.Value
        //    };
        //}
    }
}
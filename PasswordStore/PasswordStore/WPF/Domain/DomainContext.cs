using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PasswordStore.WPF.Domain
{
    public class DomainContext : ContextBase
    {
        public ObservableCollection<DomainItemContext> Domains { get; set; }
    }

    public class DomainItemContext : ContextBase
    {
        public uint DomainId { get; set; }
        public string Alias { get; set; }
        public string Info { get; set; }
        public string Group { get; set; }
        public uint PasswordId { get; set; }
        public string PasswordAlias { get; set; }
        public string PasswordValue { get; set; }
        public List<DomainItemHistoryContext> History { get; set; } = new List<DomainItemHistoryContext>();
    }

    public class DomainItemHistoryContext : ContextBase
    {
        public string Value { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
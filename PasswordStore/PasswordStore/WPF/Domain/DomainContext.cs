using PasswordStore.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        public List<string> GroupList { get; set; }
        public ObservableCollection<DomainItemHistoryContext> History { get; set; }
        public string ActualPassword { get; set; }
        public string Login { get; set; }
        public string URL { get; set; }
        public string SubHotkey { get; set; }
    }

    public class DomainItemHistoryContext : ContextBase
    {
        public string Value { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }

    public static class DomainMapper
    {
        public static DomainItemContext FromData(UserDomainData data)
        {
            return new DomainItemContext
            {
                DomainId = data.DomainId,
                Alias = data.Alias,
                Group = data.Group,
                Info = data.Info,
                History = new ObservableCollection<DomainItemHistoryContext>(data.History
                    .Select(FromHistoryData)),
                ActualPassword = data.ActualPassword,
                Login = data.Login,
                URL = data.URL,
                SubHotkey = data.SubHotkey
            };
        }

        public static DomainItemHistoryContext FromHistoryData(UserDomainHistoryData data)
        {
            return new DomainItemHistoryContext
            {
                Value = data.Value,
                CreatedDateTime = data.CreatedDateTime
            };
        }

        public static UserDomainData FromContext(DomainItemContext context)
        {
            return new UserDomainData
            {
                DomainId = context.DomainId,
                Alias = context.Alias,
                Group = context.Group,
                Info = context.Info,
                History = (context.History ?? new ObservableCollection<DomainItemHistoryContext>())
                    .Select(FromHistoryContext)
                    .ToList(),
                ActualPassword = context.ActualPassword,
                Login = context.Login,
                URL = context.URL,
                SubHotkey = context.SubHotkey
            };
        }

        public static UserDomainHistoryData FromHistoryContext(DomainItemHistoryContext context)
        {
            return new UserDomainHistoryData
            {
                Value = context.Value,
                CreatedDateTime = context.CreatedDateTime
            };
        }
    }

    public class DomainSubhotkeyContext : ContextBase
    {
        public string SubHotKey { get; set; }
    }
}
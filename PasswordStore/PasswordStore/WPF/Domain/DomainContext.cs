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
        public uint PasswordId { get; set; }
        public string PasswordAlias { get; set; }
        public List<string> PasswordList { get; set; }
        public ObservableCollection<DomainItemHistoryContext> History { get; set; }
        public DomainItemPasswordType PasswordType { get; set; }
        public string UniquePasswordValue { get; set; }

        public int PasswordTypeInt
        {
            get
            {
                return (int)PasswordType;
            }
            set
            {
                PasswordType = (DomainItemPasswordType)value;
            }
        }
    }

    public enum DomainItemPasswordType : byte
    {
        Shared = 0,
        Unique = 1
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
                PasswordId = data.PasswordId,
                Info = data.Info,
                History = new ObservableCollection<DomainItemHistoryContext>(data.History
                    .Select(FromHistoryData)),
                PasswordType = data.IsUniquePassword ? DomainItemPasswordType.Unique : DomainItemPasswordType.Shared,
                UniquePasswordValue = data.UniquePasswordValue
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
                PasswordId = context.PasswordId,
                Info = context.Info,
                History = (context.History ?? new ObservableCollection<DomainItemHistoryContext>())
                    .Select(FromHistoryContext)
                    .ToList(),
                IsUniquePassword = context.PasswordType == DomainItemPasswordType.Unique,
                UniquePasswordValue = context.UniquePasswordValue
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
}
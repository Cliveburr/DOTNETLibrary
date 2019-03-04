using PasswordStore.User;
using System.Collections.ObjectModel;
using System.Linq;

namespace PasswordStore.WPF.Password
{
    public class PasswordContext : ContextBase
    {
        public ObservableCollection<PasswordItemContext> Passwords { get; set; }
    }

    public class PasswordItemContext : ContextBase
    {
        public uint PasswordId { get; set; }
        public string Alias { get; set; }
        public string Value { get; set; }
        public int UsedCount { get; set; }
    }

    public static class PasswordMapper
    {
        public static PasswordItemContext FromData(PasswordData data)
        {
            return new PasswordItemContext
            {
                PasswordId = data.PasswordId,
                Alias = data.Alias,
                Value = data.Value,
                UsedCount = Program.Session.User.Data.Domains
                    .Count(d => d.PasswordId == data.PasswordId)
            };
        }

        public static PasswordData FromContext(PasswordItemContext context)
        {
            return new PasswordData
            {
                PasswordId = context.PasswordId,
                Alias = context.Alias,
                Value = context.Value
            };
        }
    }
}
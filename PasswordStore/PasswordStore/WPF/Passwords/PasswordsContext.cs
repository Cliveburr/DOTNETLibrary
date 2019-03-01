using PasswordStore.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordStore.WPF.Passwords
{
    public class PasswordsContext : ContextBase
    {
        public ObservableCollection<PasswordsViewModel> Passwords { get; set; }
    }

    public class PasswordsViewModel : ContextBase
    {
        public PasswordData Data { get; private set; }

        public PasswordsViewModel(PasswordData data)
        {
            this.Data = data;
        }

        public PasswordsViewModel()
        {
            this.Data = new PasswordData();
        }

        public string Alias
        {
            get
            {
                return this.Data.Alias;
            }
            set
            {
                this.Data.Alias = value;
            }
        }

        private string _associations;
        public string Associations
        {
            get
            {
                return this._associations;
            }
        }

        public void SetAssociations(List<uint> associationsIds)
        {
            //this.Data.Associations = associationsIds;

            //var associationsNames = AssociationsData.ReadAssociations(associationsIds)
            //    .Select(a => a.Name);

            //this._associations = String.Join(", ", associationsNames);

            //this.RaiseNotify("Associations");
        }
    }
}

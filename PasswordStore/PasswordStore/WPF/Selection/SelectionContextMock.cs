using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordStore.WPF.Selection
{
    public class SelectionContextMock : SelectionContext
    {
        public SelectionContextMock()
        {
            Groups = new ObservableCollection<SelectionGroupContext>(new List<SelectionGroupContext>
            {
                new SelectionGroupContext
                {
                    Header = "grupo um",
                    Items = new ObservableCollection<SelectionDomainContext>(new List<SelectionDomainContext>
                    {
                        new SelectionDomainContext
                        {
                            DomainId = 1,
                            Alias = "teste um",
                            Password = "testeum",
                            Login = "LOGIN",
                            URL = "http://teste",
                            SubHotkey = "T"
                        }
                    })
                }
            });
        }
    }
}

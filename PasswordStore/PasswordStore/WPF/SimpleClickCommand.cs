using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PasswordStore.WPF
{
    public class SimpleClickCommand : ICommand
    {
        public delegate void SimpleClickHandle();
        public event EventHandler CanExecuteChanged;
        private SimpleClickHandle _click;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public SimpleClickCommand(SimpleClickHandle click)
        {
            this._click = click;
        }

        public void Execute(object parameter)
        {
            this._click();
        }
    }
}
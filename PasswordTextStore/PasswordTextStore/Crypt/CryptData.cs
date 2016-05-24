using PasswordTextStore.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordTextStore.Crypt
{
    [Serializable()]
    public class CryptData
    {
        public string Text { get; set; }
        public bool IsCreated { get; set; }
        public List<WindowConfigData> Windows { get; set; }

        public void InitializeData()
        {
            Windows = Windows ?? new List<WindowConfigData>();
        }
    }
}
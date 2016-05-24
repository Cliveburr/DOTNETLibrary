using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordTextStore.WPF
{
    [Serializable()]
    public class WindowConfigData
    {
        public string IdWindow { get; set; }
        public System.Windows.WindowState WindowState { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
    }
}
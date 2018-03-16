using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSimulation
{
    public class Ticket
    {
        public int[] Numbers { get; set; }

        public override string ToString()
        {
            return $"{{ {string.Join(", ", Numbers)} }}";
        }
    }
}
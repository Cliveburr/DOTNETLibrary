using Runner.Business.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Data.Validator
{
    public class ValidationError
    {
        public required DataTypeProperty Type { get; set; }
        public required string Text { get; set; }
    }
}

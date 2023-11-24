using Runner.Business.Data.Types;
using Runner.Business.Data.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Data.Validator.Types
{
    public interface IDataValidator
    {
        ValidationError? ValidateValue(DataTypeProperty type, DataProperty? data);
    }
}

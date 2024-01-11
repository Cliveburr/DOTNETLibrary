using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Kernel.Mapper.TypeMappers
{
    public interface ITypeMapper
    {
        bool IsApplicable(Type fromType, Type toType);
        object? Map(Type fromType, object? from, Type toType);
    }
}

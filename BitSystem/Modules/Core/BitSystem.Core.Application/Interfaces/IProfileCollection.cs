using BitSystem.Core.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSystem.Core.Application.Interfaces;

public interface IProfileCollection
{
    Task Create(Profile profile);
}

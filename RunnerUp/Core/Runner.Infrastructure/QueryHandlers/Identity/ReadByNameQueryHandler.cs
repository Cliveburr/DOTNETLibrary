using Runner.Domain.Entities.Identity;
using Runner.Domain.Read.Identity;
using Runner.Kernel.Events.Read;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Infrastructure.QueryHandlers.Identity
{
    internal class ReadByNameQueryHandler
        : IReadHandler<ReadByName, User?>
    {
        public User? Handler(ReadByName request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

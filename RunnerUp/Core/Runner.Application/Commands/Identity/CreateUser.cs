using Runner.Kernel.Events.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Application.Commands.Identity
{
    public record CreateUser(string? Name, string? FullName, string? Email, string? Password, string? ConfirmPassword) : ICommand;
}

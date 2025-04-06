using BitSystem.Core.Application.Interfaces;
using BitSystem.Core.Application.Services.Models.Identiy;
using BitSystem.Core.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSystem.Core.Application.Services;

public class IdentityService
{
    private readonly IProfileCollection _profileCollection;

    public IdentityService(IProfileCollection profileCollection)
    {
        _profileCollection = profileCollection;
    }

    public Task Register(RegisterProfile registerProfile)
    {
        var newProfile = new Profile
        {
            Email
        }
    }
}

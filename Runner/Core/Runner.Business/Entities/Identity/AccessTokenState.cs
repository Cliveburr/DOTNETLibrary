﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Identity
{
    public enum AccessTokenState : byte
    {
        Active = 0,
        Inactive = 1,
    }
}
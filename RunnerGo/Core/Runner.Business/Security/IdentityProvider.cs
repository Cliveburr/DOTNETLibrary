﻿using Runner.Business.Entities.Identity;

namespace Runner.Business.Security
{
    public class IdentityProvider
    {
        public User? User { get; private set; }
        public bool IsLogged { get => User != null; }

        internal void Clear()
        {
            User = null;
        }

        internal void Set(User user)
        {
            User = user;
        }
    }
}
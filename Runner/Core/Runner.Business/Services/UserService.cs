using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Services
{
    public class UserService : ServiceBase
    {
        public UserService(Database database)
            : base(database)
        {
        }

        public Task<User?> ReadByEmail(string email)
        {
            return User
                .FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
        }

        public Task<User?> ReadByName(string name)
        {
            return User
                .FirstOrDefaultAsync(u => u.Name.ToLower().Equals(name.ToLower()));
        }

        public Task<User?> ReadByIdAsync(ObjectId id)
        {
            return User
                .ReadByIdAsync(id);
        }

        public Task Create(User user)
        {
            // checar se o name é unico
            // checar se já existe email

            return User.CreateAsync(user);
        }
    }
}

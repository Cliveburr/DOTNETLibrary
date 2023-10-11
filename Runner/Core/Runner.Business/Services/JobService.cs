using Runner.Business.Authentication;
using Runner.Business.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Services
{
    public class JobService : ServiceBase
    {
        private UserLogged _userLogged;

        public JobService(Database database, UserLogged userLogged)
            : base(database)
        {
            _userLogged = userLogged;
        }


    }
}

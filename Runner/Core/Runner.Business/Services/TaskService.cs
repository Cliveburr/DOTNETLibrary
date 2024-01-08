using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Services
{
    public class TaskService : ServiceBase
    {
        public TaskService(Database database)
            : base(database)
        {
        }

        public Task<List<TaskSchedule>> GetSchedulesExpired(DateTime dateTime, List<ObjectId> tasksId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Entities.Tasks.Task>> GetTasksEnabled(List<TaskType> allTaskTypes)
        {
            throw new NotImplementedException();
        }
    }
}

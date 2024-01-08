using MongoDB.Bson;
using Runner.Business.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Tasks
{
    public class TaskSchedule : DocumentBase
    {
        public ObjectId TaskId { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ExecutionUTC { get; set; }
        public TaskScheduleStatus Status { get; set; }
        public object? Parameter { get; set; }
    }
}

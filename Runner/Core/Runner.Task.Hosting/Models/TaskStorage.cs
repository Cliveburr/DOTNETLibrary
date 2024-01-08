using MongoDB.Bson;
using Runner.Business.Entities.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Task.Hosting.Models
{
    public class TaskStorage
    {
        public required ObjectId Id { get; set; }
        public TaskType Type { get; set; }
        public required Type TaskType { get; set; }
    }
}

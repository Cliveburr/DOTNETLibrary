using MongoDB.Bson;
using Runner.Business.Entities.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Entities.Tasks
{
    public class TaskLog : DocumentBase
    {
        public ObjectId TaskId { get; set; }
        public DateTime StartedUTC { get; set; }
        public DateTime EndedUTC { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public object? Result { get; set; }
    }
}

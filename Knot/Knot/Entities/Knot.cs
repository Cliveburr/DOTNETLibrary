using Knot.Business;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knot.Entities
{
    public class Knot
    {
        [BsonId]
        public ObjectId IdKnot { get; set; }
        
        [BsonElement("idParent")]
        public ObjectId IdParent { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("type")]
        public int? Type { get; set; }

        [BsonElement("isRoot")]
        public bool? IsRoot { get; set; }

        [BsonElement("isAction")]
        public bool? IsAction { get; set; }

        [BsonElement("parallel")]
        public bool? Parallel { get; set; }

        [BsonElement("status")]
        public ActionStatus? Status { get; set; }

        [BsonElement("props")]
        [BsonDefaultValue(null)]
        public Dictionary<string, object> Properties { get; set; }

        [BsonIgnore]
        public List<Knot> Childs { get; set; }

        private Knot _parent;
        internal bool loadedChilds = true;
        internal bool loadedProps = true;
        internal List<Knot> originChilds = new List<Knot>();

        public Knot()
        {
            Properties = new Dictionary<string, object>();
        }

        internal Knot(bool isRoot)
        {
            Properties = null;
            if (isRoot)
            {
                IsRoot = true;
            }
        }

        [BsonIgnore]
        public Knot Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
                IdParent = value.IdKnot;
            }
        }

        public override string ToString()
        {
            return $"{{ _id: {IdKnot.ToString()}, name: {Name} }}";
        }
    }
}
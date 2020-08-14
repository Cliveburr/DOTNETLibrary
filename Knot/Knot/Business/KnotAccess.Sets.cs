using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Knot.Business
{
    public partial class KnotAccess
    {
        private void ValidateSet(Entities.Knot knot)
        {
            if (knot.IsRoot ?? false)
            {
                throw new Exception("Can't update the root knot!");
            }

            if (knot.IdParent.Equals(ObjectId.Empty))
            {
                throw new Exception("Invalid parent of knot named: " + knot.Name);
            }

            if (string.IsNullOrEmpty(knot.Name) || knot.Name.Length < 4)
            {
                throw new Exception("Invalid name, need to min 4 length! " + knot.Name);
            }

            if (!knot.loadedChilds && knot.Childs != null)
            {
                throw new Exception("Can only set childs when knot is loaded with childs!");
            }

            if (!knot.loadedProps && knot.Properties != null)
            {
                throw new Exception("Can only set properties when knot is loaded with properties!");
            }
        }

        private UpdateDefinition<Entities.Knot> GetUpdateSets(Entities.Knot knot)
        {
            var update = Builders<Entities.Knot>.Update
                .Set(k => k.Name, knot.Name);

            if (knot.loadedProps)
            {
                update = update
                    .Set(k => k.Properties, knot.Properties);
            }

            if (knot.IsAction ?? false)
            {
                update = update
                    .Set(k => k.Parallel, knot.Parallel)
                    .Set(k => k.Status, knot.Status);
            }

            return update;
        }

        public Entities.Knot Set(Entities.Knot knot)
        {
            ValidateSet(knot);
            if (knot.IdKnot.Equals(ObjectId.Empty))
            {
                knot.loadedProps = true;
                knot.loadedChilds = true;
                Knots.InsertOne(knot);
            }
			else
            {
                var filter = Builders<Entities.Knot>.Filter.Eq(c => c.IdKnot, knot.IdKnot);
                var update = GetUpdateSets(knot);
                Knots.UpdateOne(filter, update);
            }

            if (knot.Childs != null)
            {
                foreach (var child in knot.Childs)
                {
                    child.Parent = knot;
                    Set(child);
                }

                var toUnsets = knot.originChilds
                    .Where(oc => !knot.Childs.Any(kc => kc.IdKnot.Equals(oc.IdKnot)));
                foreach (var unset in toUnsets)
                {
                    Unset(unset);
                }
            }

            return knot;
        }

        private void ValidateUnset(Entities.Knot knot)
        {
            if (knot.IdKnot.Equals(ObjectId.Empty))
            {
                throw new Exception("Invalid knot to unset, named: " + knot.Name);
            }

            if (!knot.loadedChilds)
            {
                throw new Exception("Can only unset when knot is loaded with childs!");
            }
        }

        public void Unset(Entities.Knot knot)
        {
            ValidateUnset(knot);

            foreach (var child in knot.Childs)
            {
                Unset(child);
            }

            var filter = Builders<Entities.Knot>.Filter.Eq(c => c.IdKnot, knot.IdKnot);
            Knots.DeleteOne(filter);
        }
    }
}
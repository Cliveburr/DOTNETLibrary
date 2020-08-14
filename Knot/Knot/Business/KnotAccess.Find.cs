using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Knot.Business
{
    public partial class KnotAccess
    {
        private FindOptionsTranslate TranslateOptions(FindOptions options)
        {
            if (options == null)
            {
                return new FindOptionsTranslate
                {
                    WithChildsDepth = false,
                    WithParentDepth = false
                };
            }

            var translated = new FindOptionsTranslate
            {
                LoadProperties = options.LoadProperties,
                LoadChildsProperties = options.LoadChildsProperties,
                LoadParentProperties = options.LoadParentProperties
            };

            if (options.ParentDepth.HasValue)
            {
                translated.WithParentDepth = true;
                translated.FullParentDepth = options.ParentDepth.Value == 0;
                translated.ParentDepth = options.ParentDepth.Value - 1;
            }
            else
            {
                translated.WithParentDepth = false;
            }

            if (options.ChildsDepth.HasValue)
            {
                translated.WithChildsDepth = true;
                translated.FullChildsDepth = options.ChildsDepth.Value == 0;
                translated.ChildsDepth = options.ChildsDepth.Value - 1;
            }
            else
            {
                translated.WithChildsDepth = false;
            }

            return translated;
        }

        private BsonDocument MakeParentStage(FindOptionsTranslate options)
        {
            var stageFields = new BsonDocument
            {
                    { "from", KnotsName },
                    { "startWith", "$idParent" },
                    { "connectFromField", "idParent"},
                    { "connectToField",  "_id" },
                    { "as", "parents" }
            };

            if (!options.FullParentDepth)
            {
                stageFields.Add("maxDepth", options.ParentDepth);
            }

            return new BsonDocument("$graphLookup", stageFields);
        }

        private BsonDocument MakeChildsStage(FindOptionsTranslate options)
        {
            var stageFields = new BsonDocument
            {
                    { "from", KnotsName },
                    { "startWith", "$_id" },
                    { "connectFromField", "_id"},
                    { "connectToField",  "idParent" },
                    { "as", "childs" }
            };

            if (!options.FullChildsDepth)
            {
                stageFields.Add("maxDepth", options.ChildsDepth);
            }

            return new BsonDocument("$graphLookup", stageFields);
        }

        private BsonDocument MakePropertyExclude()
        {
            return new BsonDocument("$project", new BsonDocument
            {
                { "props", 0 }
            });
        }

        private BsonDocument MakeParentPropertyExclude()
        {
            return new BsonDocument("$project", new BsonDocument
            {
                { "parents", new BsonDocument("props",  0) }
            });
        }

        private BsonDocument MakeChildsPropertyExclude()
        {
            return new BsonDocument("$project", new BsonDocument
            {
                { "childs", new BsonDocument("props",  0) }
            });
        }

        public Entities.Knot FindById(ObjectId id, FindOptions options = null)
        {
            return RunAgg(new BsonDocument("$match", new BsonDocument
                {
                    { "_id", id }
                }), options)
                .FirstOrDefault();
        }

        public IEnumerable<Entities.Knot> FindByName(string name, FindOptions options = null)
        {
            return RunAgg(new BsonDocument("$match", new BsonDocument
                {
                    { "name", name }
                }), options);
        }

        public IEnumerable<Entities.Knot> FindByName(Regex name, FindOptions options = null)
        {
            return RunAgg(new BsonDocument("$match", new BsonDocument
                {
                    {  "name", new BsonDocument("$regex", name) }
                }), options);
        }

        private IEnumerable<Entities.Knot> RunAgg(BsonDocument firstStage, FindOptions options)
        {
            var pipeline = new List<BsonDocument>
            {
                firstStage
            };

            var translatedOptions = TranslateOptions(options);

            if (!translatedOptions.LoadProperties)
            {
                pipeline.Add(MakePropertyExclude());
            }
            if (translatedOptions.WithParentDepth)
            {
                pipeline.Add(MakeParentStage(translatedOptions));
            }
            if (translatedOptions.WithChildsDepth)
            {
                pipeline.Add(MakeChildsStage(translatedOptions));
            }
            if (!translatedOptions.LoadParentProperties)
            {
                pipeline.Add(MakeParentPropertyExclude());
            }
            if (!translatedOptions.LoadChildsProperties)
            {
                pipeline.Add(MakeChildsPropertyExclude());
            }

            return RunAgg(pipeline, translatedOptions);
        }

        private IEnumerable<Entities.Knot> RunAgg(List<BsonDocument> pipeline, FindOptionsTranslate options)
        {
            var definition = (PipelineDefinition<Entities.Knot, Entities.KnotAgg>)pipeline;
            var agg = Knots.Aggregate(definition);

            while (agg.MoveNext())
            {
                foreach (var knotAgg in agg.Current)
                {
                    var knot = knotAgg as Entities.Knot;
                    knot.loadedProps = options.LoadProperties;
                    knot.loadedChilds = options.WithChildsDepth;

                    if (options.WithParentDepth)
                    {
                        ExpandAggParents(knot, knotAgg.ParentsAgg, options);
                    }

                    if (options.WithChildsDepth)
                    {
                        ExpandAggChilds(knot, knotAgg.ChildsAgg, options, options.ChildsDepth);
                    }

                    yield return knot;
                }
            }
        }

        private void ExpandAggParents(Entities.Knot knot, Entities.Knot[] parents, FindOptionsTranslate options)
        {
            var parent = parents
                .FirstOrDefault(p => p.IdKnot == knot.IdParent);

            if (parent != null)
            {
                parent.loadedProps = options.LoadParentProperties;
                parent.loadedChilds = false;
                knot.Parent = parent;

                ExpandAggParents(parent, parents, options);
            }
        }

        private void ExpandAggChilds(Entities.Knot knot, Entities.Knot[] childs, FindOptionsTranslate options, int depth)
        {
            if (!options.FullChildsDepth && depth == -1)
            {
                return;
            }

            var childWhere = childs
                .Where(c => c.IdParent == knot.IdKnot);
            knot.Childs = childWhere.ToList();
            knot.originChilds = childWhere.ToList();

            foreach (var child in knot.Childs)
            {
                child.loadedProps = options.LoadChildsProperties;
                child.loadedChilds = true;
                child.Parent = knot;
                ExpandAggChilds(child, childs, options, depth - 1);
            }
        }
    }
}
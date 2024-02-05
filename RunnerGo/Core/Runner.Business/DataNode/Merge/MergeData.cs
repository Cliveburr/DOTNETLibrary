using Runner.Business.DataNode.Validator;
using Runner.Business.Entities.Nodes.Types;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Runner.Business.DataNode.Merge
{
    public class MergeData
    {
        public IReadOnlyList<DataInst> Properties { get; private set; }

        private List<DataProperty> _waitingDatas;

        private MergeData()
        {
            Properties = new ReadOnlyCollection<DataInst>([]);
            _waitingDatas = new List<DataProperty>();
        }

        public static MergeData From(List<DataProperty> datas)
        {
            var merged = new MergeData();
            merged.ApplyData(datas);
            return merged;
        }

        public void ApplyType(List<DataTypeProperty> types, bool applyDefault = false)
        {
            var list = Properties.ToList();

            foreach (var type in types)
            {
                var has = list
                    .FirstOrDefault(d => d.Name == type.Name);
                if (has is null)
                {
                    var data = new DataInst
                    {
                        Name = type.Name,
                        Type = type.Type,
                        Default = type.Default,
                        IsRequired = type.IsRequired
                    };

                    var hasWaiting = _waitingDatas
                        .FirstOrDefault(d => d.Name == data.Name);
                    if (hasWaiting is not null)
                    {
                        var validation = DataValidator.Validate(type, hasWaiting.Value);
                        if (!validation.Any())
                        {
                            data.Value = hasWaiting.Value;
                        }
                        else
                        {
                            if (applyDefault)
                            {
                                data.Value = type.Default;
                            }
                            else
                            {
                                data.Value = null;
                            }
                        }
                        _waitingDatas.Remove(hasWaiting);
                    }
                    else
                    {
                        if (applyDefault)
                        {
                            data.Value = type.Default;
                        }
                    }

                    list.Add(data);
                }
                else
                {
                    has.Type = type.Type;
                    has.Default = type.Default;
                    has.IsRequired = type.IsRequired;

                    var validation = DataValidator.Validate(type, has.Value);
                    if (validation.Any())
                    {
                        if (applyDefault)  //TODO: try conversion
                        {
                            has.Value = type.Default;
                        }
                        else
                        {
                            has.Value = null;
                        }
                    }
                }
            }

            Properties = new ReadOnlyCollection<DataInst>(list);
        }

        public void ApplyData(List<DataProperty> datas)
        {
            var list = Properties.ToList();

            foreach (var data in datas)
            {
                var has = list
                    .FirstOrDefault(d => d.Name == data.Name);
                if (has is null)
                {
                    var hasWaiting = _waitingDatas
                        .FirstOrDefault(d => d.Name == data.Name);
                    if (hasWaiting is null)
                    {
                        _waitingDatas.Add(data);
                    }
                    else
                    {
                        hasWaiting.Value = data.Value;
                    }
                }
                else
                {
                    has.Value = data.Value;
                }
            }

            Properties = new ReadOnlyCollection<DataInst>(list);
        }

        public List<DataProperty> GetAsDataProperty()
        {
            return Properties
                .Select(p => new DataProperty
                {
                    Name = p.Name,
                    Value = p.Value
                })
                .ToList();
        }

        public IEnumerable<ValidationError> Validate()
        {
            foreach (var property in Properties)
            {
                var validated = DataValidator.Validate(new DataTypeProperty
                {
                    Name = property.Name,
                    Type = property.Type,
                    Default = property.Default,
                    IsRequired = property.IsRequired
                }, property.Value);
                if (validated is not null)
                {
                    foreach (var error in validated)
                    {
                        yield return error;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LearnAOP.AOP.Builder
{
    public class InterfaceBuilder : IBuilder
    {
        private Object _lockGet;
        private IDictionary<string, IInterfacePreBuild> _preBuild;

        public InterfaceBuilder()
        {
            _lockGet = new Object();
            _preBuild = new Dictionary<string, IInterfacePreBuild>();
        }

        public object Generate(ContainerType containerType)
        {
            var type = containerType.ResolvedType.ClassType;
            var typeName = type.FullName;

            lock (_lockGet)
            {
                if (!_preBuild.ContainsKey(typeName))
                {
                    _preBuild.Add(typeName, GeneratePreBuild(containerType));
                }
            }

            var preBuild = _preBuild[typeName];

            return preBuild.Generate();
        }

        private IInterfacePreBuild GeneratePreBuild(ContainerType containerType)
        {
            // checar pelo pipeline de interceptions se alguma aplica-se a algum metodo,
            // para determinar se é necessário criar um proxy
            // se for, criar o proxy e salvar no pre-builder

            return new DirectPreBuild(containerType);
        }
    }

    interface IInterfacePreBuild
    {
        object Generate();
    }

    class DirectPreBuild : IInterfacePreBuild
    {
        private ConstructorInfo _constructor;
        private Container _container;
        private Type _type;
        private ParameterInfo[] _params;

        public DirectPreBuild(ContainerType containerType)
        {
            _container = containerType.Container;
            _type = containerType.ResolvedType.ClassType;
            _constructor = _type.GetConstructors()[0];
            _params = _constructor.GetParameters();
        }

        public object Generate()
        {
            var paramsObjs = new List<object>();
            foreach (var pam in _params)
            {
                var obj = _container.Resolve(pam.ParameterType);
                paramsObjs.Add(obj);
            }
            return Activator.CreateInstance(_type, paramsObjs.ToArray());
        }
    }
}
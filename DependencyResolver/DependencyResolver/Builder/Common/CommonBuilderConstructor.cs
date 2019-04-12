using DependencyResolver.Containers;
using DependencyResolver.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DependencyResolver.Builder.Common
{
    public class CommonBuilderConstructor
    {
        private ConstructorInfo _constructor;
        private ParametersResolved[] _params;

        private void Initialize(Type implementationType, ResolveContext context)
        {
            if (_constructor != null)
            {
                return;
            }

            var constructors = implementationType.GetTypeInfo()
                .DeclaredConstructors
                .Where(constructor => constructor.IsPublic)
                .ToArray();

            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"Invalid class \"{implementationType.FullName}\" without constructor!");
            }
            else if (constructors.Length == 1)
            {
                ResolveSingleConstructor(context, constructors[0]);
            }
            else
            {
                MathBestConstructor(context, constructors);
            }

            if (_params == null)
            {
                var s = constructors.SelectMany(c => c.GetParameters()).Select(p => p.ParameterType.FullName + Environment.NewLine).ToArray();
                var ss = String.Concat(s);

                throw new InvalidOperationException($"Invalid constructor for type \"{implementationType.FullName}\"");
            }
        }

        private void ResolveSingleConstructor(ResolveContext context, ConstructorInfo constructor)
        {
            _constructor = constructor;
            _params = ResolveParameters(context, _constructor.GetParameters());
        }

        private void MathBestConstructor(ResolveContext context, ConstructorInfo[] constructors)
        {
            Array.Sort(constructors,
                (a, b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));

            foreach (var constructor in constructors)
            {
                var parameters = ResolveParameters(context, constructor.GetParameters());
                if (parameters == null)
                {
                    continue;
                }

                _constructor = constructor;
                _params = parameters;
                return;
            }
        }

        private ParametersResolved[] ResolveParameters(ResolveContext context, ParameterInfo[] parameters)
        {
            var result = new ParametersResolved[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = new ParametersResolved
                {
                    Parameter = parameters[i]
                };
                result[i] = parameter;

                var resolvedTypes = context.Container.ResolvedTypes(parameters[i].ParameterType);
                if (resolvedTypes?.Count > 0)
                {
                    parameter.ResolvedType = resolvedTypes[resolvedTypes.Count - 1];
                    continue;
                }

                if (ParameterDefaultValue.TryGetDefaultValue(parameters[i], out var defaultValue))
                {
                    parameter.DefaultValue = defaultValue;
                    continue;
                }

                return null;
            }
            return result;
        }

        private object[] ResolveParameters(ResolveContext context)
        {
            var result = new object[_params.Length];
            for (var i = 0; i < _params.Length; i++)
            {
                var param = _params[i];

                if (param.DefaultValue != null)
                {
                    result[i] = param.DefaultValue;
                }
                else
                {
                    result[i] = param.ResolvedType.Factory.Get(param.Parameter.ParameterType, param.ResolvedType, context);
                }
            }
            return result;
        }

        public object Instantiate(Type implementationType, ResolveContext context)
        {
            Initialize(implementationType, context);

            try
            {
                context.Chain.CheckCircularDependency(implementationType);

                var paramsObjs = ResolveParameters(context);

                return _constructor.Invoke(paramsObjs);
            }
            finally
            {
                context.Chain.Release(implementationType);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agero.Core.Checker;
using Agero.Core.DIContainer.Entities;
using Agero.Core.DIContainer.Exceptions;

namespace Agero.Core.DIContainer.Helpers
{
    internal static class InstanceCreator
    {
        public static Instance Create(IReadOnlyContainer container, Type type, IReadOnlyDictionary<Type, object> unknownTypes)
        {
            Check.ArgumentIsNull(container, nameof(container));
            Check.ArgumentIsNull(type, nameof(type));
            Check.ArgumentIsNull(unknownTypes, nameof(unknownTypes));

            var constructorsWithInjectAttr = InjectAttributeHelper.GetConstructors(type);

            if (constructorsWithInjectAttr.Count > 1)
                throw new ContainerException($"Ambiguous usage of 'Inject' attribute on constructors of type '{type}'. Only one constructor can be marked with 'Inject' attribute.");

            if (constructorsWithInjectAttr.Count == 0)
                return CreateInstanceUsingDefaultConstructor(type);

            Check.Assert(constructorsWithInjectAttr.Count == 1, "constructorsWithInjectAttr.Count == 1");
            
            return CreateInstanceUsingCustomConstructor(container, constructorsWithInjectAttr.Single(), unknownTypes);
        }

        private static Instance CreateInstanceUsingDefaultConstructor(Type type)
        {
            Check.ArgumentIsNull(type, nameof(type));

            var defaultConstructor =
                    type.GetConstructors(ConstantHelper.BindingFlags)
                        .SingleOrDefault(c => !c.GetParameters().Any());

            if (defaultConstructor == null)
                throw new ContainerException($"Type '{type}' does not have either default constructor or constructor marked with 'Inject' attribute.");

            var obj = Activator.CreateInstance(type, ConstantHelper.BindingFlags, null, null, null);

            return new Instance(obj);
        }

        private static Instance CreateInstanceUsingCustomConstructor(IReadOnlyContainer container, ConstructorInfo constructorInfo, IReadOnlyDictionary<Type, object> unknownTypes)
        {
            Check.ArgumentIsNull(container, nameof(container));
            Check.ArgumentIsNull(constructorInfo, nameof(constructorInfo));
            Check.ArgumentIsNull(unknownTypes, nameof(unknownTypes));

            var type = constructorInfo.DeclaringType;

            var typesOfParameters =
                constructorInfo.GetParameters()
                    .Select(p => p.ParameterType)
                    .ToArray();

            var unregisteredParameterTypes =
                typesOfParameters
                    .Where(t => !container.Contains(t))
                    .ToArray();

            var missedParameterTypes =
                unregisteredParameterTypes
                    .Where(t => !unknownTypes.Any(kv => t.IsAssignableFrom(kv.Key)))
                    .ToArray();

            if (missedParameterTypes.Any())
            {
                var stringOfTypes = string.Join(",", missedParameterTypes.Select(t => t.ToString()));
                throw new ContainerException(
                    $"The types ({stringOfTypes}) which are the parameters of constructor of type '{type}' are not registered in the container or not passed as unknown instances.");
            }

            var instancesOfParameters =
                typesOfParameters
                    .Select(t =>
                    {
                        if (container.Contains(t))
                            return container.Get(t);

                        return unknownTypes.First(kv => t.IsAssignableFrom(kv.Key)).Value;
                    })
                    .ToArray();

            var obj = Activator.CreateInstance(type, ConstantHelper.BindingFlags, null, instancesOfParameters, null);

            return new Instance(obj);
        }
    }
}
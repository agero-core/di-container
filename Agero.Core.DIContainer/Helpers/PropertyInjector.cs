using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agero.Core.Checker;
using Agero.Core.DIContainer.Exceptions;

namespace Agero.Core.DIContainer.Helpers
{
    internal static class PropertyInjector
    {
        public static void Inject(IReadOnlyContainer container, object instance, IReadOnlyDictionary<Type, object> unknownTypes)
        {
            Check.ArgumentIsNull(container, nameof(container));
            Check.ArgumentIsNull(instance, nameof(instance));
            Check.ArgumentIsNull(unknownTypes, nameof(unknownTypes));

            var type = instance.GetType();

            var propertiesWithInjectAttr = InjectAttributeHelper.GetProperties(type);

            CheckTypesOfPropertiesAreRegistered(container, type, propertiesWithInjectAttr, unknownTypes);

            foreach (var propertyInfo in propertiesWithInjectAttr)
            {
                var propertyType = propertyInfo.PropertyType;

                var instanceOfPropertyType =
                    container.Contains(propertyType)
                        ? container.Get(propertyType)
                        : unknownTypes.First(kv => propertyType.IsAssignableFrom(kv.Key)).Value;

                propertyInfo.SetValue(instance, instanceOfPropertyType, null);
            }
        }

        private static void CheckTypesOfPropertiesAreRegistered(IReadOnlyContainer container, Type type, IReadOnlyCollection<PropertyInfo> properties, 
            IReadOnlyDictionary<Type, object> unknownTypes)
        {
            Check.ArgumentIsNull(container, nameof(container));
            Check.ArgumentIsNull(type, nameof(type));
            Check.ArgumentIsNull(properties, nameof(properties));
            Check.ArgumentIsNull(unknownTypes, nameof(unknownTypes));
            
            var typesOfProperties =
                properties
                    .Select(p => p.PropertyType)
                    .ToArray();

            var unregisteredPropertyTypes =
                typesOfProperties
                    .Where(t => !container.Contains(t))
                    .ToArray();

            var missedPropertyTypes =
                unregisteredPropertyTypes
                    .Where(t => !unknownTypes.Any(kv => t.IsAssignableFrom(kv.Key)))
                    .ToArray();

            if (missedPropertyTypes.Any())
            {
                var stringOfTypes = string.Join(",", missedPropertyTypes.Select(t => t.ToString()));
                throw new ContainerException($"The property types ({stringOfTypes}) of type '{type}' are not registered in the container or not passed as unknown instances.");
            }
        }
    }
}
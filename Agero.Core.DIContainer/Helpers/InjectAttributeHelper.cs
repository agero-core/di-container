using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Agero.Core.Checker;
using Agero.Core.DIContainer.Attributes;

namespace Agero.Core.DIContainer.Helpers
{
    internal static class InjectAttributeHelper
    {
        public static IReadOnlyCollection<PropertyInfo> GetProperties(Type type)
        {
            Check.ArgumentIsNull(type, nameof(type));

            return type.GetProperties(ConstantHelper.BindingFlags)
                .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(InjectAttribute), true).Any())
                .ToArray();
        }

        public static IReadOnlyCollection<ConstructorInfo> GetConstructors(Type type)
        {
            Check.ArgumentIsNull(type, nameof(type));

            return type.GetConstructors(ConstantHelper.BindingFlags)
                .Where(c => c.GetCustomAttributes(typeof(InjectAttribute), true).Any())
                .ToArray();
        }
    }
}
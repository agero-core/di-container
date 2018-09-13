using System;
using Agero.Core.Checker;

namespace Agero.Core.DIContainer.Entities
{
    internal class ImplementationType : ImplementationDefinition
    {
        public ImplementationType(Type type, Lifetime lifetime)
            : base(lifetime)
        {
            Check.ArgumentIsNull(type, nameof(type));

            Type = type;
        }

        public Type Type { get; }
    }
}
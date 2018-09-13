using System;
using Agero.Core.Checker;

namespace Agero.Core.DIContainer.Entities
{
    internal class ImplementationFactoryMethod : ImplementationDefinition
    {
        public ImplementationFactoryMethod(Func<IReadOnlyContainer, object> factoryMethod, Lifetime lifetime) 
            : base(lifetime)
        {
            Check.ArgumentIsNull(factoryMethod, nameof(factoryMethod));

            FactoryMethod = factoryMethod;
        }

        public Func<IReadOnlyContainer, object> FactoryMethod { get; }
    }
}
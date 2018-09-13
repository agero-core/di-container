namespace Agero.Core.DIContainer.Entities
{
    internal abstract class ImplementationDefinition
    {
        protected ImplementationDefinition(Lifetime lifetime)
        {
            Lifetime = lifetime;
        }

        public Lifetime Lifetime { get; }
    }
}
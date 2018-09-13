namespace Agero.Core.DIContainer
{
    /// <summary>
    /// Determines lifetime of object in DI container.
    /// </summary>
    public enum Lifetime
    {
        /// <summary>Every time instance of a type is requested from DI container - a new instance is returned.</summary>
        PerCall,

        /// <summary>Every time instance of a type is requested from DI container - the same instance is returned.</summary>
        PerContainer
    }
}
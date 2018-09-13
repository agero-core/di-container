using Agero.Core.Checker;
using Agero.Core.DIContainer.Exceptions;

namespace Agero.Core.DIContainer.Extensions
{
    /// <summary>Extensions for <see cref="IContainer"/></summary>
    public static class ContainerExtensions
    {
        /// <summary>Registers type as key and implementation</summary>
        /// <typeparam name="TImplementation">Registration type</typeparam>
        /// <param name="container">DI Container</param>
        /// <param name="lifetime">Lifetime of object in DI container</param>
        /// <exception cref="ContainerException">If implementation type is already registered</exception>
        public static void RegisterImplementation<TImplementation>(this IContainer container, Lifetime lifetime = Lifetime.PerCall)
            where TImplementation : class
        {
            Check.ArgumentIsNull(container, nameof(container));

            container.RegisterImplementation<TImplementation, TImplementation>();
        }

        /// <summary>Registers object of instance type</summary>
        /// <typeparam name="TInstanceType">Registration type</typeparam>
        /// <param name="container">DI Container</param>
        /// <param name="instance">Object of instance type</param>
        /// <remarks>This method registers instance type with <see cref="Lifetime.PerContainer"/> lifetime</remarks>
        /// <exception cref="ContainerException">If instance type is already registered</exception>
        public static void RegisterInstance<TInstanceType>(this IContainer container, TInstanceType instance)
            where TInstanceType : class
        {
            Check.ArgumentIsNull(container, nameof(container));

            container.RegisterInstance<TInstanceType, TInstanceType>(instance);
        }
    }
}
using System;
using Agero.Core.DIContainer.Exceptions;

namespace Agero.Core.DIContainer
{
    /// <summary>DI container</summary>
    public interface IContainer : IReadOnlyContainer, IDisposable
    {
        /// <summary>Registers implementation type as implementation of key type></summary>
        /// <typeparam name="TKey">Registration key type</typeparam>
        /// <typeparam name="TImplementation">Implementation type of key type</typeparam>
        /// <param name="lifetime">Lifetime of object in DI container</param>
        /// <exception cref="ContainerException">If key type is already registered</exception>
        void RegisterImplementation<TKey, TImplementation>(Lifetime lifetime = Lifetime.PerCall)
            where TImplementation : class, TKey;

        /// <summary>Registers implementation type as implementation of key type</summary>
        /// <param name="keyType">Registration key type</param>
        /// <param name="implementationType">Implementation type of key type</param>
        /// <param name="lifetime">Lifetime of object in DI container</param>
        /// <exception cref="ArgumentNullException">If key type is null</exception>
        /// <exception cref="ArgumentNullException">If implementation type is null</exception>
        /// <exception cref="ContainerException">If key type is already registered</exception>
        void RegisterImplementation(Type keyType, Type implementationType, Lifetime lifetime = Lifetime.PerCall);

        /// <summary>Registers key type with factory method</summary>
        /// <typeparam name="TKey">Registration key type</typeparam>
        /// <param name="factoryMethod">Factory method to create instance</param>
        /// <param name="lifetime">Lifetime of object in DI container</param>
        /// <exception cref="ArgumentNullException">If factory method is null</exception>
        /// <exception cref="ContainerException">If key type is already registered</exception>
        void RegisterFactoryMethod<TKey>(Func<IReadOnlyContainer, TKey> factoryMethod, Lifetime lifetime = Lifetime.PerCall);

        /// <summary>Registers key type with factory method</summary>
        /// <param name="keyType">Registration key type</param>
        /// <param name="factoryMethod">Factory method to create instance</param>
        /// <param name="lifetime">Lifetime of object in DI container</param>
        /// <exception cref="ArgumentNullException">If key type is null</exception>
        /// <exception cref="ArgumentNullException">If factory method is null</exception>
        /// <exception cref="ContainerException">If key type is already registered</exception>
        void RegisterFactoryMethod(Type keyType, Func<IReadOnlyContainer, object> factoryMethod, Lifetime lifetime = Lifetime.PerCall);

        /// <summary>Registers object of instance type which is implementation of key type</summary>
        /// <typeparam name="TKey">Registration key type</typeparam>
        /// <typeparam name="TInstanceType">Implementation type of key type</typeparam>
        /// <param name="instance">Object of instance type</param>
        /// <remarks>This method registers instance type as an implementation of key type with <see cref="Lifetime.PerContainer"/> lifetime</remarks>
        /// <exception cref="ContainerException">If key type is already registered</exception>
        void RegisterInstance<TKey, TInstanceType>(TInstanceType instance)
            where TInstanceType : class, TKey;

        /// <summary>Registers object of any type which is implementation of key type</summary>
        /// <param name="keyType">Registration key type</param>
        /// <param name="instance">Object of a type which is implementation of key type</param>
        /// <remarks>This method registers instance object as an implementation of key type with <see cref="Lifetime.PerContainer"/> lifetime</remarks>
        /// <exception cref="ArgumentNullException">If key type is null</exception>
        /// <exception cref="ArgumentNullException">If instance is null</exception>
        /// <exception cref="ContainerException">If type of instance is not derived from key type</exception>
        /// <exception cref="ContainerException">If key type is already registered</exception>
        void RegisterInstance(Type keyType, object instance);

        /// <summary>Removes registration of key type from DI container</summary>
        /// <typeparam name="TKey">Registration key type</typeparam>
        /// <exception cref="ContainerException">If key type is not registered in DI container</exception>
        void Remove<TKey>();

        /// <summary>Removes registration of key type from DI container</summary>
        /// <param name="keyType">Registration key type</param>
        /// <exception cref="ArgumentNullException">If key type is null</exception>
        /// <exception cref="ContainerException">If key type is not registered in DI container</exception>
        void Remove(Type keyType);

        /// <summary>Removes all registrations from DI container</summary>
        /// <remarks>It does not remove registrations for <see cref="IReadOnlyContainer"/> and <see cref="IContainer"/></remarks>
        void ClearAll();

        /// <summary>Remove instance of key type from DI container</summary>
        /// <exception cref="ContainerException">If key type is not registered in DI container</exception>
        /// <exception cref="ContainerException">If DI container doesn't contain an instance of key type</exception>
        void RemoveInstance<TKey>();

        /// <summary>Remove instance of key type from DI container</summary>
        /// <exception cref="ArgumentNullException">If key type is null</exception>
        /// <exception cref="ContainerException">If key type is not registered in DI container</exception>
        /// <exception cref="ContainerException">If DI container doesn't contain an instance of key type</exception>
        void RemoveInstance(Type keyType);

        /// <summary>Removes all instances that DI container holds</summary>
        void ClearAllInstances();
    }
}
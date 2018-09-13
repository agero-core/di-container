using System;
using System.Collections.Generic;
using Agero.Core.DIContainer.Attributes;
using Agero.Core.DIContainer.Exceptions;

namespace Agero.Core.DIContainer
{
    /// <summary>Readonly DI container</summary>
    public interface IReadOnlyContainer
    {
        /// <summary>Returns an instance of registered key type from DI container</summary>
        /// <exception cref="ContainerException">If <see cref="ContainerException"/> type is not registered</exception>
        /// <exception cref="ArgumentException">If unknown instances have more than one instance of the same type</exception>
        TKey Get<TKey>(params object[] unknownInstances);

        /// <summary>Returns an instance of registered key type from DI container</summary>
        /// <exception cref="ArgumentNullException">If key type is null</exception>
        /// <exception cref="ContainerException">If key type is not registered</exception> 
        /// <exception cref="ArgumentException">If unknown instances have more than one instance of the same type</exception>
        object Get(Type keyType, params object[] unknownInstances);

        /// <summary>Checks whether key type is registered in DI container</summary>
        bool Contains<TKey>();

        /// <summary>Checks whether key type is registered in DI container</summary>
        /// <exception cref="ArgumentNullException">If key type is null</exception>
        bool Contains(Type keyType);

        /// <summary>Returns all types registered in DI container</summary>
        IReadOnlyCollection<Type> All { get; }

        /// <summary>Checks whether DI container holds an instance of key type</summary>
        /// <exception cref="ContainerException">If key type is not registered in DI container</exception>
        bool ContainsInstance<TKey>();

        /// <summary>Checks whether DI container holds an instance of key type</summary>
        /// <exception cref="ArgumentNullException">If key type is null</exception>
        /// <exception cref="ContainerException">If key type is not registered in DI container</exception>
        bool ContainsInstance(Type keyType);
        
        /// <summary>Returns all instances of registered types that DI container holds</summary>
        IReadOnlyCollection<object> AllInstances { get; }

        /// <summary>Create an instance of type and injects register in DI container types</summary>
        /// <exception cref="ContainerException">If there is ambiguous usage of <see cref="InjectAttribute"/> attribute on constructors</exception>
        /// <exception cref="ContainerException">If injected type is not registered in DI container and it is not in unknown instances</exception>
        /// <exception cref="ArgumentException">If unknown instances have more than one instance of the same type</exception>
        T CreateInstance<T>(params object[] unknownInstances);

        /// <summary>Creates an instance of type and injects registered in DI container types</summary>
        /// <exception cref="ArgumentNullException">If type is null</exception> 
        /// <exception cref="ContainerException">If there is ambiguous usage of <see cref="InjectAttribute"/> attribute on constructors</exception>
        /// <exception cref="ContainerException">If injected type is not registered in DI container and it is not in unknown instances</exception>
        /// <exception cref="ArgumentException">If unknown instances have more than one instance of the same type</exception>
        object CreateInstance(Type type, params object[] unknownInstances);

        /// <summary>The root which is used for syncronization of threads</summary>
        object SyncRoot { get; }
    }
}
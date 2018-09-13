using System;
using System.Collections.Generic;
using System.Linq;
using Agero.Core.Checker;
using Agero.Core.DIContainer.Entities;
using Agero.Core.DIContainer.Exceptions;
using Agero.Core.DIContainer.Helpers;

namespace Agero.Core.DIContainer
{
    internal class Container : IContainer
    {
        #region Fields

        private readonly IDictionary<Type, ImplementationDefinition> _implementationDefinitions;
        private readonly IDictionary<Type, Instance> _instances;

        #endregion


        #region Constructors

        public Container()
        {
            _implementationDefinitions = new Dictionary<Type, ImplementationDefinition>();
            _instances = new Dictionary<Type, Instance>();

            RegisterContainer();
        }

        #endregion


        #region IReadOnlyContainer

        public TKey Get<TKey>(params object[] unknownInstances)
        {
            return (TKey)Get(typeof(TKey), unknownInstances);
        }

        public object Get(Type keyType, params object[] unknownInstances)
        {
            Check.ArgumentIsNull(keyType, nameof(keyType));
            Check.ArgumentIsNull(unknownInstances, nameof(unknownInstances));

            if (unknownInstances.GroupBy(i => i.GetType()).Any(g => g.Count() > 1))
                throw new ArgumentException($"{nameof(unknownInstances)} has more than one entry of the same type.", nameof(unknownInstances));

            var unknownTypes = unknownInstances.ToDictionary(i => i.GetType(), i => i);

            lock (SyncRoot)
            {
                if (!Contains(keyType))
                    throw new ContainerException($"Container does not contain '{keyType}' type as a key.");

                {
                    if (_instances.TryGetValue(keyType, out var instance))
                        return instance.Object;
                }

                {
                    Instance createInstance(ImplementationDefinition implementation)
                    {
                        Check.ArgumentIsNull(implementation, nameof(implementation));

                        switch (implementation)
                        {
                            case ImplementationType implementationType:
                                return InstanceCreator.Create(this, implementationType.Type, unknownTypes);
                            
                            case ImplementationFactoryMethod implementationFactoryMethod:
                                return new Instance(implementationFactoryMethod.FactoryMethod(this));
                            
                            default:
                                throw new InvalidOperationException($"Implementation definition '{implementation.GetType()}' type is not supported.");
                        }
                    }

                    var implementationDefinition = _implementationDefinitions[keyType];
                    var instance = createInstance(implementationDefinition);
                    var obj = instance.Object;
                    PropertyInjector.Inject(this, instance.Object, unknownTypes);

                    if (implementationDefinition.Lifetime == Lifetime.PerContainer)
                        _instances.Add(keyType, instance);

                    return obj;
                }
            }
        }

        public bool Contains<TKey>()
        {
            return Contains(typeof(TKey));
        }

        public bool Contains(Type keyType)
        {
            Check.ArgumentIsNull(keyType, nameof(keyType));

            lock (SyncRoot)
            {
                return _implementationDefinitions.ContainsKey(keyType);
            }
        }

        public IReadOnlyCollection<Type> All
        {
            get
            {
                lock (SyncRoot)
                {
                    return _implementationDefinitions.Keys.ToArray();
                }
            }
        }

        public bool ContainsInstance<TKey>()
        {
            return ContainsInstance(typeof(TKey));
        }

        public bool ContainsInstance(Type keyType)
        {
            Check.ArgumentIsNull(keyType, nameof(keyType));

            lock (SyncRoot)
            {
                if (!Contains(keyType))
                    throw new ContainerException($"Container does not contain '{keyType}' type as a key.");

                return _instances.ContainsKey(keyType);
            }
        }

        public IReadOnlyCollection<object> AllInstances
        {
            get
            {
                lock (SyncRoot)
                {
                    return 
                        _instances.Values
                            .Select(i => i.Object)
                            .Distinct()
                            .ToArray();
                }
            }
        }

        public T CreateInstance<T>(params object[] unknownInstances)
        {
            return (T)CreateInstance(typeof(T), unknownInstances);
        }

        public object CreateInstance(Type type, params object[] unknownInstances)
        {
            Check.ArgumentIsNull(type, nameof(type));
            Check.ArgumentIsNull(unknownInstances, nameof(unknownInstances));

            if (unknownInstances.GroupBy(i => i.GetType()).Any(g => g.Count() > 1))
                throw new ArgumentException($"{nameof(unknownInstances)} has more than one entry of the same type.", nameof(unknownInstances));

            var unknownTypes = unknownInstances.ToDictionary(i => i.GetType(), i => i);

            lock (SyncRoot)
            {
                var instance = InstanceCreator.Create(this, type, unknownTypes);
                PropertyInjector.Inject(this, instance.Object, unknownTypes);

                return instance.Object;
            }
        }

        public object SyncRoot { get; } = new object();

        #endregion


        #region IContainer

        public void RegisterImplementation<TKey, TImplementation>(Lifetime lifetime = Lifetime.PerCall)
            where TImplementation : class, TKey
        {
            RegisterImplementation(typeof(TKey), typeof(TImplementation), lifetime);
        }

        public void RegisterImplementation(Type keyType, Type implementationType, Lifetime lifetime = Lifetime.PerCall)
        {
            Check.ArgumentIsNull(keyType, nameof(keyType));
            Check.ArgumentIsNull(implementationType, nameof(implementationType));

            if (!keyType.IsAssignableFrom(implementationType))
                throw new ContainerException($"Type '{implementationType}' must be derived from type '{keyType}'.");

            lock (SyncRoot)
            {
                if (Contains(keyType))
                    throw new ContainerException($"Container already contains '{keyType}' type as a key.");

                var implementation = new ImplementationType(implementationType, lifetime);

                _implementationDefinitions.Add(keyType, implementation);
            }
        }

        public void RegisterImplementation<TImplementation>(Lifetime lifetime = Lifetime.PerCall) 
            where TImplementation : class
        {
            RegisterImplementation<TImplementation, TImplementation>();
        }

        public void RegisterFactoryMethod<TKey>(Func<IReadOnlyContainer, TKey> factoryMethod, Lifetime lifetime = Lifetime.PerCall)
        {
            Check.ArgumentIsNull(factoryMethod, nameof(factoryMethod));

            RegisterFactoryMethod(typeof(TKey), container => factoryMethod(container), lifetime);
        }

        public void RegisterFactoryMethod(Type keyType, Func<IReadOnlyContainer, object> factoryMethod, Lifetime lifetime = Lifetime.PerCall)
        {
            Check.ArgumentIsNull(keyType, nameof(keyType));
            Check.ArgumentIsNull(factoryMethod, nameof(factoryMethod));

            lock (SyncRoot)
            {
                if (Contains(keyType))
                    throw new ContainerException($"Container already contains '{keyType}' type as a key.");

                var implementation = new ImplementationFactoryMethod(factoryMethod, lifetime);

                _implementationDefinitions.Add(keyType, implementation);
            }
        }

        public void RegisterInstance<TKey, TInstanceType>(TInstanceType instance)
            where TInstanceType : class, TKey
        {
            Check.ArgumentIsNull(instance, nameof(instance));
            
            RegisterInstance(typeof(TKey), instance);
        }

        public void RegisterInstance(Type keyType, object instance)
        {
            Check.ArgumentIsNull(keyType, nameof(keyType));
            Check.ArgumentIsNull(instance, nameof(instance));

            if (!keyType.IsInstanceOfType(instance))
                throw new ContainerException($"Type '{instance.GetType()}' must be derived from type '{keyType}'.");

            lock (SyncRoot)
            {
                if (Contains(keyType))
                    throw new ContainerException($"Container already contains '{keyType}' type as a key.");

                _implementationDefinitions.Add(keyType, new ImplementationType(instance.GetType(), Lifetime.PerContainer));
                _instances.Add(keyType, new Instance(instance));
            }
        }

        public void RegisterInstance<TInstanceType>(TInstanceType instance) 
            where TInstanceType : class
        {
            Check.ArgumentIsNull(instance, nameof(instance));
            
            RegisterInstance<TInstanceType, TInstanceType>(instance);
        }

        public void Remove<TKey>()
        {
            Remove(typeof(TKey));
        }

        public void Remove(Type keyType)
        {
            Check.ArgumentIsNull(keyType, nameof(keyType));

            if (keyType == typeof(IReadOnlyContainer) || keyType == typeof(IContainer))
                throw new ContainerException($"The key type '{keyType}' cannot be removed from container.");

            lock (SyncRoot)
            {
                if (!Contains(keyType))
                    throw new ContainerException($"Container does not contain '{keyType}' type as a key.");

                if (_instances.ContainsKey(keyType))
                    _instances.Remove(keyType);

                _implementationDefinitions.Remove(keyType);
            }
        }

        public void ClearAll()
        {
            lock (SyncRoot)
            {
                _implementationDefinitions.Clear();
                _instances.Clear();

                RegisterContainer();
            }
        }

        public void RemoveInstance<TKey>()
        {
            RemoveInstance(typeof(TKey));
        }

        public void RemoveInstance(Type keyType)
        {
            Check.ArgumentIsNull(keyType, nameof(keyType));

            if (keyType == typeof(IReadOnlyContainer) || keyType == typeof(IContainer))
                throw new ContainerException($"The instance of key type '{keyType}' cannot be removed from container.");

            lock (SyncRoot)
            {
                if (!Contains(keyType))
                    throw new ContainerException($"Container does not contain '{keyType}' type as a key.");

                if (!ContainsInstance(keyType))
                    throw new ContainerException($"Container does not contain an instance of '{keyType}' type.");

                _instances.Remove(keyType);
            }
        }

        public void ClearAllInstances()
        {
            lock (SyncRoot)
            {
                _instances.Clear();

                _instances.Add(typeof(IReadOnlyContainer), new Instance(this));
                _instances.Add(typeof(IContainer), new Instance(this));
            }
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            _implementationDefinitions.Clear();
            _instances.Clear();
        }

        #endregion


        #region Private Methods

        private void RegisterContainer()
        {
            RegisterInstance<IReadOnlyContainer, Container>(this);
            RegisterInstance<IContainer, Container>(this);
        }

        #endregion
    }
}
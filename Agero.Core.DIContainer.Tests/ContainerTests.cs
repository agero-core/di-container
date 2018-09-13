using System;
using System.Linq;
using Agero.Core.DIContainer.Exceptions;
using Agero.Core.DIContainer.Tests.Helpers;
using Agero.Core.DIContainer.Tests.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Agero.Core.DIContainer.Tests
{
    [TestClass]
    public class ContainerTests
    {
        #region Get

        [TestMethod]
        public void Get_Should_Set_UnknownInstances()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterfaceWithUnknownInstance, ContainerTestClassWithUnknownInstance>();
            var unknownInstance = new ContainerUnknownInstanceTestClass();

            // Act
            var instance = container.Get<IContainerTestInterfaceWithUnknownInstance>(unknownInstance);

            // Assert
            Assert.AreSame(unknownInstance, instance.PropertyUnknownInstance);
            Assert.AreSame(unknownInstance, instance.ConstructorUnknownInstance);
        }

        [TestMethod]
        public void Get_Should_Throw_Exception_When_UnknownInstances_Has_More_Than_One_Entry_Of_The_Same_Type()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterfaceWithUnknownInstance, ContainerTestClassWithUnknownInstance>();
            var unknownInstance1 = new ContainerUnknownInstanceTestClass();
            var unknownInstance2 = new ContainerUnknownInstanceTestClass();

            // Act
            // Assert
            ExceptionAssert.Thrown<ArgumentException>(
                () => container.Get<IContainerTestInterfaceWithUnknownInstance>(unknownInstance1, unknownInstance2),
                $"unknownInstances has more than one entry of the same type.{Environment.NewLine}Parameter name: unknownInstances");
        }

        [TestMethod]
        public void Get_Should_Throw_Exception_When_Type_Is_Not_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.Get<IContainerTestInterface>(),
                "Container does not contain 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }
        
        [TestMethod]
        public void Get_When_Implementation_Is_Registered_And_Lifetime_Is_PerCall()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // ReSharper disable once RedundantArgumentDefaultValue
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(Lifetime.PerCall);
            
            // Act
            var instance1 = container.Get<IContainerTestInterface>();
            var instance2 = container.Get<IContainerTestInterface>();

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void Get_When_Implementation_Is_Registered_And_Lifetime_Is_PerContainer()
        {
            // Arrange
            var container = ContainerFactory.Create();

            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(Lifetime.PerContainer);

            // Act
            var instance1 = container.Get<IContainerTestInterface>();
            var instance2 = container.Get<IContainerTestInterface>();

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void Get_When_FactoryMethod_Is_Registered_And_Lifetime_Is_PerCall()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // ReSharper disable once RedundantArgumentDefaultValue
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass(), Lifetime.PerCall);

            // Act
            var instance1 = container.Get<IContainerTestInterface>();
            var instance2 = container.Get<IContainerTestInterface>();

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void Get_When_FactoryMethod_Is_Registered_And_Lifetime_Is_PerContainer()
        {
            // Arrange
            var container = ContainerFactory.Create();

            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass(), Lifetime.PerContainer);

            // Act
            var instance1 = container.Get<IContainerTestInterface>();
            var instance2 = container.Get<IContainerTestInterface>();

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void Get_When_Instance_Is_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();

            var instance = new ContainerTestClass();

            container.RegisterInstance<IContainerTestInterface, ContainerTestClass>(instance);

            // Act
            var returnedInstance = container.Get<IContainerTestInterface>();

            // Assert
            Assert.IsNotNull(returnedInstance);
            Assert.AreSame(instance, returnedInstance);
        }

        [TestMethod]
        public void Get_Should_Throw_Exception_When_Constructor_Injected_Instance_Is_Not_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface2, ContainerTestClass2>();
            container.RegisterImplementation<IContainerTestInterfaceWithInjectedInstances, ContainerTestClassWithInjectedInstances>();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.Get<IContainerTestInterfaceWithInjectedInstances>(),
                "The types (Agero.Core.DIContainer.Tests.Types.IContainerTestInterface) which" +
                " are the parameters of constructor of type 'Agero.Core.DIContainer.Tests.Types.ContainerTestClassWithInjectedInstances'" +
                " are not registered in the container or not passed as unknown instances.");
        }

        [TestMethod]
        public void Get_Should_Throw_Exception_When_Property_Injected_Instance_Is_Not_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();
            container.RegisterImplementation<IContainerTestInterfaceWithInjectedInstances, ContainerTestClassWithInjectedInstances>();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.Get<IContainerTestInterfaceWithInjectedInstances>(),
                "The property types (Agero.Core.DIContainer.Tests.Types.IContainerTestInterface2) of type" +
                " 'Agero.Core.DIContainer.Tests.Types.ContainerTestClassWithInjectedInstances' are not registered" +
                " in the container or not passed as unknown instances.");
        }

        [TestMethod]
        public void Get_When_FactoryMethod_Is_Registered_And_Property_Is_Injected()
        {
            // Arrange
            var container = ContainerFactory.Create();

            var instance1 = new ContainerTestClass();
            container.RegisterInstance<IContainerTestInterface>(instance1);

            container.RegisterFactoryMethod<IContainerTestInterfaceWithInjectedProperties>(c => new ContainerTestInterfaceWithInjectedProperties());

            // Act
            var instance2 = container.Get<IContainerTestInterfaceWithInjectedProperties>();

            // Assert
            Assert.IsNotNull(instance2);
            Assert.AreSame(instance1, instance2.Instance);
        }
        
        #endregion


        #region Contains

        [TestMethod]
        public void Contains_Should_Return_True_When_Implementation_Is_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();

            // Act
            var result = container.Contains<IContainerTestInterface>();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_Should_Return_True_When_FactoryMethod_Is_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass());

            // Act
            var result = container.Contains<IContainerTestInterface>();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_Should_Return_True_When_Instance_Is_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterInstance<IContainerTestInterface, ContainerTestClass>(new ContainerTestClass());

            // Act
            var result = container.Contains<IContainerTestInterface>();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_Should_Return_False_When_Type_Is_Not_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // Act
            var result = container.Contains<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(result);
        }

        #endregion


        #region All

        [TestMethod]
        public void All_Should_Return_All_Types()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();
            container.RegisterInstance<IContainerTestInterface2, ContainerTestClass2>(new ContainerTestClass2());
            container.RegisterFactoryMethod<IContainerTestInterfaceWithInjectedProperties>(c => new ContainerTestInterfaceWithInjectedProperties());

            // Act
            var result = container.All;

            // Assert
            var expected =
                new[]
                {
                    typeof (IReadOnlyContainer),
                    typeof (IContainer),
                    typeof (IContainerTestInterface),
                    typeof (IContainerTestInterface2),
                    typeof (IContainerTestInterfaceWithInjectedProperties),
                };

            CollectionAssert.AreEquivalent(expected, result.ToArray());
        }

        #endregion


        #region ContainsInstance

        [TestMethod]
        public void ContainsInstance_Should_Throw_Exception_When_Type_Is_Not_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.ContainsInstance<IContainerTestInterface>(),
                "Container does not contain 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        [TestMethod]
        public void ContainsInstance_Should_Return_True_When_Instance_Is_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterInstance<IContainerTestInterface, ContainerTestClass>(new ContainerTestClass());

            // Act
            var result = container.ContainsInstance<IContainerTestInterface>();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsInstance_Should_Return_True_When_Implementation_Is_Registered_Per_Container_And_Instance_Is_Created()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(Lifetime.PerContainer);
            container.Get<IContainerTestInterface>();

            // Act
            var result = container.ContainsInstance<IContainerTestInterface>();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsInstance_Should_Return_True_When_FactoryMethod_Is_Registered_Per_Container_And_Instance_Is_Created()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass(), Lifetime.PerContainer);
            container.Get<IContainerTestInterface>();

            // Act
            var result = container.ContainsInstance<IContainerTestInterface>();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsInstance_Should_Return_False_When_Implementation_Is_Registered_Per_Container_And_Instance_Is_Not_Created()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(Lifetime.PerContainer);

            // Act
            var result = container.ContainsInstance<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ContainsInstance_Should_Return_False_When_FactoryMethod_Is_Registered_Per_Container_And_Instance_Is_Not_Created()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass(), Lifetime.PerContainer);

            // Act
            var result = container.ContainsInstance<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ContainsInstance_Should_Return_False_When_Implementation_Is_Registered_Per_Call()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();
            container.Get<IContainerTestInterface>();

            // Act
            var result = container.ContainsInstance<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(result);
        }

        #endregion


        #region AllInstances

        [TestMethod]
        public void AllInstances_Should_Return_All_Instances()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();
            container.RegisterInstance<IContainerTestInterface2, ContainerTestClass2>(new ContainerTestClass2());
            container.RegisterImplementation<IContainerTestInterfaceWithInjectedInstances, ContainerTestClassWithInjectedInstances>(Lifetime.PerContainer);
            container.Get<IContainerTestInterfaceWithInjectedInstances>();
            container.RegisterFactoryMethod<IContainerTestInterfaceWithInjectedProperties>(c => new ContainerTestInterfaceWithInjectedProperties(), Lifetime.PerContainer);
            container.Get<IContainerTestInterfaceWithInjectedProperties>();

            // Act
            var result = container.AllInstances;

            // Assert
            var expected =
                new[]
                {
                    typeof (Container),
                    typeof (ContainerTestClass2),
                    typeof (ContainerTestClassWithInjectedInstances),
                    typeof (ContainerTestInterfaceWithInjectedProperties)
                };
            var actual =
                result
                    .Select(o => o.GetType())
                    .ToArray();
            
            CollectionAssert.AreEquivalent(expected, actual);
        }

        #endregion


        #region CreateInstance

        [TestMethod]
        public void CreateInstance_Should_Throw_Exception_When_Type_Has_Ambiguous_Constructors()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.CreateInstance<ContainerTestClassWithAmbiguousConstructors>(),
                "Ambiguous usage of 'Inject' attribute on constructors of type 'Agero.Core.DIContainer.Tests.Types.ContainerTestClassWithAmbiguousConstructors'." +
                " Only one constructor can be marked with 'Inject' attribute.");
        }
        
        [TestMethod]
        public void CreateInstance_Should_Set_UnknownInstances()
        {
            // Arrange
            var container = ContainerFactory.Create();
            var unknownInstance = new ContainerUnknownInstanceTestClass();

            // Act
            var instance = container.CreateInstance<ContainerTestClassWithUnknownInstance>(unknownInstance);

            // Assert
            Assert.AreSame(unknownInstance, instance.PropertyUnknownInstance);
            Assert.AreSame(unknownInstance, instance.ConstructorUnknownInstance);
        }

        [TestMethod]
        public void CreateInstance_Should_Throw_Exception_When_UnknownInstances_Has_More_Than_One_Entry_Of_The_Same_Type()
        {
            // Arrange
            var container = ContainerFactory.Create();
            var unknownInstance1 = new ContainerUnknownInstanceTestClass();
            var unknownInstance2 = new ContainerUnknownInstanceTestClass();

            // Act
            // Assert
            ExceptionAssert.Thrown<ArgumentException>(
                () => container.CreateInstance<ContainerTestClassWithUnknownInstance>(unknownInstance1, unknownInstance2),
                $"unknownInstances has more than one entry of the same type.{Environment.NewLine}Parameter name: unknownInstances");
        }

        [TestMethod]
        public void CreateInstance_Should_Throw_Exception_When_Constructor_Injected_Instance_Is_Not_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface2, ContainerTestClass2>();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.CreateInstance<ContainerTestClassWithInjectedInstances>(),
                "The types (Agero.Core.DIContainer.Tests.Types.IContainerTestInterface) which" +
                " are the parameters of constructor of type 'Agero.Core.DIContainer.Tests.Types.ContainerTestClassWithInjectedInstances'" +
                " are not registered in the container or not passed as unknown instances.");
        }

        [TestMethod]
        public void CreateInstance_Should_Throw_Exception_When_Property_Injected_Instance_Is_Not_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.CreateInstance<ContainerTestClassWithInjectedInstances>(),
                "The property types (Agero.Core.DIContainer.Tests.Types.IContainerTestInterface2) of type" +
                " 'Agero.Core.DIContainer.Tests.Types.ContainerTestClassWithInjectedInstances' are not registered" +
                " in the container or not passed as unknown instances.");
        }

        #endregion


        #region RegisterImplementation

        [TestMethod]
        public void RegisterImplementation()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // Act
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();
            
            // Assert
            Assert.IsTrue(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void RegisterImplementation_Should_Throw_Exception_When_Implementation_Is_Already_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(),
                "Container already contains 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        [TestMethod]
        public void RegisterImplementation_Should_Throw_Exception_When_FactoryMethod_Is_Already_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass());

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(),
                "Container already contains 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        [TestMethod]
        public void RegisterImplementation_Should_Throw_Exception_When_Instance_Is_Already_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterInstance<IContainerTestInterface>(new ContainerTestClass());

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(),
                "Container already contains 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        #endregion


        #region RegisterFactoryMethod

        [TestMethod]
        public void RegisterFactoryMethod()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // Act
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass());

            // Assert
            Assert.IsTrue(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void RegisterFactoryMethod_Should_Throw_Exception_When_Implementation_Is_Already_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass()),
                "Container already contains 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        [TestMethod]
        public void RegisterFactoryMethod_Should_Throw_Exception_When_Instance_Is_Already_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterInstance<IContainerTestInterface>(new ContainerTestClass());

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass()),
                "Container already contains 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        #endregion


        #region RegisterInstance

        [TestMethod]
        public void RegisterInstance()
        {
            // Arrange
            var container = ContainerFactory.Create();
            
            // Act
            container.RegisterInstance<IContainerTestInterface>(new ContainerTestClass());

            // Assert
            Assert.IsTrue(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void RegisterInstance_Should_Throw_Exception_When_Implementation_Is_Already_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.RegisterInstance<IContainerTestInterface>(new ContainerTestClass()),
                "Container already contains 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        [TestMethod]
        public void RegisterInstance_Should_Throw_Exception_When_FactoryMethod_Is_Already_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass());

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.RegisterInstance<IContainerTestInterface>(new ContainerTestClass()),
                "Container already contains 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        [TestMethod]
        public void RegisterInstance_Should_Throw_Exception_When_Instance_Is_Already_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterInstance<IContainerTestInterface>(new ContainerTestClass());

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                () => container.RegisterInstance<IContainerTestInterface>(new ContainerTestClass()),
                "Container already contains 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        #endregion


        #region Remove

        [TestMethod]
        public void Remove_When_Instance_Is_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterInstance<IContainerTestInterface>(new ContainerTestClass());

            // Act
            container.Remove<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void Remove_When_Implementation_Is_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();

            // Act
            container.Remove<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void Remove_When_Implementation_Is_Registered_And_Get_Is_Called()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(Lifetime.PerContainer);
            container.Get<IContainerTestInterface>();

            // Act
            container.Remove<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void Remove_When_FactoryMethod_Is_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass());

            // Act
            container.Remove<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void Remove_When_FactoryMethod_Is_Registered_And_Get_Is_Called()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass(), Lifetime.PerContainer);
            container.Get<IContainerTestInterface>();

            // Act
            container.Remove<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void Remove_Should_Throw_Exception_When_Type_Is_Not_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                container.Remove<IContainerTestInterface>,
                "Container does not contain 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        #endregion


        #region ClearAll

        [TestMethod]
        public void ClearAll_Should_Clear_All_Registered_Types()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();
            container.RegisterInstance<IContainerTestInterface2, ContainerTestClass2>(new ContainerTestClass2());
            container.RegisterFactoryMethod<IContainerTestInterfaceWithInjectedProperties>(c => new ContainerTestInterfaceWithInjectedProperties());

            // Act
            container.ClearAll();

            // Assert
            var expected =
                new[]
                {
                    typeof (IReadOnlyContainer),
                    typeof (IContainer)
                };
            CollectionAssert.AreEquivalent(expected, container.All.ToArray());
            CollectionAssert.AreEquivalent(new object[] { container }, container.AllInstances.ToArray());
        }

        #endregion


        #region RemoveInstance

        [TestMethod]
        public void RemoveInstance_Should_Throw_Exception_When_Type_Is_Not_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                container.RemoveInstance<IContainerTestInterface>,
                "Container does not contain 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type as a key.");
        }

        [TestMethod]
        public void RemoveInstance_Should_Remove_Instance_When_Instance_Is_Registered()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterInstance<IContainerTestInterface, ContainerTestClass>(new ContainerTestClass());

            // Act
            container.RemoveInstance<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(container.ContainsInstance<IContainerTestInterface>());
            Assert.IsTrue(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void RemoveInstance_Should_Remove_Instance_When_Implementation_Is_Registered_Per_Container_And_Instance_Is_Created()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(Lifetime.PerContainer);
            container.Get<IContainerTestInterface>();

            // Act
            container.RemoveInstance<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(container.ContainsInstance<IContainerTestInterface>());
            Assert.IsTrue(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void RemoveInstance_Should_Throw_Exception_When_Implementation_Is_Registered_Per_Container_And_Instance_Is_Not_Created()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>(Lifetime.PerContainer);

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                container.RemoveInstance<IContainerTestInterface>,
                "Container does not contain an instance of 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type.");
        }

        [TestMethod]
        public void RemoveInstance_Should_Throw_Exception_When_Implementation_Is_Registered_Per_Call()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();
            container.Get<IContainerTestInterface>();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                container.RemoveInstance<IContainerTestInterface>,
                "Container does not contain an instance of 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type.");
        }

        [TestMethod]
        public void RemoveInstance_Should_Remove_Instance_When_FactoryMethod_Is_Registered_Per_Container_And_Instance_Is_Created()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass(), Lifetime.PerContainer);
            container.Get<IContainerTestInterface>();

            // Act
            container.RemoveInstance<IContainerTestInterface>();

            // Assert
            Assert.IsFalse(container.ContainsInstance<IContainerTestInterface>());
            Assert.IsTrue(container.Contains<IContainerTestInterface>());
        }

        [TestMethod]
        public void RemoveInstance_Should_Throw_Exception_When_FactoryMethod_Is_Registered_Per_Container_And_Instance_Is_Not_Created()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass(), Lifetime.PerContainer);

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                container.RemoveInstance<IContainerTestInterface>,
                "Container does not contain an instance of 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type.");
        }

        [TestMethod]
        public void RemoveInstance_Should_Throw_Exception_When_FactoryMethod_Is_Registered_Per_Call()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterFactoryMethod<IContainerTestInterface>(c => new ContainerTestClass());
            container.Get<IContainerTestInterface>();

            // Act
            // Assert
            ExceptionAssert.Thrown<ContainerException>(
                container.RemoveInstance<IContainerTestInterface>,
                "Container does not contain an instance of 'Agero.Core.DIContainer.Tests.Types.IContainerTestInterface' type.");
        }

        #endregion


        #region ClearAllInstances

        [TestMethod]
        public void ClearAllInstances_Should_Clear_All_Instances()
        {
            // Arrange
            var container = ContainerFactory.Create();
            container.RegisterImplementation<IContainerTestInterface, ContainerTestClass>();
            container.RegisterInstance<IContainerTestInterface2, ContainerTestClass2>(new ContainerTestClass2());
            container.RegisterImplementation<IContainerTestInterfaceWithInjectedInstances, ContainerTestClassWithInjectedInstances>(Lifetime.PerContainer);
            container.Get<IContainerTestInterfaceWithInjectedInstances>();
            container.RegisterFactoryMethod<IContainerTestInterfaceWithInjectedProperties>(c => new ContainerTestInterfaceWithInjectedProperties(), Lifetime.PerContainer);
            container.Get<IContainerTestInterfaceWithInjectedProperties>();

            // Act
            container.ClearAllInstances();

            // Assert
            var expected =
                new[]
                {
                    typeof (IReadOnlyContainer),
                    typeof (IContainer),
                    typeof (IContainerTestInterface),
                    typeof (IContainerTestInterface2),
                    typeof (IContainerTestInterfaceWithInjectedInstances),
                    typeof (IContainerTestInterfaceWithInjectedProperties)
                };
            CollectionAssert.AreEquivalent(expected, container.All.ToArray());
            CollectionAssert.AreEquivalent(new object[] { container }, container.AllInstances.ToArray());
        }

        #endregion
    }
}
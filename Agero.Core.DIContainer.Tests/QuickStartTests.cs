using Agero.Core.DIContainer.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Agero.Core.DIContainer.Tests
{
    [TestClass]
    public class QuickStartTests
    {
        public interface ISample { }

        public class Sample : ISample { }

        public interface ISingletonSample { }

        public class SingletonSample : ISingletonSample { }

        public interface IInstanceSample { }

        public class InstanceSample : IInstanceSample { }

        [TestMethod]
        public void Test_Registration_And_Retrieving()
        {
            //  Creating DI container
            var container = ContainerFactory.Create();

            // Registration a class as an implementation of an interface
            container.RegisterImplementation<ISample, Sample>();

            // Retrieving registered implementation
            var instance = container.Get<ISample>();
            Assert.AreEqual(typeof (Sample), instance.GetType());

            // Registration as a singleton
            container.RegisterImplementation<ISingletonSample, SingletonSample>(Lifetime.PerContainer);
            var instance1 = container.Get<ISingletonSample>();
            var instance2 = container.Get<ISingletonSample>();
            Assert.AreSame(instance1, instance2);

            // Registration an instance of a class as an instance of an interface type
            var instanceSample = new InstanceSample();
            container.RegisterInstance<IInstanceSample, InstanceSample>(instanceSample);
            var retrievedInstance = container.Get<IInstanceSample>();
            Assert.AreSame(instanceSample, retrievedInstance);
        }

        public interface IInjectSample
        {
            ISample SampleProperty { get; }

            IInstanceSample InstanceSampleProperty { get; }
        }

        public class InjectSample : IInjectSample
        {
            // An instance of IInstanceSample will be injected on creating InjectSample
            [Inject]
            public InjectSample(IInstanceSample instanceSample)
            {
                InstanceSampleProperty = instanceSample;
            }

            // An instance of ISample will be injected right after creating InjectSample
            [Inject]
            public ISample SampleProperty { get; set; }

            public IInstanceSample InstanceSampleProperty { get; private set; }
        }

        [TestMethod]
        public void Test_Injection()
        {
            // Creating DI container
            var container = ContainerFactory.Create();

            // Registration of types and objects which will be injected
            container.RegisterImplementation<ISample, Sample>();
            var instanceSample = new InstanceSample();
            container.RegisterInstance<IInstanceSample, InstanceSample>(instanceSample);

            // Creating an instance without registering in DI container
            var instanceInjectSample = container.CreateInstance<InjectSample>();
            Assert.IsNotNull(instanceInjectSample.SampleProperty);
            Assert.AreSame(instanceSample, instanceInjectSample.InstanceSampleProperty);

            // Retrieving an instance with injection
            container.RegisterImplementation<IInjectSample, InjectSample>();
            var retrievedInstanceInjectSample = container.Get<IInjectSample>();
            Assert.IsNotNull(retrievedInstanceInjectSample.SampleProperty);
            Assert.AreSame(instanceSample, retrievedInstanceInjectSample.InstanceSampleProperty);
        }
    }
}
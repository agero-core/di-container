using Agero.Core.DIContainer.Attributes;

namespace Agero.Core.DIContainer.Tests.Types
{
    public class ContainerTestClassWithUnknownInstance : IContainerTestInterfaceWithUnknownInstance
    {
        private readonly ContainerUnknownInstanceTestClass _constructorUnknownInstance;

        [Inject]
        public ContainerTestClassWithUnknownInstance(ContainerUnknownInstanceTestClass constructorUnknownInstance)
        {
            _constructorUnknownInstance = constructorUnknownInstance;
        }

        [Inject]
        public ContainerUnknownInstanceTestClass PropertyUnknownInstance { get; set; }

        public ContainerUnknownInstanceTestClass ConstructorUnknownInstance 
        {
            get { return _constructorUnknownInstance; }
        }
    }
}
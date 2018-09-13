using Agero.Core.DIContainer.Attributes;

namespace Agero.Core.DIContainer.Tests.Types
{
    public class ContainerTestInterfaceWithInjectedProperties : IContainerTestInterfaceWithInjectedProperties
    {
        [Inject]
        public IContainerTestInterface Instance { get; set; }
    }
}
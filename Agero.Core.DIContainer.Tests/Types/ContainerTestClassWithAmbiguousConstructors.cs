using Agero.Core.DIContainer.Attributes;

namespace Agero.Core.DIContainer.Tests.Types
{
    public class ContainerTestClassWithAmbiguousConstructors
    {
        [Inject]
        public ContainerTestClassWithAmbiguousConstructors(string s)
        {
        }

        [Inject]
        public ContainerTestClassWithAmbiguousConstructors(int i)
        {
        }
    }
}
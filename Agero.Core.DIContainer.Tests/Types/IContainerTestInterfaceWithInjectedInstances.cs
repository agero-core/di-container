namespace Agero.Core.DIContainer.Tests.Types
{
    public interface IContainerTestInterfaceWithInjectedInstances
    {
        IContainerTestInterface2 PropertyInjectedInstance { get; }

        IContainerTestInterface ConstructorInjectedInstance { get; }
    }
}
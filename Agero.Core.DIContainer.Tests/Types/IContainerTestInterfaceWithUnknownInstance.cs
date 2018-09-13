namespace Agero.Core.DIContainer.Tests.Types
{
    public interface IContainerTestInterfaceWithUnknownInstance
    {
        ContainerUnknownInstanceTestClass PropertyUnknownInstance { get; }

        ContainerUnknownInstanceTestClass ConstructorUnknownInstance { get; }
    }
}
namespace Agero.Core.DIContainer
{
    /// <summary>DI container factory</summary>
    public static class ContainerFactory
    {
        /// <summary>Creates a new instance of DI Container</summary>
        public static IContainer Create()
        {
            return new Container();
        }
    }
}
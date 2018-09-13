using System;

namespace Agero.Core.DIContainer.Exceptions
{
    /// <summary>DI container exception</summary>
    [Serializable]
    public sealed class ContainerException : Exception
    {
        /// <summary>Constructor</summary>
        public ContainerException(string message)
            : base(message)
        {
        }
    }
}
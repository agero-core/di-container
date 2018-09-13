using System;

namespace Agero.Core.DIContainer.Attributes
{
    /// <summary>
    /// /// Attribute which can be used on constructor with arguments or on property with setter.
    /// - For constructor it tells that DI container will use this constructor to create an instance of a type and arguments will be taken from DI container.
    /// - For property it tells to set the value of property from IoC container right after creating of a new instance of a type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InjectAttribute : Attribute
    {
    }
}
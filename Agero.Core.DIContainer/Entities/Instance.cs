using Agero.Core.Checker;

namespace Agero.Core.DIContainer.Entities
{
    internal class Instance
    {
        public Instance(object obj)
        {
            Check.ArgumentIsNull(obj, nameof(obj));

            Object = obj;
        }

        public object Object { get; }
    }
}
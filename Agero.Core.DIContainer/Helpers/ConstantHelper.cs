using System.Reflection;

namespace Agero.Core.DIContainer.Helpers
{
    internal static class ConstantHelper
    {
        public const BindingFlags BindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
    }
}
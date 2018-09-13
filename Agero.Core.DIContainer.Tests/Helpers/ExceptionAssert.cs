using System;
using Agero.Core.Checker;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Agero.Core.DIContainer.Tests.Helpers
{
    public static class ExceptionAssert
    {
        public static void Thrown<TException>(Action action, Action<TException> onExceptionThrown = null)
            where TException : Exception
        {
            Check.ArgumentIsNull(action, "action");
            
            try
            {
                action();
                Assert.Fail("Exception of type '{0}' is expected.", typeof(TException));
            }
            catch (TException ex)
            {
                onExceptionThrown?.Invoke(ex);
            }
        }

        public static void Thrown<TException>(Action action, string exceptionMessage)
            where TException : Exception
        {
            Thrown<TException>(action, ex => Assert.AreEqual(exceptionMessage, ex.Message));
        }
    }
}
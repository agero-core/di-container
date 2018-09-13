using System.Linq;
using Agero.Core.DIContainer.Helpers;
using Agero.Core.DIContainer.Tests.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Agero.Core.DIContainer.Tests
{
    [TestClass]
    public class InjectAttributeHelperTests
    {
        [TestMethod]
        public void GetConstructors()
        {
            // Arrange
            var type = typeof(InjectAttributeHelperTestClass);

            // Act
            var constructors = InjectAttributeHelper.GetConstructors(type);

            // Assert
            Assert.AreEqual(4, constructors.Count);

            var expectedParameterTypes = 
                new[]
                {
                    typeof (int), 
                    typeof (double), 
                    typeof (decimal), 
                    typeof (string)
                };
            var actualParameterTypes = constructors.Select(c => c.GetParameters().Single().ParameterType).ToArray();
            
            CollectionAssert.AreEqual(expectedParameterTypes, actualParameterTypes);
        }

        [TestMethod]
        public void GetProperties()
        {
            // Arrange
            var type = typeof(InjectAttributeHelperTestClass);

            // Act
            var properties = InjectAttributeHelper.GetProperties(type);

            // Assert
            Assert.AreEqual(7, properties.Count);

            var expectedPropertyNames =
                new[]
                {
                    nameof(InjectAttributeHelperTestClass.PublicProperty),
                    nameof(InjectAttributeHelperTestClass.PublicPropertyWithInternalSettter),
                    nameof(InjectAttributeHelperTestClass.PublicPropertyWithProtectedSettter),
                    nameof(InjectAttributeHelperTestClass.PublicPropertyWithPrivateSettter),
                    nameof(InjectAttributeHelperTestClass.InternalProperty),
                    "ProtectedProperty",
                    "PrivateProperty"
                };
            var actualPropertyNames = properties.Select(p => p.Name).ToArray();

            CollectionAssert.AreEqual(expectedPropertyNames, actualPropertyNames);
        }
    }
}
using FluentAssertions;
using Nemeio.Core.Errors;
using NUnit.Framework;

namespace Nemeio.Core.Test.Managers
{
    [TestFixture]
    public class ErrorManagerShould
    {
        [Test]
        public void ErrorManager_01_01_Constructor_WorksOk()
        {
            var errorManager = new ErrorManager();
            errorManager.ErrorStack.Should().NotBeNull();
            errorManager.ErrorStack.Should().BeEmpty();
        }
    }
}

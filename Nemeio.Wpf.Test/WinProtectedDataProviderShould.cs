using System.IO.IsolatedStorage;
using FluentAssertions;
using Nemeio.Platform.Windows.Applications;
using Nemeio.Wpf.Services;
using NUnit.Framework;

namespace Nemeio.Wpf.Test
{
    [TestFixture]
    public class WinProtectedDataProviderShould
    {
        [Test]
        public void GetScope_MustContain_UserScope() => HasScope(IsolatedStorageScope.User).Should().BeTrue();

        [Test]
        public void GetScope_MustContain_AssemblyScope() => HasScope(IsolatedStorageScope.Assembly).Should().BeTrue();

        private bool HasScope(IsolatedStorageScope scope)
        {
            var scopes = WinProtectedDataProvider.GetScope();
            var currentScope = scopes & scope;

            return currentScope != 0;
        }
    }
}

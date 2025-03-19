using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Keys.Executors;
using Nemeio.Core.Systems;
using NUnit.Framework;

namespace Nemeio.Core.Test.Keys.Executors
{
    public class UnicodeKeyExecutorShould
    {
        [Test]
        public void UnicodeKeyExecutor_Constructor_Ok()
        {
            var loggerFactory = new LoggerFactory();
            var mockSystem = Mock.Of<ISystem>();

            Assert.Throws<ArgumentNullException>(() => new UnicodeKeyExecutor(null, mockSystem, new List<string>()));
            Assert.Throws<ArgumentNullException>(() => new UnicodeKeyExecutor(loggerFactory, null, new List<string>()));
            Assert.Throws<ArgumentNullException>(() => new UnicodeKeyExecutor(loggerFactory, mockSystem, null));

            Assert.DoesNotThrow(() => new UnicodeKeyExecutor(loggerFactory, mockSystem, new List<string>()));
        }

        [Test]
        public async Task UnicodeKeyExecutor_Execute_Ok()
        {
            var pressKeyCalled = false;

            var loggerFactory = new LoggerFactory();

            var mockSystem = Mock.Of<ISystem>();
            Mock.Get(mockSystem)
                .Setup(mock => mock.PressKeys(It.IsAny<IList<string>>()))
                .Callback(() => pressKeyCalled = true);

            var executor = new UnicodeKeyExecutor(loggerFactory, mockSystem, new List<string>());

            await executor.ExecuteAsync();

            pressKeyCalled.Should().BeTrue();
        }
    }
}

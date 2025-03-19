using System;
using FluentAssertions;
using Nemeio.Core.Exceptions;
using NUnit.Framework;
using CAction = Nemeio.Core.DataModels.Configurator.KeyAction;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class ActionShould
    {
        [Test]
        public void TooLargeUnicodeItemInvokeException()
        {
            Assert.Throws<TooLargeItemException>(() =>
            {
                var action = new CAction();
                action.Display = "thisSentenceIsTooLong";
            });
        }

        [Test]
        public void EmbeddedResourcePathNotInvokeException()
        {
            var action = new CAction();

            try
            {
                action.Display = CAction.EmbeddedPrefix + "imageName.svg";
            }
            catch (Exception)
            {
                Assert.Fail("Exception occured");
            }
        }

        [Test]
        [TestCase("emb://toto.svg")]
        [TestCase("emb://windows.svg")]
        [TestCase("emb://os.svg")]
        [TestCase("emb://tata.png")]
        public void IsEmbeddedResourcePath(string val) => CAction.IsEmbeddedResource(val).Should().BeTrue();
    }
}

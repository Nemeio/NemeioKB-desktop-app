using System.Collections.Generic;
using Nemeio.Core.Exceptions;
using NUnit.Framework;
using CAction = Nemeio.Core.DataModels.Configurator.KeyAction;
using CKey = Nemeio.Core.DataModels.Configurator.Key;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class KeyShould
    {
        [Test]
        public void TooManyActions()
        {
            var actions = new List<CAction>();
            for (int i = 0; i <= CKey.MaxNumberOfActionSupported; i++)
            {
                actions.Add(new CAction());
            }

            var key = new CKey();

            Assert.Throws<TooManyActionException>(() => key.Actions = actions);
        }
    }
}

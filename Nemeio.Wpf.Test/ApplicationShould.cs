using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.Wpf.Test
{
    class ApplicationShould
    {
        [SetUp]
        public void Setup()
        {
            var app = new App();
        }

        [Test]
        public void Test()
        {
            var x = 0;
        }
    }
}

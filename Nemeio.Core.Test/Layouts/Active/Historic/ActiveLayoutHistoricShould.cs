using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Nemeio.Core.Layouts.Active.Historic;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test.Layouts.Active.Historic
{
    [TestFixture]
    public class ActiveLayoutHistoricShould
    {
        private IHistoricLog CreateLog()
        {
            var layout = Mock.Of<ILayout>();
            var randomGuid = Guid.NewGuid().ToString();

            Mock.Get(layout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId(randomGuid));

            var log = new HistoricLog(layout, HistoricActor.User);

            return log;
        }

        [Test]
        public void ActiveLayoutHistoric_Back_WhenIndexIsNotZero_ReturnLayout()
        {
            var initialLayout = CreateLog();
            var historic = new ActiveLayoutHistoric();
            historic.Insert(initialLayout);

            var lastLayout = CreateLog();
            historic.Insert(lastLayout);

            var backLayout = historic.Back();

            backLayout.Should().NotBeNull();
            backLayout.Should().Be(initialLayout);
        }

        [Test]
        public void ActiveLayoutHistoric_Forward_WhenIndexIsNotMaximum_ReturnLayout()
        {
            var initialLayout = CreateLog();
            var historic = new ActiveLayoutHistoric();
            historic.Insert(initialLayout);

            var middleLayout = CreateLog();
            var lastLayout = CreateLog();
            historic.Insert(middleLayout);
            historic.Insert(lastLayout);

            var backLayout = historic.Back();
            backLayout = historic.Forward();

            backLayout.Should().NotBeNull();
            backLayout.Should().Be(lastLayout);
        }

        [Test]
        public void ActiveLayoutHistoric_Back_WhenIndexIsZero_ReturnNull()
        {
            var initialLog = Mock.Of<IHistoricLog>();
            var historic = new ActiveLayoutHistoric();
            historic.Insert(initialLog);

            var backLayout = historic.Back();

            backLayout.Should().BeNull();
        }

        [Test]
        public void ActiveLayoutHistoric_Insert_WhenCapacityIsFull_AndNotAtMaximumIndex_RemoveAllLastLayout_AndAddNewLayout()
        {
            var historic = new ActiveLayoutHistoric();
            var initialLayout = CreateLog();
            historic.Insert(initialLayout);
            var secondLayout = CreateLog();
            historic.Insert(secondLayout);
            var thridLayout = CreateLog();
            historic.Insert(thridLayout);
            var fourthLayout = CreateLog();
            historic.Insert(fourthLayout);
            var fifthLayout = CreateLog();
            historic.Insert(fifthLayout);

            historic.Historic.Count.Should().Be(5);
            historic.Historic.Last().Should().Be(fifthLayout);

            historic.Back();
            historic.Back();

            var newLastLayout = CreateLog();
            historic.Insert(newLastLayout);

            historic.Historic.Count.Should().Be(4);
            historic.Historic.Last().Should().Be(newLastLayout);
        }

        [Test]
        public void ActiveLayoutHistoric_Insert_WhenCapacityIsNotFull_AndNotAtMaximumIndex_RemoveNothing_AndAddNewLayout()
        {
            var historic = new ActiveLayoutHistoric();
            var initialLayout = CreateLog();
            historic.Insert(initialLayout);
            var secondLayout = CreateLog();
            historic.Insert(secondLayout);
            var thridLayout = CreateLog();
            historic.Insert(thridLayout);

            historic.Historic.Count.Should().Be(3);
            historic.Historic.Last().Should().Be(thridLayout);

            var newLastLayout = CreateLog();
            historic.Insert(newLastLayout);

            historic.Historic.Count.Should().Be(4);
            historic.Historic.Last().Should().Be(newLastLayout);

            var forward = historic.Forward();
            forward.Should().BeNull();
        }

        [Test]
        public void ActiveLayoutHistoric_Insert_WhenCapacityIsFull_AndAtMaximumIndex_RemoveFirstLayout_AndAddNewLayout_AndKeepAtMaximumIndex()
        {
            var initialLayout = CreateLog();
            var historic = new ActiveLayoutHistoric();
            historic.Insert(initialLayout);

            var secondLayout = CreateLog();
            historic.Insert(secondLayout);
            var thridLayout = CreateLog();
            historic.Insert(thridLayout);
            var fourthLayout = CreateLog();
            historic.Insert(fourthLayout);
            var fifthLayout = CreateLog();
            historic.Insert(fifthLayout);

            var newLastLayout = CreateLog();
            historic.Insert(newLastLayout);

            historic.Index.Should().Be((int)historic.MaxItemCount );
            historic.Historic[historic.Index-1].Should().Be(newLastLayout);
        }

        [Test]
        public void ActiveLayoutHistoric_Insert_CantAddTwiceSameLayout_Ok()
        {
            var guid = "738CB0ED-4939-4E57-AE11-39B368C53DA0";

            var layout = Mock.Of<ILayout>();
            Mock.Get(layout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId(guid));

            var log = Mock.Of<IHistoricLog>();
            Mock.Get(log)
                .Setup(x => x.Layout)
                .Returns(layout);

            var historic = new ActiveLayoutHistoric();
            historic.Insert(log);
            historic.Insert(log);

            historic.Historic.Count.Should().Be(1);
        }

        [Test]
        public void ActiveLayoutHistoric_Remove_WhenLayoutIsNotPresent_DoNothing()
        {
            var initialLayout = CreateLog();
            var historic = new ActiveLayoutHistoric();
            historic.Insert(initialLayout);

            var newLayout = Mock.Of<ILayout>();
            var randomGuid = Guid.NewGuid().ToString();

            Mock.Get(newLayout)
                .Setup(x => x.LayoutId)
                .Returns(new LayoutId(randomGuid));

            Assert.DoesNotThrow(() => historic.Remove(newLayout));
        }

        [Test]
        public void ActiveLayoutHistoric_Remove_WhenLayoutIsPresent_RemoveFromHistoric()
        {   
            var historic = new ActiveLayoutHistoric();

            var initialLayout = CreateLog();
            historic.Insert(initialLayout);
            var secondLayout = CreateLog();
            historic.Insert(secondLayout);
            var thridLayout = CreateLog();
            historic.Insert(thridLayout);

            historic.Historic.Count.Should().Be(3);

            historic.Remove(thridLayout.Layout);

            historic.Historic.Count.Should().Be(0);
            historic.Index.Should().Be(0);
        }
    }
}

using System.Linq;
using FluentAssertions;
using Moq;
using Nemeio.Core.Enums;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Converters;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test.Modes
{
    public partial class LGClassicLayoutConverterShould_ShouldAddKey : LayoutConverterShould
    {
        private ILayoutConverter _layoutConverter;

        [SetUp]
        public void SetUp()
        {
            var mockDocument = Mock.Of<IDocument>();

            _layoutConverter = new LGClassicLayoutConverter(mockDocument);
        }

        #region Single disposition

        [TestCase(KeyboardModifier.None)]
        public void LGClassicLayoutConverter_ShouldAddKey_WhenDispositionIsSingle_AndModifierIsSupported_AddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        [TestCase(KeyboardModifier.Shift)]
        [TestCase(KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Shift | KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Function)]
        [TestCase(KeyboardModifier.Capslock)]
        public void LGClassicLayoutConverter_ShouldAddKey_WhenDispositionIsSingle_AndModifierIsNotSupported_NotAddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(0);
        }

        #endregion

        #region Double disposition

        [TestCase(KeyboardModifier.None)]
        [TestCase(KeyboardModifier.Shift)]
        public void LGClassicLayoutConverter_ShouldAddKey_WhenDispositionIsDouble_AndModifierIsSupported_AddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.Double;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        [TestCase(KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Shift | KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Function)]
        [TestCase(KeyboardModifier.Capslock)]
        public void LGClassicLayoutConverter_ShouldAddKey_WhenDispositionIsDouble_AndModifierIsNotSupported_NotAddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.Double;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(0);
        }

        #endregion

        #region Full disposition

        [TestCase(KeyboardModifier.None)]
        [TestCase(KeyboardModifier.Shift)]
        [TestCase(KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Shift | KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Function)]
        public void LGClassicLayoutConverter_ShouldAddKey_WhenDispositionIsFull_AndModifierIsSupported_AddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.Full;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        #endregion

        #region TShape disposition

        [TestCase(KeyboardModifier.None)]
        [TestCase(KeyboardModifier.Shift)]
        [TestCase(KeyboardModifier.AltGr)]
        public void LGClassicLayoutConverter_ShouldAddKey_WhenDispositionIsTShape_AndModifierIsSupported_AddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.TShape;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        [TestCase(KeyboardModifier.Shift | KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Function)]
        [TestCase(KeyboardModifier.Capslock)]
        public void LGClassicLayoutConverter_ShouldAddKey_WhenDispositionIsTShape_AndModifierIsNotSupported_NotAddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.TShape;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(0);
        }

        #endregion
    }
}

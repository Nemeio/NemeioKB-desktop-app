using System.Linq;
using FluentAssertions;
using Moq;
using Nemeio.Core.Enums;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Converters;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test.Modes
{
    [TestFixture]
    public sealed class LGBoldLayoutConverterShould : LayoutConverterShould
    {
        private ILayoutConverter _layoutConverter;

        [SetUp]
        public void SetUp()
        {
            var mockDocument = Mock.Of<IDocument>();

            _layoutConverter = new LGBoldLayoutConverter(mockDocument);
        }

        #region Single disposition

        [Test]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsNone_AndDispositionIsSingle_ActionIsNone_AddKey_Ok()
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, imageModifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        [TestCase(KeyboardModifier.Shift)]
        [TestCase(KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Shift | KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Function)]
        [TestCase(KeyboardModifier.Capslock)]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsNone_AndDispositionIsSingle_ActionIsNotNone_DontAddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(0);
        }

        [Test]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsShift_AndDispositionIsSingle_ActionIsShift_AddKey_Ok()
        {
            const KeyboardModifier imageModifier = KeyboardModifier.Shift;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, imageModifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        [TestCase(KeyboardModifier.None)]
        [TestCase(KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Shift | KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Function)]
        [TestCase(KeyboardModifier.Capslock)]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsShift_AndDispositionIsSingle_ActionIsNotShift_DontAddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.Shift;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(0);
        }

        [Test]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsAltGr_AndDispositionIsSingle_ActionIsAltGr_AddKey_Ok()
        {
            const KeyboardModifier imageModifier = KeyboardModifier.AltGr;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, imageModifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        [TestCase(KeyboardModifier.Shift)]
        [TestCase(KeyboardModifier.None)]
        [TestCase(KeyboardModifier.Shift | KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Function)]
        [TestCase(KeyboardModifier.Capslock)]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsAltGr_AndDispositionIsSingle_ActionIsNotAltGr_DontAddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.AltGr;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(0);
        }

        [Test]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsShiftAltGr_AndDispositionIsSingle_ActionIsShiftAltGr_AddKey_Ok()
        {
            const KeyboardModifier imageModifier = KeyboardModifier.Shift | KeyboardModifier.AltGr;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, imageModifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        [TestCase(KeyboardModifier.Shift)]
        [TestCase(KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.None)]
        [TestCase(KeyboardModifier.Function)]
        [TestCase(KeyboardModifier.Capslock)]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsShiftAltGr_AndDispositionIsSingle_ActionIsNotShiftAltGr_DontAddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.Shift | KeyboardModifier.AltGr;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(0);
        }

        [Test]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsFunction_AndDispositionIsSingle_ActionIsFunction_AddKey_Ok()
        {
            const KeyboardModifier imageModifier = KeyboardModifier.Function;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, imageModifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        [TestCase(KeyboardModifier.Shift)]
        [TestCase(KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Shift | KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.None)]
        [TestCase(KeyboardModifier.Capslock)]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenModifierIsFunction_AndDispositionIsSingle_ActionIsNotFunction_DontAddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.Function;
            const KeyDisposition disposition = KeyDisposition.Single;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(0);
        }

        #endregion

        #region Double disposition

        [TestCase(KeyboardModifier.None)]
        [TestCase(KeyboardModifier.Shift)]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenDispositionIsDouble_AndModifierIsSupported_AddKey_Ok(KeyboardModifier modifier)
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
        public void LGBoldLayoutConverter_ShouldAddKey_WhenDispositionIsDouble_AndModifierIsNotSupported_NotAddKey_Ok(KeyboardModifier modifier)
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
        public void LGBoldLayoutConverter_ShouldAddKey_WhenDispositionIsFull_AndModifierIsSupported_AddKey_Ok(KeyboardModifier modifier)
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
        public void LGBoldLayoutConverter_ShouldAddKey_WhenDispositionIsTShape_AndModifierIsSupported_AddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.TShape;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(1);
        }

        [TestCase(KeyboardModifier.Shift | KeyboardModifier.AltGr)]
        [TestCase(KeyboardModifier.Function)]
        [TestCase(KeyboardModifier.Capslock)]
        public void LGBoldLayoutConverter_ShouldAddKey_WhenDispositionIsTShape_AndModifierIsNotSupported_NotAddKey_Ok(KeyboardModifier modifier)
        {
            const KeyboardModifier imageModifier = KeyboardModifier.None;
            const KeyDisposition disposition = KeyDisposition.TShape;

            var layout = CreateLayout(disposition, modifier, imageModifier, _layoutConverter);

            layout.Keys.First().Subkeys.Count.Should().Be(0);
        }

        #endregion
    }
}

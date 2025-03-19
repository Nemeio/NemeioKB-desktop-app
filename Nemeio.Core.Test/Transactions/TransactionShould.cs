using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Transactions;
using NUnit.Framework;

namespace Nemeio.Core.Test.Transactions
{
    /// <summary>
    /// The unique goal of this class is to test Backup implementation
    /// </summary>
    public class SimpleTestObject : IBackupable<SimpleTestObject>
    {
        public int IntValue { get; set; }

        public string StringValue { get; set; }

        public byte[] ByteArrayValue { get; set; }

        public bool BoolValue { get; set; }

        public List<string> NullableValue { get; set; }

        public SimpleTestObject CreateBackup()
        {
            var byteArrayCopy = new byte[ByteArrayValue.Length];

            Array.Copy(ByteArrayValue, 0, byteArrayCopy, 0, ByteArrayValue.Length);

            return new SimpleTestObject()
            {
                IntValue = IntValue,
                StringValue = StringValue,
                ByteArrayValue = byteArrayCopy,
                BoolValue = BoolValue,
                NullableValue = NullableValue == null ? null : new List<string>(NullableValue)
            };
        }
    }

    public class TransactionShould
    {
        [Test]
        public void Transaction_01_01_Run_WithoutException_WorksOk()
        {
            var simpleTestObject = new SimpleTestObject()
            {
                IntValue = 54,
                StringValue = "this_is_a_test_string",
                ByteArrayValue = new byte[2] { 0x03, 0x9F },
                BoolValue = true,
                NullableValue = null
            };

            var loggerFactory = new LoggerFactory();
            var transaction = new Transaction<SimpleTestObject>(loggerFactory);

            const string newStringValue     = "new string value";
            const int newIntValue           = 59;

            transaction.Run(ref simpleTestObject, () => 
            {
                simpleTestObject.StringValue = newStringValue;
                simpleTestObject.IntValue = newIntValue;
            });

            simpleTestObject.StringValue.Should().Be(newStringValue);
            simpleTestObject.IntValue.Should().Be(newIntValue);
        }

        [Test]
        public void Transaction_01_02_Run_WithException_RollbackObject_WorksOk()
        {
            const string initialStringValue = "this_is_a_test_string";
            const int initialIntValue = 52;

            var simpleTestObject = new SimpleTestObject()
            {
                IntValue = initialIntValue,
                StringValue = initialStringValue,
                ByteArrayValue = new byte[2] { 0x03, 0x9F },
                BoolValue = true,
                NullableValue = null
            };

            var loggerFactory = new LoggerFactory();
            var transaction = new Transaction<SimpleTestObject>(loggerFactory);

            const string newStringValue = "new string value";

            try
            {
                transaction.Run(ref simpleTestObject, () =>
                {
                    simpleTestObject.StringValue = newStringValue;

                    throw new InvalidOperationException("An error");
                });
            }
            catch (Exception)
            {
                //  When an error occurred, transaction always propagate it
            }

            simpleTestObject.StringValue.Should().Be(initialStringValue);
            simpleTestObject.IntValue.Should().Be(initialIntValue);
        }

        [Test]
        public void Transaction_01_03_Run_WithException_OnLayout_RollbackObject_WorksOk()
        {
            var initialLayoutIsHid = true;
            var initialLayoutOsLayoutId = new OsLayoutId("456826");
            var initialLayoutTitle = "My new layout";
            var initialLayoutSubtitle = "Subtitle My new layout";
            var initialLayoutImage = new byte[5] { 0x45, 0x78, 0xFF, 0x0F, 0x00 };
            var initialLayoutCategory = 123;
            var initialLinkPath = new List<string>() { "C:/Program Files/Fake Folder/FakeProgram.exe" };
            var initialLinkEnable = false;
            var initialDarkMode = HexColor.Black;
            var initialIsTemplate = true;

            var screen = Mock.Of<IScreen>();
            var layoutImageInfo = new LayoutImageInfo(
                initialDarkMode,
                FontProvider.GetDefaultFont(),
                screen
            );

            ILayout layout = new Layout(
                new LayoutInfo(initialLayoutOsLayoutId, false, initialLayoutIsHid, initialLinkPath, initialLinkEnable, initialIsTemplate),
                layoutImageInfo,
                initialLayoutImage,
                initialLayoutCategory,
                0,
                initialLayoutTitle,
                initialLayoutSubtitle,
                DateTime.Now,
                DateTime.Now,
                new List<Key>(),
                new LayoutId("B40259A4-3DEB-4061-ADFD-A2765298096B"),
                null,
                false,
                true
            );

            var loggerFactory = new LoggerFactory();
            var transaction = new Transaction<ILayout>(loggerFactory);

            try
            {
                transaction.Run(ref layout, () =>
                {
                    layout.Title = "My new layout title";
                    layout.Image = new byte[5] { 0xFF, 0x99, 0x33, 0x49, 0x00 };
                    layout.CategoryId = 456;
                    layout.LayoutImageInfo.Color = HexColor.White;
                    layout.LayoutInfo.IsTemplate = !initialIsTemplate;
                    layout.LayoutInfo.LinkApplicationEnable = !initialLinkEnable;
                    layout.LayoutInfo.LinkApplicationPaths = new List<string>() { "/Applications/Fork.app" };

                    throw new FakeTransactionException("An error");
                });
            }
            catch (FakeTransactionException)
            {
                //  When an error occurred, transaction always propagate it
            }

            layout.LayoutInfo.Hid.Should().Be(initialLayoutIsHid);
            layout.Title.Should().Be(initialLayoutTitle);
            layout.Image.Should().BeEquivalentTo(initialLayoutImage);
            layout.CategoryId.Should().Be(initialLayoutCategory);
            layout.LayoutImageInfo.Color.Should().Be(initialDarkMode);
            layout.LayoutInfo.IsTemplate.Should().Be(initialIsTemplate);
            layout.LayoutInfo.LinkApplicationEnable.Should().Be(initialLinkEnable);
            layout.LayoutInfo.LinkApplicationPaths.Count().Should().Be(initialLinkPath.Count);

            for (var i = 0; i <= layout.LayoutInfo.LinkApplicationPaths.Count() - 1; i++)
            {
                var current = layout.LayoutInfo.LinkApplicationPaths.ElementAt(i);
                var original = initialLinkPath.ElementAt(i);

                current.Equals(original, StringComparison.InvariantCultureIgnoreCase).Should().BeTrue();
            }
        }
    }

    internal class FakeTransactionException : Exception
    {
        public FakeTransactionException(string message) 
            : base(message) { }
    }
}

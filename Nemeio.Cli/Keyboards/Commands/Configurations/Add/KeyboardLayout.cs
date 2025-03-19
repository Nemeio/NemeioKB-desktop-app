using System;
using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Images;
using Nemeio.Core.Images.Builders;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Models.LayoutWarning;
using Nemeio.Core.Services.Layouts;
using Nemeio.Tools.Core.Files;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Add
{
    internal sealed class KeyboardLayout : ILayout
    {
        /// <summary>
        /// Don't provide real implementation
        /// The goal is just to hold image bpp
        /// </summary>
        private sealed class StaticImageFormat : ImageFormat
        {
            public StaticImageFormat(int bpp)
                : base(bpp) { }
        }

        private sealed class MinimalistScreen : IScreen
        {
            //  Not need here
            //  Must never be called
            public KeyboardMap Map => throw new NotImplementedException();

            //  Not need here
            //  Must never be called
            public IImageBuilder Builder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            //  Not need here
            //  Must never be called
            public ScreenType Type => throw new NotImplementedException();

            //  Not need here
            //  Must never be called
            public ImageFormat Format { get; private set; }

            public List<NemeioIndexKeystroke> TransformKeystrokes(IEnumerable<NemeioIndexKeystroke> keystrokes)
            {
                //  Not need here
                //  Must never be called
                throw new NotImplementedException();
            }

            public MinimalistScreen(int imageBpp)
            {
                Format = new StaticImageFormat(imageBpp);
            }
        }

        public LayoutId LayoutId { get; set; }
        public LayoutHash Hash { get; set; }
        public LayoutId AssociatedLayoutId { get; set; }
        public LayoutInfo LayoutInfo { get; set; }
        public LayoutImageInfo LayoutImageInfo { get; set; }
        public int Index { get; set; }
        public byte[] Image { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool Enable { get; set; }
        public SpecialSequences SpecialSequences { get; set; }
        public Font Font { get; set; }
        public bool IsDefault { get; set; }
        public List<Key> Keys { get; set; }
        public IEnumerable<LayoutWarning> Warnings { get; set; }
        public int Order { get; set; }
        public LayoutId OriginalAssociatedLayoutId { get; set; }

        public KeyboardLayout(LayoutFileDto fileDto, byte[] image)
        {
            LayoutId = new LayoutId(fileDto.Id);
            Hash = new LayoutHash(fileDto.Id);
            LayoutInfo = new LayoutInfo(
                OsLayoutId.Empty,
                fileDto.Factory.Equals("1"),
                fileDto.IsHid.Equals("1")
            );
            AssociatedLayoutId = string.IsNullOrWhiteSpace(fileDto.AssociatedId) ? null : new LayoutId(fileDto.AssociatedId);
            Image = image;
            SpecialSequences = SpecialSequences.Empty;
            LayoutImageInfo = new LayoutImageInfo(
                Core.Layouts.Color.HexColor.Black,
                FontProvider.GetDefaultFont(),
                new MinimalistScreen(fileDto.ImageBpp),
                fileDto.DisableModifiers.Equals("1") ? LayoutImageType.Classic : LayoutImageType.Hide
            );
        }

        public void CalculateImageHash() { }
        public ILayout CreateBackup() => null;
    }
}

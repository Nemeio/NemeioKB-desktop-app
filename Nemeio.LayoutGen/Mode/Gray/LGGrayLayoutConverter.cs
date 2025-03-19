using System;
using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Extensions;
using Nemeio.Core.Images;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Converters
{
    internal class LGGrayLayoutConverter : LGLayoutConverter
    {
        internal LGGrayLayoutConverter(IDocument documentService)
            : base(documentService) { }

        public override LGLayout Convert(KeyboardModifier modifier, IEnumerable<Key> keys, Font mainFont, bool isDark, IDeviceMap deviceMap, ImageAdjustment imageAdjustment)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            var layout = new LGLayout(
                ApplyAdjustmentToLayoutPosition(imageAdjustment),
                deviceMap.DeviceSize,
                mainFont,
                isDark ? SKColors.Black : SKColors.White
            );

            foreach (var key in keys)
            {
                var uiKey = _keyFactory.CreateKey(layout, key, deviceMap);

                foreach (var subKey in CreateSubKeys(key, uiKey, deviceMap, modifier))
                {
                    uiKey.Subkeys.Add(subKey);
                }

                layout.Keys.Add(uiKey);
            }

            return layout;
        }

        private IList<LGSubkey> CreateSubKeys(Key key, LGKey graphicKey, IDeviceMap deviceMap, KeyboardModifier modifier)
        {
            var subKeys = new List<LGSubkey>();

            foreach (var action in key.Actions)
            {
                //  We show capslock only for capslock image
                if (action.Modifier == KeyboardModifier.Capslock && modifier != KeyboardModifier.Capslock)
                {
                    continue;
                }

                if (ActionMustBeByPassed(key, action, deviceMap))
                {
                    //  Nothing must be drawn for this action

                    continue;
                }

                if (!ShouldAddKey(key, action, modifier))
                {
                    continue;
                }

                var disposition = GetDisposition(action.Modifier);
                var isHighlighted = modifier == KeyboardModifier.None ? true : action.Modifier == modifier;
                var subKey = CreateSubKey(graphicKey, action, disposition, isHighlighted);

                subKeys.Add(subKey);
            }

            return subKeys;
        }

        private LGSubkey CreateSubKey(LGKey key, KeyAction action, LGSubKeyDispositionArea disposition, bool isHighlighted)
        {
            var resource = action.Display;
            var foregroundColor = GetColor(action, key.Parent.BackgroundColor, isHighlighted);

            if (!resource.IsFileRef())
            {
                return new LGTextSubkey(key, disposition, resource, foregroundColor);
            }
            else
            {
                var imageStream = _imageProvider.GetImageStream(resource);
                foregroundColor = ComputeForegroundColor(key, action);
                return new LGImageSubkey(key, disposition, resource, foregroundColor, imageStream);
            }
        }

        protected override bool ShouldAddKey(Key key, KeyAction action, KeyboardModifier modifier)
        {
            switch (key.Disposition)
            {
                case KeyDisposition.Single:
                    return action.Modifier == modifier;
                case KeyDisposition.Double:
                    return action.Modifier == KeyboardModifier.None ||
                           action.Modifier == KeyboardModifier.Shift;
                case KeyDisposition.Full:
                    return
                        action.Modifier == KeyboardModifier.None ||
                        action.Modifier == KeyboardModifier.Shift ||
                        action.Modifier == KeyboardModifier.AltGr ||
                        action.Modifier == (KeyboardModifier.Shift | KeyboardModifier.AltGr) ||
                        action.Modifier == KeyboardModifier.Function;
                case KeyDisposition.TShape:
                    return
                        action.Modifier == KeyboardModifier.None ||
                        action.Modifier == KeyboardModifier.Shift ||
                        action.Modifier == KeyboardModifier.AltGr;
                default:
                    throw new ArgumentNullException($"Not supported disposition <{key.Disposition}>");
            }
        }

        private SKColor GetColor(KeyAction action, SKColor backgroundColor, bool isHighlighted)
        {
            SKColor color = SKColor.Empty;

            if (backgroundColor == SKColors.Black)
            {
                if (isHighlighted)
                {
                    return color == SKColor.Empty ? SKColors.White : color;
                }
                return SKColor.Parse("#FF555555");
            }
            else
            {
                if (isHighlighted)
                {
                    return color == SKColor.Empty ? SKColors.Black : color;
                }
                return SKColor.Parse("#FFAAAAAA");
            }
        }
    }
}

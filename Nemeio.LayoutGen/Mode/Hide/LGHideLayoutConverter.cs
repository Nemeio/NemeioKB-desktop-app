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
    internal class LGHideLayoutConverter : LGLayoutConverter
    {
        internal LGHideLayoutConverter(IDocument documentService)
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

                foreach (var action in key.Actions)
                {
                    //  We show capslock only for capslock image
                    if (action.Modifier == KeyboardModifier.Capslock && modifier != KeyboardModifier.Capslock)
                    {
                        continue;
                    }

                    //  Create only subKey which match with current modifier
                    //  Except for None modifier
                    if (modifier != KeyboardModifier.None && action.Modifier != modifier)
                    {
                        continue;
                    }

                    //  In some cases (function keys, ...) key must be bypassed
                    if (ActionMustBeByPassed(key, action, deviceMap))
                    {
                        continue;
                    }

                    if (!ShouldAddKey(key, action, modifier))
                    {
                        continue;
                    }

                    var disposition = GetDisposition(action.Modifier);
                    var subKey = CreateSubKey(uiKey, action, disposition);

                    uiKey.Subkeys.Add(subKey);
                }

                layout.Keys.Add(uiKey);
            }

            return layout;
        }

        private LGSubkey CreateSubKey(LGKey key, KeyAction action, LGSubKeyDispositionArea disposition)
        {
            var resource = action.Display;
            var foregroundColor = ComputeForegroundColor(key, action);

            if (!resource.IsFileRef())
            {
                return new LGTextSubkey(key, disposition, resource, foregroundColor);
            }
            else
            {
                var imageStream = _imageProvider.GetImageStream(resource);

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
                    return 
                        action.Modifier == KeyboardModifier.None ||
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
    }
}

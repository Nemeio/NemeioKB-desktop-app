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
    internal class LGBoldLayoutConverter : LGLayoutConverter
    {
        internal LGBoldLayoutConverter(IDocument documentService)
            : base(documentService) { }

        public override LGLayout Convert(KeyboardModifier modifier, IEnumerable<Key> keys, Font mainFont, bool isDark, IDeviceMap deviceMap, ImageAdjustment imageAdjustment)
        {
            //  In this case, all modifier create the same image

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

                foreach (var subKey in CreateSubKeys(modifier, key, uiKey, deviceMap))
                {
                    uiKey.Subkeys.Add(subKey);
                }

                layout.Keys.Add(uiKey);
            }

            return layout;
        }

        private IList<LGSubkey> CreateSubKeys(KeyboardModifier modifier, Key key, LGKey graphicKey, IDeviceMap deviceMap)
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
                var isHighlighted = action.Modifier == modifier;

                var subKey = CreateSubKey(graphicKey, action, disposition, isHighlighted);

                subKeys.Add(subKey);
            }

            return subKeys;
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

        private LGSubkey CreateSubKey(LGKey key, KeyAction action, LGSubKeyDispositionArea disposition, bool isHighlighted)
        {
            var resource = action.Display;
            var foregroundColor = ComputeForegroundColor(key, action);

            if (!resource.IsFileRef())
            {
                return new LGTextSubkey(key, disposition, resource, foregroundColor, isHighlighted);
            }
            else
            {
                var imageStream = _imageProvider.GetImageStream(resource);

                return new LGImageSubkey(key, disposition, resource, foregroundColor, imageStream, isHighlighted);
            }
        }
    }
}

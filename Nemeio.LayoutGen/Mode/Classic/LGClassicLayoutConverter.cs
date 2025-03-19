using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Extensions;
using Nemeio.Core.Images;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Converters
{
    internal class LGClassicLayoutConverter : LGLayoutConverter
    {
        internal LGClassicLayoutConverter(IDocument documentService)
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
                //  In classic mode, we never show Capslock modifier
                if (action.Modifier == KeyboardModifier.Capslock)
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
                var subKey = CreateSubKey(graphicKey, action, disposition, IsLetterKey(key));

                subKeys.Add(subKey);
            }

            return subKeys;
        }

        private LGSubkey CreateSubKey(LGKey key, KeyAction action, LGSubKeyDispositionArea disposition, bool isLetter = false)
        {
            var resource = action.Display;
            var foregroundColor = ComputeForegroundColor(key, action);

            if (!resource.IsFileRef())
            {
                //  On classic mode we want all letter key to display in upper case for none
                if (isLetter)
                {
                    resource = resource.ToUpper();
                }

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
                    return action.Modifier == KeyboardModifier.None;
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

        private bool IsLetterKey(Key key)
        {
            var noneKey = key.Actions.FirstOrDefault(x => x.Modifier == KeyboardModifier.None);
            var shiftKey = key.Actions.FirstOrDefault(x => x.Modifier == KeyboardModifier.Shift);

            if (noneKey != null && shiftKey != null && !string.IsNullOrEmpty(shiftKey.Display))
            {
                return noneKey.Display.ToUpper().Equals(shiftKey.Display);
            }

            return false;
        }
    }
}

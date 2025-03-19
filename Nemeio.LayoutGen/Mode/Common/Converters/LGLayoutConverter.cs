using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Images;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Factory;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Converters
{
    internal abstract class LGLayoutConverter : ILayoutConverter
    {
        protected readonly LGImageProvider _imageProvider;
        protected readonly LGKeyFactory _keyFactory;

        internal LGLayoutConverter(IDocument documentService)
        {
            if (documentService == null)
            {
                throw new ArgumentNullException(nameof(documentService));
            }

            _keyFactory = new LGKeyFactory();
            _imageProvider = new LGImageProvider(documentService);
        }

        public abstract LGLayout Convert(KeyboardModifier modifier, IEnumerable<Key> keys, Font mainFont, bool isDark, IDeviceMap deviceMap, ImageAdjustment imageAdjustment);

        protected LGPosition ApplyAdjustmentToLayoutPosition(ImageAdjustment imageAdjustment)
        {
            if (imageAdjustment == null)
            {
                return LGPosition.Zero;
            }

            var adjustedPosition = new LGPosition(imageAdjustment.X, imageAdjustment.Y);

            return adjustedPosition;
        }

        protected LGSubKeyDispositionArea GetDisposition(KeyboardModifier modifier)
        {
            switch (modifier)
            {
                case KeyboardModifier.None:
                    return LGSubKeyDispositionArea.None;
                case KeyboardModifier.Shift:
                case KeyboardModifier.Capslock:
                    return LGSubKeyDispositionArea.Shift;
                case KeyboardModifier.AltGr:
                    return LGSubKeyDispositionArea.AltGr;
                case KeyboardModifier.Shift | KeyboardModifier.AltGr:
                case KeyboardModifier.Function:
                    return LGSubKeyDispositionArea.ShiftAndAltGr;
                default:
                    throw new InvalidOperationException("Not supported value");
            }
        }

        protected bool ActionMustBeByPassed(Key key, KeyAction keyAction, IDeviceMap deviceMap)
        {
            var hasFunction = key.Actions.Any(x => x.Modifier == KeyboardModifier.Function);
            var firstLineKeysBannedModifiers = new List<KeyboardModifier>()
            {
                KeyboardModifier.Shift,
                KeyboardModifier.AltGr,
                KeyboardModifier.Shift | KeyboardModifier.AltGr
            };

            if (deviceMap.IsFirstLineKey(key.Index) && firstLineKeysBannedModifiers.Contains(keyAction.Modifier))
            {
                //  For first line keys, we only show 'None' and 'Function' modifiers

                return true;
            }

            if (hasFunction && keyAction.Modifier == (KeyboardModifier.Shift | KeyboardModifier.AltGr))
            {
                //  For keys which has functions we never show Shift + AltGr

                return true;
            }

            return false;
        }

        protected virtual bool ShouldAddKey(Key key, KeyAction action, KeyboardModifier modifier)
        {
            switch (key.Disposition)
            {
                case KeyDisposition.Single:
                    return true;
                case KeyDisposition.Double:
                    return action.Modifier == KeyboardModifier.None ||
                           action.Modifier == KeyboardModifier.Shift;
                case KeyDisposition.Full:
                    return true;
                case KeyDisposition.TShape:
                    return
                        action.Modifier == KeyboardModifier.None ||
                        action.Modifier == KeyboardModifier.Shift ||
                        action.Modifier == KeyboardModifier.AltGr;
                default:
                    throw new ArgumentNullException($"Not supported disposition <{key.Disposition}>");
            }
        }

        protected virtual SKColor ComputeForegroundColor(LGKey key, KeyAction action)
        {
            if (action.IsGrey)
            {
                return key.Parent.BackgroundColor == SKColors.Black ? SKColor.Parse("#FF555555") : SKColor.Parse("#FFAAAAAA");
            }
            else
            {
                return key.Parent.BackgroundColor == SKColors.Black ? SKColors.White : SKColors.Black;
            }
        }
    }
}

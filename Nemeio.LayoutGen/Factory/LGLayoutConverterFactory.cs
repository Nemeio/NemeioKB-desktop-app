using System;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Converters;

namespace Nemeio.LayoutGen.Factory
{
    internal class LGLayoutConverterFactory
    {
        private IDocument _documentService;

        public LGLayoutConverterFactory(IDocument documentService)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        internal ILayoutConverter CreateLayoutConverter(LayoutImageType layoutImageType)
        {
            switch (layoutImageType)
            {
                case LayoutImageType.Classic: 
                    return new LGClassicLayoutConverter(_documentService);
                case LayoutImageType.Hide:
                    return new LGHideLayoutConverter(_documentService);
                case LayoutImageType.Gray:
                    return new LGGrayLayoutConverter(_documentService);
                case LayoutImageType.Bold:
                    return new LGBoldLayoutConverter(_documentService);
                default:
                    throw new InvalidOperationException($"Unknow layout image type <{layoutImageType}>");
            }
        }
    }
}

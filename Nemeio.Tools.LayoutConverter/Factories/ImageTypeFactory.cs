using System;
using System.Collections.Generic;
using Nemeio.Tools.LayoutConverter.Models;

namespace Nemeio.Tools.LayoutConverter.Factories
{
    internal class ImageTypeFactory
    {
        internal const string GrayImageName = "gray";
        internal const string HideImageName = "hide";
        internal const string ClassicImageName = "classic";
        internal const string BoldImageName = "bold";

        internal ImageType CreateImageType(string imageType)
        {
            if (string.IsNullOrWhiteSpace(imageType))
            {
                throw new ArgumentNullException(nameof(imageType));
            }

            switch (imageType.ToLower())
            {
                case GrayImageName:
                    return new ImageType(GrayImageName, new List<string>() { Constantes.NoneModifier, Constantes.ShiftModifier, Constantes.AltGrModifier, Constantes.ShiftAltGrModifier, Constantes.CapslockModifier, Constantes.FunctionModifier }) ;
                case HideImageName:
                    return new ImageType(HideImageName, new List<string>() { Constantes.NoneModifier, Constantes.ShiftModifier, Constantes.AltGrModifier, Constantes.ShiftAltGrModifier, Constantes.CapslockModifier, Constantes.FunctionModifier });
                case ClassicImageName:
                    return new ImageType(ClassicImageName, new List<string>() { Constantes.NoneModifier });
                case BoldImageName:
                    return new ImageType(BoldImageName, new List<string>() { Constantes.NoneModifier, Constantes.ShiftModifier, Constantes.AltGrModifier, Constantes.ShiftAltGrModifier, Constantes.CapslockModifier, Constantes.FunctionModifier });
                default:
                    throw new InvalidOperationException($"<{imageType}> is not supported");
            }
        }
    }
}

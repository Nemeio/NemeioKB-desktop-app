using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Api.Dto.In.Layout;
using Nemeio.Api.Exceptions;
using Nemeio.Core;
using Nemeio.Core.Images;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Api.PatchApplier
{
    public enum LayoutPatchError
    {
        EmptyLayoutName = 0,
        LayoutNameAlreadyUsed = 1,
        NotAnExecutable = 2,
        InvalidPath = 3,
        InvalidLayoutAssociatedId = 4,
        InvalidLayoutFont = 5
    }

    public class LayoutPatchApplier : BasePatchApplier<PutLayoutApiInDto, ILayout>
    {
        private readonly ILayoutImageGenerator _layoutGenService;
        private readonly IApplicationLayoutManager _applicationLayoutManager;
        private readonly IFontProvider _fontProvider;
        private readonly ILayoutLibrary _library;

        public LayoutPatchApplier(ILoggerFactory loggerFactory, ILayoutImageGenerator layoutGenService, ILayoutLibrary library, IApplicationLayoutManager applicationLayoutManager, IFontProvider fontProvider)
            : base(loggerFactory)
        {
            _layoutGenService = layoutGenService ?? throw new ArgumentNullException(nameof(layoutGenService));
            _applicationLayoutManager = applicationLayoutManager ?? throw new ArgumentNullException(nameof(applicationLayoutManager));
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
        }

        public override ILayout Patch(PutLayoutApiInDto input, ILayout currentValue)
        {
            if (input == null || currentValue == null)
            {
                throw new ArgumentNullException("<input> or <currentValue> parameter is null");
            }

            _transaction.Run(ref currentValue, () =>
            {
                if (input.Index.IsValuePresent)
                {
                    currentValue.Index = input.Index.Value;
                }

                if (input.Title.IsValuePresent)
                {
                    var newTitle = input.Title.Value;

                    if (string.IsNullOrEmpty(newTitle))
                    {
                        throw new PatchFailedException((int)LayoutPatchError.EmptyLayoutName, "Layout name can't be empty");
                    }

                    var layoutWithSameTitle = _library.Layouts
                                                    .Any(x => !x.LayoutId.Equals(currentValue.LayoutId) &&
                                                        x.Title.Equals(newTitle, StringComparison.OrdinalIgnoreCase));

                    if (layoutWithSameTitle)
                    {
                        throw new PatchFailedException((int)LayoutPatchError.LayoutNameAlreadyUsed, "Layout name is already used for another layout");
                    }

                    currentValue.Title = newTitle;
                }

                if (input.AssociatedId.IsValuePresent && !string.IsNullOrWhiteSpace(input.AssociatedId.Value))
                {
                    var associatedLayoutId = new LayoutId(input.AssociatedId.Value);
                    var newAssociatedLayout = _library.Layouts.FirstOrDefault(x => x.LayoutId == associatedLayoutId);

                    if (System.AppContext.TryGetSwitch("ISMACOS", out bool ISMACOS) && ISMACOS)
                    {
                        currentValue.Subtitle = newAssociatedLayout?.Title;
                    }
                    else
                    {
                        currentValue.Subtitle = newAssociatedLayout?.Subtitle;
                    }
                }

                if (input.CategoryId.IsValuePresent)
                {
                    currentValue.CategoryId = input.CategoryId.Value;
                }

                if (!currentValue.LayoutInfo.Hid)
                {
                    currentValue.Enable = input.Enable.Value && !currentValue.Enable;
                }

                if (input.LinkApplicationPath != null)
                {
                    var newLinks = new List<string>();
                    input.LinkApplicationPath.ForEach((rawpath) =>
                    {
                        var path = rawpath.Replace(">>", @":\").Replace(">", @"\").Replace(".zip", string.Empty);

                        try
                        {
                            var assocLayout = _applicationLayoutManager.GetLayoutByLinkedApplicationPath(path);
                            if (assocLayout != null && !assocLayout.LayoutId.Equals(currentValue.LayoutId))
                            {
                                throw new PatchFailedException($"The application path <{path}> is already associated to <{assocLayout.LayoutId}>");
                            }

                            var hasValidExtension = FileHelpers.IsValidExecutableExtension(path);
                            if (!hasValidExtension)
                            {
                                throw new PatchFailedException((int)LayoutPatchError.NotAnExecutable, $"File's extension of file <{path}> isn't valid");
                            }

                            // beware we don't care about result path or name here, we just expect it to throw InvalidOperationException
                            // if path does not conform to executable or does not exist
                            FileHelpers.IsValidPathString(path);

                            newLinks.Add(path);
                        }
                        catch (InvalidOperationException exception)
                        {
                            throw new PatchFailedException((int)LayoutPatchError.InvalidPath, exception.Message);
                        }
                    });

                    currentValue.LayoutInfo.LinkApplicationPaths = newLinks.Distinct();
                }

                if (input.LinkApplicationEnable.IsValuePresent)
                {
                    currentValue.LayoutInfo.LinkApplicationEnable = (bool)input.LinkApplicationEnable;
                }

                if (!currentValue.LayoutInfo.Hid && input.AssociatedId.IsValuePresent)
                {
                    var newAssociatedId = input.AssociatedId.Value;

                    if (string.IsNullOrEmpty(newAssociatedId))
                    {
                        throw new PatchFailedException((int)LayoutPatchError.InvalidLayoutAssociatedId, "Associated id is null or empty");
                    }

                    currentValue.AssociatedLayoutId = new LayoutId(newAssociatedId);
                }

                var needToRecreateLayoutImage = false;

                if (currentValue.LayoutInfo.Hid && input.AugmentedHidEnable.IsValuePresent)
                {
                    currentValue.LayoutInfo.AugmentedHidEnable = input.AugmentedHidEnable.Value;

                    needToRecreateLayoutImage = true;
                }

                if (input.IsDarkMode.IsValuePresent)
                {
                    currentValue.LayoutImageInfo.Color = input.IsDarkMode.Value ? HexColor.Black : HexColor.White;

                    needToRecreateLayoutImage = true;
                }

                if (input.Font.IsValuePresent)
                {
                    var newFont = input.Font.Value;
                    if (newFont == null)
                    {
                        throw new PatchFailedException((int)LayoutPatchError.InvalidLayoutFont, "Font can't be null");
                    }

                    if (!_fontProvider.FontExists(newFont.Name))
                    {
                        throw new PatchFailedException((int)LayoutPatchError.InvalidLayoutFont, "Font can't be null");
                    }

                    currentValue.LayoutImageInfo.Font = newFont.ToDomainModel();

                    needToRecreateLayoutImage = true;
                }

                if (input.ImageType.IsValuePresent)
                {
                    var newImageType = input.ImageType.Value;

                    currentValue.LayoutImageInfo.ImageType = newImageType;

                    needToRecreateLayoutImage = true;
                }

                if (input.AdjustmentXPosition.IsValuePresent)
                {
                    currentValue.LayoutImageInfo.XPositionAdjustment = input.AdjustmentXPosition.Value;

                    needToRecreateLayoutImage = true;
                }

                if (input.AdjustmentYPosition.IsValuePresent)
                {
                    currentValue.LayoutImageInfo.YPositionAdjustement = input.AdjustmentYPosition.Value;

                    needToRecreateLayoutImage = true;
                }

                if (needToRecreateLayoutImage)
                {
                    var imageRequest = new ImageRequest(
                        info: currentValue.LayoutInfo,
                        imageInfo: currentValue.LayoutImageInfo,
                        keys: currentValue.Keys,
                        screen: currentValue.LayoutImageInfo.Screen,
                        adjustment: new ImageAdjustment(
                            currentValue.LayoutImageInfo.XPositionAdjustment,
                            currentValue.LayoutImageInfo.YPositionAdjustement
                        )
                    );

                    currentValue.Image = _layoutGenService.RenderLayoutImage(imageRequest);
                }

                currentValue.DateUpdated = DateTime.Now;

            });

            return currentValue;
        }
    }
}

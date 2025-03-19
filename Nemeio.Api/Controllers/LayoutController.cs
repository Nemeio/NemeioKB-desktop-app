using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Api.Dto.In.Layout;
using Nemeio.Api.Dto.Out;
using Nemeio.Api.Dto.Out.Category;
using Nemeio.Api.Dto.Out.Layout;
using Nemeio.Api.Dto.Out.Templates;
using Nemeio.Api.Exceptions;
using Nemeio.Api.PatchApplier;
using Nemeio.Core.Enums;
using Nemeio.Core.Errors;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Images;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Category;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Keyboard.Communication.Tools.Frames;
using Newtonsoft.Json;

namespace Nemeio.Api.Controllers
{
    /// <summary>
    /// Key display option.
    /// All = Display all layout's keys
    /// None and NotSet = Not display layout's key
    /// </summary>
    public enum KeyOption
    {
        All,
        None,
        NotSet
    }

    [Route("api/layouts")]
    [ApiController]
    [ApiFilter]
    public class LayoutController : DefaultController
    {
        private static object _locker = new object();
        private const string OptionKeyName = "optionKey";
        private readonly ILogger _logger;
        private readonly ILayoutDbRepository _layoutDbRepository;
        private readonly ILayoutImageGenerator _layoutGenService;
        private readonly ICategoryDbRepository _categoryDbRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFontProvider _fontProvider;
        private readonly ILayoutExportService _layoutExportService;
        private readonly ILayoutFacade _layoutFacade;
        private readonly ILayoutLibrary _library;
        private readonly IApplicationLayoutManager _applicationLayoutManager;
        private readonly ISystem _system;

        private const string MediaTypeNameJson = "application/json";
        private const string MediaTypeNameZip = "application/binary";

        public LayoutController()
            : base()
        {
            _loggerFactory = Mvx.Resolve<ILoggerFactory>();
            _logger = _loggerFactory.CreateLogger<LayoutController>();
            _layoutDbRepository = Mvx.Resolve<ILayoutDbRepository>();
            _layoutGenService = Mvx.Resolve<ILayoutImageGenerator>();
            _categoryDbRepository = Mvx.Resolve<ICategoryDbRepository>();
            _fontProvider = Mvx.Resolve<IFontProvider>();
            _layoutExportService = Mvx.Resolve<ILayoutExportService>();
            _library = Mvx.Resolve<ILayoutLibrary>();
            _layoutFacade = Mvx.Resolve<ILayoutFacade>();
            _applicationLayoutManager = Mvx.Resolve<IApplicationLayoutManager>();
            _system = Mvx.Resolve<ISystem>();
        }

        /// <summary>
        /// Allows you to retrieve all categories containing all layouts (including keys).
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetLayouts()
        {
            var categories = _categoryDbRepository.ReadAllCategories();
            var dtoCategories = categories.Select(x => CategoryApiOutDto.FromModel(x)).ToList();

            // not really sure here that categories will be maintained so we currently simplifies layout access to basic operations
            // and will consequently filter layouts by categories instead of a category readng which will still access all layouts

            var layoutApiOutDtos = _library
                .Layouts
                .Select(layout => LayoutApiOutDto.FromModel(layout))
                .ToList();

            foreach (var cat in dtoCategories)
            {
                foreach (var l in layoutApiOutDtos)
                {
                    var sysLayout = l.IsHid ?
                        _system?.Layouts?.FirstOrDefault(x => x.Id == l.SystemLayoutId) :
                        _system?.Layouts?.FirstOrDefault(x => x.Id == layoutApiOutDtos.FirstOrDefault(y => y.Id == l.AssociatedId)?.SystemLayoutId);

                    if (sysLayout != null)
                    {
                        l.Order = sysLayout.Order;
                    }
                }
                cat.Layouts = layoutApiOutDtos.Where(item => item.CategoryId == cat.Id).OrderBy(x => x.IsHid).ThenBy(x => x.Order);
                if (cat.Layouts == null)
                {
                    cat.Layouts = new List<LayoutApiOutDto>();
                }

            }
            return SuccessResponse(dtoCategories);
        }

        /// <summary>
        /// Retrieve a layout by is identifier. OptionKey allows you to retrieve all the keys associated with the layout or not. The possible values are all or none. The value is mandatory.
        /// </summary>
        /// <param name="id">Layout's id</param>
        /// <param name="keyOption">Key's option : All (returns layouts with keys), None (without keys)</param>
        [HttpGet("{id}")]
        public IActionResult GetLayoutById(string id, [FromQuery(Name = OptionKeyName)] KeyOption keyOption = KeyOption.NotSet)
        {
            if (keyOption == KeyOption.NotSet)
            {
                return ErrorResponse(ErrorCode.ApiGetLayoutOptionInvalid);
            }

            try
            {
                var layout = _library.Layouts.First(x => x.LayoutId.Equals(new LayoutId(id)));

                var outDto = keyOption == KeyOption.All
                    ? GetLayoutJsonWithKeys(layout)
                    : GetLayoutJsonWithoutKeys(layout);

                return SuccessResponse(outDto);
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError($"GetLayoutById : {exception.Message}");

                return ErrorResponse(ErrorCode.ApiGetLayoutIdNotFound);
            }
        }

        private BaseOutDto GetLayoutJsonWithKeys(ILayout layout) => LayoutApiOutDto.FromModel(layout);

        private BaseOutDto GetLayoutJsonWithoutKeys(ILayout layout) => LightLayoutApiOutDto.FromModel(layout);

        /// <summary>
        /// Allow user to update layout's information. If the call failed, transaction is lost and layout has not been updated in database. This endpoint will recreate layout's image onnly if user change UI settings (e.g. background color, main font, ...).
        /// </summary>
        /// <param name="id">Layout's id</param>
        /// <param name="inDto">Layout's updated information</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLayout(string id, [FromBody] PutLayoutApiInDto inDto)
        {
            try
            {
                var layoutId = new LayoutId(id);
                var layout = _library.Layouts.FirstOrDefault(x => x.LayoutId.Equals(layoutId));
                if (layout == null)
                {
                    _logger.LogWarning($"PutLayouts : Layout unknow with id <{layoutId}>");
                    throw new ApiException(ErrorCode.ApiPutLayoutIdNotFound);
                }

                var layoutPatcher = new LayoutPatchApplier(_loggerFactory, _layoutGenService, _library, _applicationLayoutManager, _fontProvider);

                if (inDto.CategoryId.IsValuePresent)
                {
                    //  Check if category exists only if it's specified
                    var categoryExists = _categoryDbRepository.CategoryExists(inDto.CategoryId.Value);
                    if (!categoryExists)
                    {
                        _logger.LogWarning($"PutLayouts : Category does not exists <{inDto.CategoryId}>");
                        throw new ApiException(ErrorCode.ApiPutLayoutCategoryIdInvalid);
                    }
                }
                bool associatedIdChanged = false;
                ILayout newAssociatedLayout = null;
                if (inDto.AssociatedId.IsValuePresent && inDto.AssociatedId.Value != null)
                {
                    var targetAssociatedId = new LayoutId(inDto.AssociatedId.Value);
                    newAssociatedLayout = _library.Layouts.FirstOrDefault(x => x.LayoutId == targetAssociatedId);
                    if (newAssociatedLayout == null || !newAssociatedLayout.LayoutInfo.Hid)
                    {
                        _logger.LogWarning($"PutLayouts : Null or Invalid AssociatedId  <{inDto.AssociatedId.Value}>");
                        throw new ApiException(ErrorCode.ApiPutAssociatedIdInvalid);
                    }
                    if (targetAssociatedId != layout.AssociatedLayoutId)
                    {
                        CherryPickUnmodifiedKeys(layout, newAssociatedLayout);
                        layout.AssociatedLayoutId = layout.OriginalAssociatedLayoutId;
                    }
                }

                _logger.LogInformation($"Edit layout <{layout.LayoutId}>");

                try
                {
                    var updatedLayout = layoutPatcher.Patch(inDto, layout);
                    await _layoutFacade.UpdateLayoutAsync(updatedLayout);

                    _logger.LogInformation($"Edit layout <{layout.LayoutId}> succeed");

                    return SuccessResponse(
                        LayoutApiOutDto.FromModel(updatedLayout)
                    );
                }
                catch (ApiException apiException)
                {
                    return ErrorResponse(apiException.ErrorCode);
                }
                catch (PatchFailedException exception)
                {
                    _logger.LogWarning(exception, $"Edit layout <{layout.LayoutId}> failed");

                    var apiErrorCode = ErrorCode.ApiPutLayoutDataContentInvalid;

                    switch ((LayoutPatchError)exception.ErrorCode)
                    {
                        case LayoutPatchError.EmptyLayoutName:
                            apiErrorCode = ErrorCode.ApiPutLayoutNameEmpty;
                            break;
                        case LayoutPatchError.LayoutNameAlreadyUsed:
                            apiErrorCode = ErrorCode.ApiPutLayoutNameAlreadyUsed;
                            break;
                        case LayoutPatchError.NotAnExecutable:
                            apiErrorCode = ErrorCode.ApiPutLayoutNotExecutableFile;
                            break;
                        case LayoutPatchError.InvalidPath:
                            apiErrorCode = ErrorCode.ApiPutLayoutInvalidPath;
                            break;
                    }

                    _logger.LogError(exception, $"PutLayouts (error code <{apiErrorCode}>) : Layout update failed <{exception.Message}>");

                    return ErrorResponse(apiErrorCode);
                }
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogWarning($"PutLayouts : Layout unknow {exception.Message}");

                return ErrorResponse(ErrorCode.ApiPutLayoutIdNotFound);
            }
        }

        /// <summary>
        /// Replaces any Unedited key in targetLayout with the corresponding key in refLayout
        /// </summary>
        /// <param name="targetLayout"></param>
        /// <param name="refLayout"></param>
        private void CherryPickUnmodifiedKeys(ILayout targetLayout, ILayout refLayout)
        {
            foreach (var targetKey in refLayout.Keys)
            {
                if (!targetLayout.Keys[targetKey.Index].Edited)
                {
                    targetLayout.Keys[targetKey.Index] = targetKey;
                }
            }

        }

        /// <summary>
        /// Delete layout from the database. It's impossible to delete Hid layout. CAUTION: The deletion is definitive.
        /// </summary>
        /// <param name="id">Layout's id</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLayout(string id)
        {
            try
            {
                var layoutId = new LayoutId(id);
                var layout = _library.Layouts.FirstOrDefault(x => x.LayoutId.Equals(layoutId));

                if (layout == null)
                {
                    return ErrorResponse(ErrorCode.ApiDeleteLayoutIdNotFound);
                }

                if (layout.LayoutInfo.Hid)
                {
                    return ErrorResponse(ErrorCode.ApiDeleteHidLayoutForbidden);
                }

                await _layoutFacade.RemoveLayoutAsync(layoutId);

                return SuccessResponse();
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogWarning($"DeleteLayout : Layout unknow {exception.Message}");

                return ErrorResponse(ErrorCode.ApiDeleteLayoutIdNotFound);
            }
        }

        /// <summary>
        /// Activate the provided layout Id as being the default layout and being active
        /// </summary>
        /// <param name="id">Layout's id</param>
        /// <returns></returns>
        [HttpPut("default/{id}")]
        public async Task<IActionResult> SetDefaultLayout(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    await _layoutFacade.ResetDefaultLayoutAsync();
                }
                else
                {
                    var layoutId = new LayoutId(id);

                    await _layoutFacade.SetDefaultLayoutAsync(layoutId);
                }

                return SuccessResponse();
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogWarning(exception, $"SetDefaultLayout : Layout unknow");

                return ErrorResponse(ErrorCode.ApiSetDefaultLayoutIdNotFound);
            }
        }

        /// <summary>
        /// Allow user to retrieve template layouts to create custom layout.
        /// </summary>
        [HttpGet("templates")]
        public IActionResult GetTemplates()
        {
            var templates = _layoutDbRepository.GetTemplateLayouts();
            var templatesOutDtos = templates.Select(x => new TemplateOutDto()
            {
                Id = x.LayoutId,
                Title = x.Title
            });

            var outDto = new TemplatesOutDto()
            {
                Templates = templatesOutDtos
            };

            return SuccessResponse(outDto);
        }

        /// <summary>
        /// Allow user to create a custom layout from template.
        /// </summary>
        /// <param name="inDto">Creation's parameters</param>
        [HttpPost]
        public async Task<IActionResult> PostLayout([FromBody] PostLayoutInDto inDto)
        {
            var exists = _layoutDbRepository.TemplateExists(inDto.TemplateId);
            if (!exists)
            {
                return ErrorResponse(ErrorCode.ApiPostLayoutTemplateIdNotFound);
            }

            var layoutWithSameExists = _library
                .Layouts
                .Select(layout => layout.Title)
                .Any((name) => inDto.Title == name);

            if (layoutWithSameExists)
            {
                return ErrorResponse(ErrorCode.ApiDuplicationLayoutNameAlreadyUsed);
            }

            var templateLayout = _library
                .Layouts
                .First(lyt => lyt.LayoutId == new LayoutId(inDto.TemplateId))
                .CreateBackup();

            if (templateLayout.LayoutInfo.Hid)
            {
                templateLayout.AssociatedLayoutId = templateLayout.LayoutId;
                templateLayout.OriginalAssociatedLayoutId = templateLayout.LayoutId;
            }

            templateLayout.LayoutId = LayoutId.NewLayoutId;
            templateLayout.Title = inDto.Title;
            templateLayout.Enable = false;
            templateLayout.DateCreated = DateTime.Now;
            templateLayout.DateUpdated = DateTime.Now;
            templateLayout.IsDefault = false;
            templateLayout.LayoutInfo.Hid = false;
            templateLayout.LayoutInfo.Factory = false;
            templateLayout.LayoutInfo.OsLayoutId = new OsLayoutId(string.Empty);
            templateLayout.LayoutInfo.IsTemplate = false;
            templateLayout.LayoutInfo.LinkApplicationPaths = new List<string>();
            templateLayout.LayoutInfo.LinkApplicationEnable = false;

            if (System.AppContext.TryGetSwitch("ISMACOS", out bool ISMACOS) && ISMACOS)
            {
                templateLayout.Subtitle = templateLayout?.Title;
            }

            templateLayout.CalculateImageHash();

            await _layoutFacade.AddLayoutAsync(templateLayout);

            return SuccessResponse(
                LayoutApiOutDto.FromModel(templateLayout)
            );
        }

        /// <summary>
        /// Allow user to retrieve only hid layouts
        /// </summary>
        [HttpGet("hids")]
        public IActionResult GetHidLayouts()
        {
            var hidLayouts = GetLayoutsFromLibrary(isHid: true)
                                .Select(x => LayoutApiOutDto.FromModel(x));

            return SuccessResponse(hidLayouts);
        }

        /// <summary>
        /// Allow user to retrieve only custom layouts
        /// </summary>
        [HttpGet("customs")]
        public IActionResult GetCustomLayouts()
        {
            var customLayouts = GetLayoutsFromLibrary(isHid: false)
                                    .Select(x => LayoutApiOutDto.FromModel(x));

            return SuccessResponse(customLayouts);
        }

        private IEnumerable<ILayout> GetLayoutsFromLibrary(bool isHid)
        {
            var layouts = _library
                .Layouts.OrderBy(x => x.Order)
                .Where(x => x.LayoutInfo.Hid == isHid);

            return layouts;
        }

        /// <summary>
        /// Allow configurator to retrieve embedded images
        /// </summary>
        /// <param name="imageName">Wanted image name</param>
        /// <returns>Base64 encoded image</returns>
        [HttpGet("hids/image/{imageName}")]
        public IActionResult GetHidImage(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return ErrorResponse(ErrorCode.ApiGetHidImageNameInvalid);
            }

            byte[] image;

            using (MemoryStream ms = new MemoryStream())
            {
                var resourceStream = _layoutGenService.LoadEmbeddedResource(imageName);
                if (resourceStream == null)
                {
                    return ErrorResponse(ErrorCode.ApiGetHidImageNotFound);
                }

                resourceStream.CopyTo(ms);
                image = ms.ToArray();
            }

            var base64Image = Convert.ToBase64String(image);

            return SuccessResponse(base64Image);
        }

        /// <summary>
        /// Allow user to update layout keys. Works only with custom layouts. WARNING ! The layout image is not recreated after this call. You must commit changes to recreate image.
        /// </summary>
        /// <param name="id">Layout's id</param>
        /// <param name="keyId">Key's id</param>
        /// <param name="keyInDto">Key's updated information</param>
        [HttpPut("{id}/keys/{keyId}")]
        public IActionResult UpdateLayoutKey(string id, int keyId, [FromBody] PutLayoutKeyInDto keyInDto)
        {
            lock (_locker)
            {
                ILayout layout = null;
                KeyboardMap map = null;
                Core.DataModels.Configurator.Key key = null;
                ErrorCode layoutKeyErrorCode;

                if (!ValidateLayoutAndKeyIds(id, keyId, out layout, out map, out key, out layoutKeyErrorCode))
                {
                    return ErrorResponse(layoutKeyErrorCode);
                }

                try
                {
                    var keyIndex = layout.Keys.IndexOf(key);
                    var updatedKey = new LayoutKeyPatchApplier(_loggerFactory, _fontProvider, _layoutGenService, map).Patch(keyInDto, key);
                    layout.Keys.RemoveAt(keyIndex);
                    layout.Keys.Add(updatedKey);
                    layout.Keys = layout.Keys.OrderBy(x => x.Index).ToList();
                    layout.Enable = false;

                    return SuccessResponse(
                        LayoutApiOutDto.FromModel(layout)
                    );
                }
                catch (PatchFailedException exception)
                {
                    _logger.LogError(exception, $"UpdateLayoutKey Failed : <{exception.Message}>");

                    var apiErrorCode = ErrorCode.ApiUpdateLayoutKeyUpdateKeyFailed;
                    var errorCode = (LayoutKeyError)exception.ErrorCode;

                    switch (errorCode)
                    {
                        case LayoutKeyError.SelectedFileNotExecutable:
                            apiErrorCode = ErrorCode.ApiUpdateLayoutKeyApplicationInvalid;
                            break;
                        case LayoutKeyError.InvalidPath:
                            apiErrorCode = ErrorCode.ApiUpdateLayoutKeyPathInvalid;
                            break;
                        case LayoutKeyError.InvalidUrl:
                            apiErrorCode = ErrorCode.ApiUpdateLayoutKeyUrlInvalid;
                            break;
                        case LayoutKeyError.InvalidFont:
                            apiErrorCode = ErrorCode.ApiUpdateLayoutKeyFontInvalid;
                            break;
                        case LayoutKeyError.InvalidModifier:
                            apiErrorCode = ErrorCode.ApiUpdateLayoutKeyModifierIsLocked;
                            break;
                    }

                    return ErrorResponse(apiErrorCode);
                }
            }
        }

        /// <summary>
        /// Allow user to reset a layout key. Works only with custom layouts. WARNING ! The layout image is not recreated after this call. You must commit changes to recreate image.
        /// </summary>
        /// <param name="id">Layout's id</param>
        /// <param name="keyId">Key's id</param>
        [HttpPut("{id}/reset/{keyId}")]
        public IActionResult Reset(string id, int keyId)
        {
            _logger.LogInformation($"Reset resquest received for layout '{id}' and key '{keyId}'");
            ILayout layout = null;
            KeyboardMap map = null;
            Core.DataModels.Configurator.Key key = null;
            ErrorCode layoutKeyErrorCode;

            if (!ValidateLayoutAndKeyIds(id, keyId, out layout, out map, out key, out layoutKeyErrorCode))
            {
                return ErrorResponse(layoutKeyErrorCode);
            }

            var associatedLayout = _library.Layouts.FirstOrDefault(x => x.LayoutId.Equals(new LayoutId(layout.AssociatedLayoutId)));
            if (associatedLayout == null)
            {
                return ErrorResponse(ErrorCode.ApiUpdateLayoutKeyLayoutNotFound);
            }
            var associatedKey = associatedLayout.Keys.FirstOrDefault(x => x.Index == keyId);

            if (associatedKey == null)
            {
                return ErrorResponse(ErrorCode.ApiUpdateLayoutKeyNotFound);
            }

            var newKey = associatedKey.CreateBackup();
            newKey.Edited = false;
            var keyIndex = layout.Keys.IndexOf(key);
            layout.Keys.RemoveAt(keyIndex);
            layout.Keys.Add(newKey);
            layout.Keys = layout.Keys.OrderBy(x => x.Index).ToList();

            layout.Enable = false;
            _library.UpdateLayoutAsync(layout);
            return SuccessResponse(
                LayoutApiOutDto.FromModel(layout)
            );
        }

        private bool ValidateLayoutAndKeyIds(string id, int keyId, out ILayout layout, out KeyboardMap map, out Core.DataModels.Configurator.Key key, out ErrorCode errorCode)
        {
            key = null;
            layout = null;
            map = null;
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("layout id is undefined");
                errorCode = ErrorCode.ApiUpdateLayoutKeyIdInvalid;
                return false;
            }

            layout = _library.Layouts.FirstOrDefault(x => x.LayoutId.Equals(new LayoutId(id)));
            if (layout == null)
            {
                errorCode = ErrorCode.ApiUpdateLayoutKeyLayoutNotFound;
                return false;
            }
            map = layout.LayoutImageInfo.Screen.Map;
            var numberOfKey = map.Buttons.Count;

            if (keyId < 0 || keyId >= numberOfKey)
            {
                _logger.LogWarning($"key id is invalid: must be greater or equals than 0 et less than {numberOfKey}");

                errorCode = ErrorCode.ApiUpdateLayoutKeyIndexInvalid;
                return false;
            }

            if (layout.LayoutInfo.Hid)
            {
                errorCode = ErrorCode.ApiUpdateLayoutKeyHidLayoutForbidden;
                return false;
            }

            key = layout.Keys.FirstOrDefault(x => x.Index == keyId);

            if (key == null)
            {
                errorCode = ErrorCode.ApiUpdateLayoutKeyNotFound;
                return false;
            }
            errorCode = ErrorCode.Success;
            return true;
        }

        /// <summary>
        /// Allow user to recreate layout's image.
        /// </summary>
        /// <param name="id">Layout's id</param>
        [HttpPost("{id}/commit")]
        public IActionResult CommitUIChanges(string id)
        {
            lock (_locker)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("layout id is undefined");

                    return ErrorResponse(ErrorCode.ApiCommitUiIdInvalid);
                }

                var layout = _library.Layouts.FirstOrDefault(x => x.LayoutId.Equals(new LayoutId(id)));
                if (layout == null)
                {
                    return ErrorResponse(ErrorCode.ApiCommitUiLayoutNotFound);
                }

                if (layout.LayoutInfo.Hid)
                {
                    return ErrorResponse(ErrorCode.ApiCommitUiHidLayoutForbidden);
                }

                var oldLayoutHash = layout.Hash;

                var imageRequest = new ImageRequest(
                    info: layout.LayoutInfo,
                    imageInfo: layout.LayoutImageInfo,
                    keys: layout.Keys,
                    screen: layout.LayoutImageInfo.Screen,
                    adjustment: new ImageAdjustment(
                        layout.LayoutImageInfo.XPositionAdjustment,
                        layout.LayoutImageInfo.YPositionAdjustement
                    )
                );

                layout.Image = _layoutGenService.RenderLayoutImage(imageRequest);

                _layoutFacade.UpdateLayoutAsync(layout);

                return SuccessResponse();
            }
        }

        /// <summary>
        /// Allow user to duplicate layout.
        /// </summary>
        /// <param name="duplicateLayoutInDto">Duplication information</param>
        [HttpPost("duplicate")]
        public async Task<IActionResult> DuplicateLayout([FromBody] DuplicateLayoutInDto duplicateLayoutInDto)
        {
            var layoutId = new LayoutId(duplicateLayoutInDto.LayoutId);
            var layout = _library.Layouts.FirstOrDefault(x => x.LayoutId.Equals(layoutId));
            if (layout == null)
            {
                return ErrorResponse(ErrorCode.ApiDuplicateLayoutIsNotFound);
            }

            try
            {
                var duplicatedLayout = await _layoutFacade.DuplicateLayoutAsync(layout, duplicateLayoutInDto.Title);

                return SuccessResponse(LayoutApiOutDto.FromModel(duplicatedLayout));
            }
            catch (ForbiddenActionException exception)
            {
                _logger.LogError(exception, "Layout is Hid : forbidden to duplicate it");

                return ErrorResponse(ErrorCode.ApiDuplicateLayoutHidForbidden);
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError(exception, "Save duplicated layout failed");

                return ErrorResponse(ErrorCode.ApiDuplicationLayoutFailed);
            }
            catch (InvalidDataException exception)
            {
                _logger.LogError(exception, "Layout title already used");

                return ErrorResponse(ErrorCode.ApiDuplicationLayoutNameAlreadyUsed);
            }
        }

        /// <summary>
        /// Export a layout file on json format.
        /// </summary>
        /// <param name="id">Layout's id</param>
        /// 
        [HttpGet("{id}/export")]
        public IActionResult ExportLayout(string id)
        {
            try
            {
                // Create export layout structure.
                var layoutExport = _layoutExportService.Export(id);

                // Convert to dto.
                var layoutExportApiOutDto = LayoutExportApiOutDto.FromModel(layoutExport);

                // Serialize data.
                var serializedLayoutExport = JsonConvert.SerializeObject(layoutExportApiOutDto);

                // Send binary file.
                var binary = Encoding.UTF8.GetBytes(serializedLayoutExport);
                return File(binary, MediaTypeNameJson, layoutExport.Filename);
            }
            catch (Exception exception)
            {
                throw ConvertException(exception);
            }
        }

        /// <summary>
        /// Export a zip package for CLI.
        /// </summary>
        /// <param name="id">Layout's id</param>
        [HttpGet("{id}/exportforcli")]
        public IActionResult ExportLayoutForCli(string id)
        {
            try
            {
                var layout = _library.Layouts.First(x => x.LayoutId.Equals(new LayoutId(id)));
                var imgStream = new MemoryStream();
                var jsnStream = new MemoryStream();
                var finalResultStream = new MemoryStream();
                imgStream.Write(layout.Image, 0, layout.Image.Length);

                var layoutExport = new LayoutInfoDto(layout);

                // Create export layout structure.

                using (StreamWriter sw = new StreamWriter(jsnStream))
                {
                    sw.Write(JsonConvert.SerializeObject(layoutExport));
                }
                using (var ZipArchive = new ZipArchive(finalResultStream, ZipArchiveMode.Create, true))
                {
                    var jsonEntry = ZipArchive.CreateEntry($"{layout.Title}/json.json");
                    using (var entryStream = jsonEntry.Open())
                    {
                        using (var sw = new StreamWriter(entryStream))
                        {
                            sw.Write(JsonConvert.SerializeObject(layoutExport));
                        }
                    }
                    var imgEntry = ZipArchive.CreateEntry($"{layout.Title}/json.wallpaper.gz");
                    using (var entryStream = imgEntry.Open())
                    {
                        entryStream.Write(layout.Image, 0, layout.Image.Length);
                    }
                }
                finalResultStream.Position = 0;
                return File(finalResultStream, MediaTypeNameZip, $"{layout.Title}.zip");
            }
            catch (Exception exception)
            {
                throw ConvertException(exception);
            }
        }

        /// <summary>
        /// Open an exported layout file.
        /// </summary>
        /// <param name="file">File selected by user</param>
        [HttpPost("export/open")]
        public IActionResult OpenExportedLayout(IFormFile file)
        {
            try
            {
                // Read file.
                var fileContent = ReadFileAsText(file);

                // Deserialize data.
                var layoutExportApiOutDto = JsonConvert.DeserializeObject<LayoutExportApiOutDto>(fileContent);

                // Convert dto.
                var layoutExport = LayoutExportApiOutDto.FromDto(layoutExportApiOutDto);

                // Check version.
                _layoutExportService.CheckExportVersion(layoutExport);

                // Return export structure.
                return SuccessResponse(layoutExportApiOutDto);
            }
            catch (Exception exception)
            {
                throw ConvertException(exception);
            }
        }

        private string ReadFileAsText(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Import a custom layout.
        /// </summary>
        /// <param name="layoutExportApiOutDto">Import parameters</param>
        [HttpPost("import")]
        public async Task<IActionResult> ImportLayout([FromBody] LayoutExportApiOutDto layoutExportApiOutDto)
        {
            try
            {
                // Convert dto.
                var layoutExport = LayoutExportApiOutDto.FromDto(layoutExportApiOutDto);

                // Check version.
                _layoutExportService.CheckExportVersion(layoutExport);

                // Import layout.
                var layout = await _layoutFacade.ImportLayoutAsync(layoutExport);

                var layoutApiOutDto = LayoutApiOutDto.FromModel(layout);

                // Return layout.
                return SuccessResponse(layoutApiOutDto);
            }
            catch (Exception exception)
            {
                throw ConvertException(exception);
            }
        }

        /// <summary>
        /// You can test if a name is already taken by another layout
        /// </summary>
        /// <param name="inDto">Check information</param>
        [HttpPost("nameAvailable")]
        public IActionResult PostLayoutNameIsAvailable(PostLayoutNameIsAvailableInDto inDto)
        {
            var exists = _library
                .Layouts
                .Select(layout => layout.Title)
                .Any((name) => inDto.Name == name);

            var outDto = new LayoutNameIsAvailableOutDto(!exists);

            return SuccessResponse(outDto);
        }

        private Exception ConvertException(Exception exception)
        {
            if (exception is CoreException coreException)
            {
                switch (coreException.ErrorCode)
                {
                    case ErrorCode.CoreExportLayoutInvalidId:
                        return new ApiException(ErrorCode.ApiExportLayoutInvalidId);
                    case ErrorCode.CoreExportLayoutIsNotFound:
                        return new ApiException(ErrorCode.ApiExportLayoutIsNotFound);
                    case ErrorCode.CoreExportLayoutHidForbidden:
                        return new ApiException(ErrorCode.ApiExportLayoutHidForbidden);
                    case ErrorCode.CoreImportLayoutInvalidExportVersion:
                        return new ApiException(ErrorCode.ApiImportLayoutInvalidExportVersion);
                    case ErrorCode.CoreImportLayoutTitleEmpty:
                        return new ApiException(ErrorCode.ApiImportLayoutTitleEmpty);
                    case ErrorCode.CoreImportLayoutTitleAlreadyUsed:
                        return new ApiException(ErrorCode.ApiImportLayoutTitleAlreadyUsed);
                    case ErrorCode.CoreImportLayoutInvalidAssociatedLayoutId:
                        return new ApiException(ErrorCode.ApiImportLayoutInvalidAssociatedLayoutId);
                    case ErrorCode.CoreImportLayoutMissingAssociatedLayout:
                        return new ApiException(ErrorCode.ApiImportLayoutMissingAssociatedLayout);
                }
            }

            return exception;
        }
    }
}

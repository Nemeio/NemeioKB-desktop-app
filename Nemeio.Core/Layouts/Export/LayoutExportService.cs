using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Enums;
using Nemeio.Core.Errors;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Export
{
    public class LayoutExportService : ILayoutExportService
    {
        protected readonly ILogger _logger;

        protected readonly ILayoutDbRepository _layoutDbRepository;

        private const int CurrentMajorVersion = 2;
        private const int CurrentMinorVersion = 0;

        private string CurrentVersion => $"{CurrentMajorVersion}.{CurrentMinorVersion}";

        public const string ExportExtension = "nemeio";
        public const string ExportForCliExtension = "zip";

        public LayoutExportService(ILoggerFactory loggerFactory, ILayoutDbRepository layoutDbRepository)
        {
            _logger = loggerFactory.CreateLogger<LayoutExportService>();
            _layoutDbRepository = layoutDbRepository ?? throw new ArgumentNullException(nameof(layoutDbRepository));
        }

        /// <summary>
        /// Export a custom layout.
        /// </summary>
        /// <param name="id">The id of the layout to export</param>
        /// <returns>An object representing the layout at export format.</returns>
        /// <exception cref="ArgumentException">When the id is not a GUID.</exception>
        /// <exception cref="InvalidOperationException">When the layout does not exists.</exception>
        /// <exception cref="ForbiddenActionException">When the layout is not a custom layout.</exception>
        public LayoutExport Export(string id, ExportFileType target = ExportFileType.API)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            // Convert id to layoutId.
            LayoutId layoutId;
            try
            {
                layoutId = new LayoutId(id);
            }
            catch (ArgumentException)
            {
                throw new CoreException(ErrorCode.CoreExportLayoutInvalidId);
            }

            return Export(layoutId, target);
        }

        /// <summary>
        /// Export a custom layout.
        /// </summary>
        /// <param name="layoutId">The id of the layout to export</param>
        /// <returns>An object representing the layout at export format.</returns>
        /// <exception cref="InvalidOperationException">When the layout does not exists.</exception>
        /// <exception cref="ForbiddenActionException">When the layout is not a custom layout.</exception>
        public LayoutExport Export(LayoutId layoutId, ExportFileType target)
        {
            if (layoutId == null)
            {
                throw new ArgumentNullException(nameof(layoutId));
            }

            // Load layout from database.
            ILayout layout;
            try
            {
                layout = _layoutDbRepository.FindLayoutById(layoutId);
            }
            catch (InvalidOperationException)
            {
                throw new CoreException(ErrorCode.CoreExportLayoutIsNotFound);
            }

            return Export(layout, target);
        }

        private LayoutExport Export(ILayout layout, ExportFileType target)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            if (layout.LayoutInfo.Hid)
            {
                throw new CoreException(ErrorCode.CoreExportLayoutHidForbidden);
            }

            // Create export layout object.
            var layoutExport = new LayoutExport()
            {
                Filename = target == ExportFileType.CLI ? $"{layout.Title}.{ExportForCliExtension}" : $"{layout.Title}.{ExportExtension}",
                Title = layout.Title,
                MajorVersion = CurrentMajorVersion,
                MinorVersion = CurrentMinorVersion,
                Version = CurrentVersion,
                AssociatedLayoutId = layout.AssociatedLayoutId,
                Keys = layout.Keys.Where(o => o.Edited).ToList(),
                Font = layout.LayoutImageInfo.Font.CreateBackup(),
                LinkApplicationPaths = layout.LayoutInfo.LinkApplicationPaths.ToList(),
                LinkApplicationEnable = layout.LayoutInfo.LinkApplicationEnable,
                IsDarkMode = layout.LayoutImageInfo.Color.IsBlack(),
                ImageType = layout.LayoutImageInfo.ImageType,
                Screen = layout.LayoutImageInfo.Screen.Type
            };

            return layoutExport;
        }

        public void CheckExportVersion(LayoutExport layoutExport)
        {
            var isValid = layoutExport.MajorVersion == CurrentMajorVersion;
            if (!isValid)
            {
                throw new CoreException(ErrorCode.CoreImportLayoutInvalidExportVersion);
            }
        }
    }
}

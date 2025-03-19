using System;
using Nemeio.Core.Enums;
using Nemeio.Core.Exceptions;

namespace Nemeio.Core.Layouts.Export
{
    public interface ILayoutExportService
    {
        /// <summary>
        /// Export a custom layout.
        /// </summary>
        /// <param name="id">The id of the layout to export</param>
        /// <returns>An object representing the layout at export format.</returns>
        /// <exception cref="ArgumentException">When the id is not a GUID.</exception>
        /// <exception cref="InvalidOperationException">When the layout does not exists.</exception>
        /// <exception cref="ForbiddenActionException">When the layout is not a custom layout.</exception>
        LayoutExport Export(string id, ExportFileType target = ExportFileType.API);

        /// <summary>   
        /// Check if a layout export is compliant with the current version.
        /// </summary>
        void CheckExportVersion(LayoutExport layoutExport);
    }
}

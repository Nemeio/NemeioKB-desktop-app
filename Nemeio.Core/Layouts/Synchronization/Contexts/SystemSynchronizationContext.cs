using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Core.Layouts.Synchronization.Contexts
{
    public class SystemSynchronizationContext : SynchronizationContext
    {
        private readonly ISystemLayoutLoaderAdapter _systemLayoutLoaderAdapter;
        private readonly ILayoutLibrary _library;
        private readonly ILayoutFactory _layoutFactory;
        private readonly ISystem _system;
        private readonly IScreen _screen;

        public SystemSynchronizationContext(ILoggerFactory loggerFactory, ISystem system, ILayoutFactory layoutFactory, ILayoutLibrary library, ISystemLayoutLoaderAdapter systemLayoutLoaderAdapter, IScreen screen)
            : base(loggerFactory)
        {
            _layoutFactory = layoutFactory ?? throw new ArgumentNullException(nameof(layoutFactory));
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _systemLayoutLoaderAdapter = systemLayoutLoaderAdapter ?? throw new ArgumentNullException(nameof(systemLayoutLoaderAdapter));
            _screen = screen ?? throw new ArgumentNullException(nameof(screen));
        }

        public override async Task SyncAsync()
        {
            var systemLayouts = _systemLayoutLoaderAdapter
                .Load()
                .ToList();

            if (!_library.Started)
            {
                _library.Start(_screen);
            }

            var databaseLayouts = _library.Layouts;

            //  Determine which system layout must be added to database
            var missingLayoutInDatabase = systemLayouts
                .Except(databaseLayouts.Where(lyt => lyt.LayoutInfo.Hid).Select(sl => sl.LayoutInfo.OsLayoutId))
                .Select(x => new OsLayoutId(x.Id))
                .ToList();

            //  Create HID layout
            var missingHidLayouts = _layoutFactory.CreateHids(missingLayoutInDatabase, _screen);
            var customLayoutsToEnable = new List<ILayout>();

            foreach (var layout in missingHidLayouts)
            {
                _logger.LogTrace($"Add system layout <{layout.Title}> with id <{layout.LayoutId}> and system id <{layout.LayoutInfo.OsLayoutId}> on database");
                customLayoutsToEnable.AddRange(_library.Layouts.Where(x => x.OriginalAssociatedLayoutId == layout.LayoutId));
                await _library.AddLayoutAsync(layout);
            }
            foreach (var layout in customLayoutsToEnable)
            {
                _logger.LogTrace($"Enabling Custom Layout  <{layout.LayoutId}> in database");
                layout.Enable = true;
                layout.AssociatedLayoutId = layout.OriginalAssociatedLayoutId;
                await _library.UpdateLayoutAsync(layout);
            }
            //  Determine which system layout must be deleted from database
            var uselessHidLayoutOsIdOnDatabase = databaseLayouts
                .Where(x => x.LayoutInfo.Hid)
                .Select(layout => layout.LayoutInfo.OsLayoutId)
                .Except(systemLayouts)
                .ToList();

            var uselessHidLayoutOnDatabase = new List<ILayout>();
            var customLayoutsToDisable = new List<ILayout>();

            foreach (var id in uselessHidLayoutOsIdOnDatabase)
            {
                foreach (var layout in databaseLayouts)
                {
                    if (layout.LayoutInfo.OsLayoutId == id)
                    {
                        uselessHidLayoutOnDatabase.Add(layout);
                        customLayoutsToDisable.AddRange(_library.Layouts.Where(x => x.AssociatedLayoutId == layout.LayoutId));
                    }
                }
            }

            foreach (var layout in uselessHidLayoutOnDatabase)
            {
                _logger.LogTrace($"Remove system layout <{layout.LayoutId}> from database");
                await _library.RemoveLayoutAsync(layout.LayoutId);
            }
            if (customLayoutsToDisable.Any())
            {
                foreach (var layout in customLayoutsToDisable)
                {
                    _logger.LogTrace($"Disabling Custom Layout  <{layout.LayoutId}> in database");
                    layout.Enable = false;
                    layout.AssociatedLayoutId = null;
                    await _library.UpdateLayoutAsync(layout);
                }
                _system.NotifyCustomRemovedByHid(uselessHidLayoutOnDatabase, customLayoutsToDisable);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Configurations.Add;
using Nemeio.Core.Keyboard.Configurations.Delete;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Layouts.Synchronization.Contexts.State;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Synchronization.Contexts
{
    public class KeyboardSynchronizationContext : SynchronizationContext, ISynchronizationStateHolder
    {
        private readonly ISynchronizableNemeioProxy _proxy;
        private readonly ILayoutLibrary _library;
        private readonly Stopwatch _stopWatch;

        public ISynchronizationContextState State { get; private set; }

        public KeyboardSynchronizationContext(ILoggerFactory loggerFactory, ILayoutLibrary library, ISynchronizableNemeioProxy proxy)
            : base(loggerFactory)
        {
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _stopWatch = new Stopwatch();

            State = new SynchronizationContextState();
        }

        public override async Task SyncAsync()
        {
            //  Bypass if keyboard is on factory mode
            if (_proxy.State == NemeioState.FactoryReset)
            {
                return;
            }

            //  Calculate difference between keyboard and database
            var creatableLayoutOnKeyboard = new List<ILayout>();

            //  Load layouts from database
            var databaseLayouts = _library.Layouts;

            //  Calculate deletable layouts
            var deletableHashes = _proxy
                .LayoutIdWithHashs.Select(x => x.Hash)
                .Except(databaseLayouts.Select(x => x.Hash))
                .ToList();


            //  Calculate creatable layouts
            foreach (var layout in databaseLayouts)
            {
                if (!_proxy.LayoutIdWithHashs.Any(x => x.Hash == layout.Hash) && layout.Enable)
                {
                    creatableLayoutOnKeyboard.Add(layout);
                }
            }

            //  On keyboard we must delete first
            //  and then add layout to update it

            var nbModification = creatableLayoutOnKeyboard.Count() + deletableHashes.Count();
            uint startWithNbModification = nbModification >= 0 ? (uint)nbModification : 0;

            State.StartSync(startWithNbModification);

            await _proxy.StartSynchronizationAsync();

            foreach (var layoutHash in deletableHashes)
            {
                _logger.LogTrace($"Remove layout from keyboard with hash <{layoutHash}>");
                var target = _proxy.LayoutIdWithHashs.FirstOrDefault(x => x.Hash == layoutHash);
                if (target != null)
                {
                    try
                    {

                        _stopWatch.Reset();
                        _stopWatch.Start();

                        await _proxy.DeleteLayoutAsync(target.Id);

                        _stopWatch.Stop();

                        _logger.LogTrace($"Time to delete <{layoutHash}> is <{_stopWatch.ElapsedMilliseconds}> milliseconds");
                    }
                    catch (DeleteConfigurationFailedException exception)
                    {
                        _logger.LogError(exception, $"Try to remove layout with hash <{layoutHash}> but failed");
                    }

                }
                State.Progress();
            }

            foreach (var layout in creatableLayoutOnKeyboard)
            {
                _logger.LogTrace($"Add layout <{layout.Title}> with id <{layout.LayoutId}> on keyboard");

                try
                {
                    _stopWatch.Reset();
                    _stopWatch.Start();

                    await _proxy.AddLayoutAsync(layout);

                    _stopWatch.Stop();

                    _logger.LogTrace($"Time to add <{layout.Title}> is <{_stopWatch.ElapsedMilliseconds}> milliseconds");
                }
                catch (AddConfigurationFailedException exception)
                {
                    _logger.LogError(exception, $"Try to add layout <{layout.Title}> but failed");
                }

                State.Progress();
            }

            await _proxy.EndSynchronizationAsync();

            State.EndSync();
        }
    }
}

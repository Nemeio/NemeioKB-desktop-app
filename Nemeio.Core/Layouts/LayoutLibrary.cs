using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Name;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Tools.Stoppable;

namespace Nemeio.Core.Layouts
{
    public class LayoutLibrary : Stoppable, ILayoutLibrary
    {
        private readonly ILogger _logger;

        private readonly ILayoutDbRepository _repository;
        private readonly IErrorManager _errorManager;
        private readonly ILayoutValidityChecker _validtityChecker;
        private readonly ILayoutNativeNameAdapter _nativeNameAdapter;

        public IList<ILayout> Layouts { get; private set; }

        public LayoutLibrary(ILoggerFactory loggerFactory, ILayoutValidityChecker validtityChecker, ILayoutDbRepository layoutRepository, IErrorManager errorManager, ILayoutNativeNameAdapter nativeNameAdapter)
            : base(false)
        {
            _logger = loggerFactory.CreateLogger<LayoutLibrary>();
            _validtityChecker = validtityChecker ?? throw new ArgumentNullException(nameof(validtityChecker));
            _repository = layoutRepository ?? throw new ArgumentNullException(nameof(layoutRepository));
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _nativeNameAdapter = nativeNameAdapter ?? throw new ArgumentNullException(nameof(nativeNameAdapter));

            Layouts = new List<ILayout>();
        }

        public void Start(IScreen screen)
        {
            AliveState = AliveState.Starting;

            Layouts = new List<ILayout>(_repository.ReadAllLayouts(screen.Type));

            CheckLayoutsIntegrity();
            RefreshHidNameIfNeeded();

            AliveState = AliveState.Started;
        }

        public override void Stop()
        {
            Layouts.Clear();

            base.Stop();
        }

        public async Task<ILayout> AddLayoutAsync(ILayout layout)
        {
            await Task.Yield();

            //  verify layout before save it
            var isValid = _validtityChecker.Check(layout);
            if (!isValid)
            {
                _logger.LogError(
                   _errorManager.GetFullErrorMessage(ErrorCode.CoreLayoutValidationBeforeSaveFailed)
               );

                throw new InvalidOperationException("Layout validity check failed.");
            }

            //  stores a new layout into DB
            _repository.CreateLayout(layout);

            Layouts.Add(layout);

            return layout;
        }

        public async Task<ILayout> UpdateLayoutAsync(ILayout layout)
        {
            await Task.Yield();

            _repository.UpdateLayout(layout);

            var oldLayout = Layouts.FirstOrDefault(lyt => lyt.LayoutId == layout.LayoutId);

            Layouts.Remove(oldLayout);
            Layouts.Add(layout);

            return layout;
        }

        public async Task RemoveLayoutAsync(ILayout layout)
        {
            await Task.Yield();

            _repository.DeleteLayout(layout);

            Layouts.Remove(layout);
        }

        public async Task RemoveLayoutAsync(LayoutId layoutId)
        {
            var layout = Layouts.FirstOrDefault(lyt => lyt.LayoutId == layoutId);
            if (layout != null)
            {
                await RemoveLayoutAsync(layout);
            }
        }

        private void RefreshHidNameIfNeeded()
        {
            var hidLayouts = Layouts.Where(x => x.LayoutInfo.Hid);

            foreach (var layout in hidLayouts)
            {
                var osLayoutId = layout.LayoutInfo.OsLayoutId;
                var calculatedName = _nativeNameAdapter.RetrieveNativeName(osLayoutId);

                if (!layout.Title.Equals(calculatedName))
                {
                    layout.Title = calculatedName;

                    UpdateLayoutAsync(layout).ConfigureAwait(false);
                }
            }
        }

        private void CheckLayoutsIntegrity()
        {
            var hidLayouts = Layouts.Where(x => x.LayoutInfo.Hid);
            var invalidLayouts = new HashSet<ILayout>();

            foreach (var layout in hidLayouts)
            {
                var valid = _validtityChecker.Check(layout);
                if (!valid)
                {
                    invalidLayouts.Add(layout);
                }
            }

            var result = new List<ILayout>(Layouts);

            foreach (var invalidLayout in invalidLayouts)
            {
                _logger.LogError($"Layout <{invalidLayout.LayoutId}> is invalid. We delete it.");
                _logger.LogError(
                    _errorManager.GetFullErrorMessage(ErrorCode.CoreLayoutValidationAfterLoadFailed)
                );

                result.Remove(invalidLayout);

                _repository.DeleteLayout(invalidLayout);
            }

            var validationFailed = invalidLayouts.Count > 0;

            if (validationFailed)
            {
                _logger.LogError(
                    _errorManager.GetFullErrorMessage(ErrorCode.CoreLayoutDefinitiveValidationFailed)
                );

                throw new FatalException("Definitive layout validation failed");
            }

            Layouts = result;
        }
    }
}

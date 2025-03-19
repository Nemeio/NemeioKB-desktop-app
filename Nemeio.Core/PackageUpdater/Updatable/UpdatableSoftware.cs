using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Applications;
using Nemeio.Core.PackageUpdater.Strategies;

namespace Nemeio.Core.PackageUpdater.Updatable
{
    public class UpdatableSoftware : Updatable
    {
        private readonly IApplicationSettingsProvider _applicationSettingsManager;
        private readonly Version _targetVersion;

        public override bool IsMandatoryUpdate => false;

        public UpdatableSoftware(ILoggerFactory loggerFactory, IInstallStrategy strategy, IApplicationSettingsProvider applicationSettingsManager, Version targetVersion)
            : base(loggerFactory.CreateLogger<UpdatableSoftware>(), strategy)
        {
            _applicationSettingsManager = applicationSettingsManager ?? throw new ArgumentNullException(nameof(applicationSettingsManager));
            _targetVersion = targetVersion ?? throw new ArgumentNullException(nameof(targetVersion));
        }

        public override Task UpdateAsync()
        {
            _applicationSettingsManager.UpdateTo = _targetVersion;
            _applicationSettingsManager.LastRollbackManifestString = String.Empty;
            return base.UpdateAsync();
        }
    }
}

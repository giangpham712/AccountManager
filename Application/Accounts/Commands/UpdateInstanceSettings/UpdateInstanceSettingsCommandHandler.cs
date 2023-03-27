using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Key;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.UpdateInstanceSettings
{
    public class UpdateInstanceSettingsCommandHandler : CommandHandlerBase<UpdateInstanceSettingsCommand, Unit>
    {
        private readonly IKeysManager _keysManager;
        private readonly ISoftwareVersionResolver _versionResolver;

        public UpdateInstanceSettingsCommandHandler(IMediator mediator, ICloudStateDbContext context,
            IKeysManager keysManager, ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _versionResolver = versionResolver;
            _keysManager = keysManager;
        }

        public override async Task<Unit> Handle(UpdateInstanceSettingsCommand command,
            CancellationToken cancellationToken)
        {
            var transaction = Context.GetTransaction();
            try
            {
                var account = Context.Set<Account>()
                    .Include(x => x.Keys)
                    .Include(x => x.MachineConfig)
                    .Include(x => x.Machines)
                    .FirstOrDefault(x => !x.IsDeleted && x.Id == command.AccountId);

                if (account == null)
                    throw new EntityNotFoundException(nameof(Account), command.AccountId);

                var machineConfig = account.MachineConfig;

                machineConfig.ShowInGrafana = command.ShowInGrafana;
                machineConfig.UseSparePool = command.UseSparePool;

                machineConfig.IncludeIrmoData = command.IncludeIrmoData;
                machineConfig.VmImageName = command.VmImageName;
                machineConfig.AutoStopTime = command.AutoStopTime;
                machineConfig.MarketingVersion = command.MarketingVersion;


                if (!machineConfig.UseUniqueKey && command.KeyType != KeyType.Default ||
                    machineConfig.UseUniqueKey && command.KeyType != KeyType.Unique)
                {
                    var currentKeys = account.Keys;
                    var userTokenKeyPair = _keysManager.GenerateUserTokenKeyPair(command.KeyType == KeyType.Default);
                    var interServerKeyPair =
                        _keysManager.GenerateInterServerKeyPair(command.KeyType == KeyType.Default);
                    var licenseKeyPair = _keysManager.GenerateLicenseKeyPair(command.KeyType == KeyType.Default);

                    account.Keys = new Keys
                    {
                        UserTokenPrivate = userTokenKeyPair.PrivateKey,
                        UserTokenPublic = userTokenKeyPair.PublicKey,
                        InterServerPrivate = interServerKeyPair.PrivateKey,
                        InterServerPublic = interServerKeyPair.PublicKey,
                        LicensePrivate = licenseKeyPair.PrivateKey,
                        LicensePublic = licenseKeyPair.PublicKey,
                        ApiPassword = currentKeys.ApiPassword,
                        AccountFile = currentKeys.AccountFile,
                        SqlExportPass = currentKeys.SqlExportPass
                    };

                    Context.Set<Keys>().Remove(currentKeys);
                }

                machineConfig.UseUniqueKey = command.KeyType != KeyType.Default;

                await account.SetLastUserCycle(Context);

                await UpdateSoftwareVersions(machineConfig, command, cancellationToken);

                foreach (var machine in account.Machines) await UpdateMachine(machine, command, cancellationToken);

                await Context.SaveChangesAsync(cancellationToken, out var changes);

                transaction.Commit();

                await Mediator.Publish(new InstanceSettingsPushedEvent
                {
                    User = command.User,
                    Account = account,
                    Command = command,
                    Changes = changes
                }, cancellationToken);

                return Unit.Value;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task UpdateSoftwareVersions(MachineConfig machineConfig, UpdateInstanceSettingsCommand command,
            CancellationToken cancellationToken)
        {
            var launcherVersion = _versionResolver.Resolve(Software.Launcher, command.LauncherVersionMode,
                command.LauncherVersion, machineConfig.LauncherVersion);
            machineConfig.LauncherVersion = launcherVersion?.ToString();

            var reportingVersion = _versionResolver.Resolve(Software.Reporting, command.ReportingVersionMode,
                command.ReportingVersion, machineConfig.ReportingVersion);
            machineConfig.ReportingVersion = reportingVersion?.ToString();

            var clientVersion = _versionResolver.Resolve(Software.Client, command.ClientVersionMode,
                command.ClientVersion, machineConfig.ClientVersion);
            machineConfig.ClientVersion = clientVersion?.ToString();

            var siteMasterVersion = _versionResolver.Resolve(Software.SiteMaster, command.SiteMasterVersionMode,
                command.SiteMasterVersion, machineConfig.SiteMasterVersion);
            machineConfig.SiteMasterVersion = siteMasterVersion?.ToString();


            var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances)
                .FirstOrDefaultAsync(x => x.Id == machineConfig.Account.ClassId, cancellationToken);
            var deployerVersion = _versionResolver.Resolve(Software.Deployer, command.DeployerVersionMode,
                command.DeployerVersion, machineConfig.DeployerVersion, mmaClass?.MmaInstance?.DeployerVer);
            machineConfig.DeployerVersion = deployerVersion?.ToString();

            var pdfExportVersion = _versionResolver.Resolve(Software.PdfExport, command.PdfExportVersionMode,
                command.PdfExportVersion, machineConfig.PdfExportVersion);
            machineConfig.PdfExportVersion = pdfExportVersion?.ToString();

            var sqlExportVersion = _versionResolver.Resolve(Software.SqlExport, command.SqlExportVersionMode,
                command.SqlExportVersion, machineConfig.SqlExportVersion);
            machineConfig.SqlExportVersion = sqlExportVersion?.ToString();

            var populateVersion = _versionResolver.Resolve(Software.Populate, command.PopulateVersionMode,
                command.PopulateVersion, machineConfig.PopulateVersion);
            machineConfig.PopulateVersion = populateVersion?.ToString();

            var linkwareVersion = _versionResolver.Resolve(Software.Linkware, command.LinkwareVersionMode,
                command.LinkwareVersion, machineConfig.LinkwareVersion);
            machineConfig.LinkwareVersion = linkwareVersion?.ToString();

            var smchkVersion = _versionResolver.Resolve(Software.Smchk, command.SmchkVersionMode,
                command.SmchkVersion, machineConfig.SmchkVersion);
            machineConfig.SmchkVersion = smchkVersion?.ToString();

            var discoveryVersion = _versionResolver.Resolve(Software.Discovery, command.DiscoveryVersionMode,
                command.DiscoveryVersion, machineConfig.DiscoveryVersion);
            machineConfig.DiscoveryVersion = discoveryVersion?.ToString();

            var fiberSenSysVersion = _versionResolver.Resolve(Software.FiberSenSys, command.FiberSenSysVersionMode,
                command.FiberSenSysVersion, machineConfig.FiberSenSysVersion);
            machineConfig.FiberSenSysVersion = fiberSenSysVersion?.ToString();

            var fiberMountainVersion = _versionResolver.Resolve(Software.FiberMountain,
                command.FiberMountainVersionMode,
                command.FiberMountainVersion, machineConfig.FiberMountainVersion);
            machineConfig.FiberMountainVersion = fiberMountainVersion?.ToString();

            var serviceNowVersion = _versionResolver.Resolve(Software.ServiceNow,
                command.ServiceNowVersionMode,
                command.ServiceNowVersion, machineConfig.ServiceNowVersion);
            machineConfig.ServiceNowVersion = serviceNowVersion?.ToString();

            var commScopeVersion = _versionResolver.Resolve(Software.CommScope,
                command.ClientVersionMode,
                command.CommScopeVersion, machineConfig.CommScopeVersion);
            machineConfig.CommScopeVersion = commScopeVersion?.ToString();

            var mainLibraryFiles = _versionResolver.GetLibraryFiles(command.MainLibraryFiles, command.MainLibraryMode,
                machineConfig.MainLibraryFiles);
            machineConfig.MainLibraryFiles = mainLibraryFiles;
            machineConfig.MainLibraryFile = mainLibraryFiles.FirstOrDefault(x =>
            {
                var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                return file != null;
            });

            machineConfig.MainLibraryMode = machineConfig.MainLibraryFiles.Any()
                ? LibraryFileModes.Select
                : LibraryFileModes.None;

            var accountLibraryFiles = _versionResolver.GetLibraryFiles(
                command.AccountLibraryFile.HasValue ? new[] { command.AccountLibraryFile.Value } : new long[0],
                command.AccountLibraryMode,
                machineConfig.AccountLibraryFile.HasValue
                    ? new[] { machineConfig.AccountLibraryFile.Value }
                    : new long[0]);

            if (accountLibraryFiles.Any())
            {
                machineConfig.AccountLibraryFile = accountLibraryFiles.FirstOrDefault(x =>
                {
                    var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                    return file != null;
                });

                machineConfig.AccountLibraryMode = LibraryFileModes.Select;
            }
            else
            {
                machineConfig.AccountLibraryFile = null;
                machineConfig.AccountLibraryMode = LibraryFileModes.None;
            }
        }

        private async Task UpdateMachine(Machine machine, UpdateInstanceSettingsCommand command,
            CancellationToken cancellationToken)
        {
            var machineConfig = machine.Account.MachineConfig;

            var currentDesiredState = await Context.Set<State>()
                .Where(x => x.MachineId == machine.Id && x.Desired)
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync(cancellationToken);

            var newDesiredState = new State()
            {
                MachineId = machine.Id,
                Timestamp = DateTimeOffset.Now,
                SslEnabled = currentDesiredState?.SslEnabled ?? machineConfig.EnableSsl,
                MonitoringEnabled = currentDesiredState?.MonitoringEnabled ?? machineConfig.ShowInGrafana,
                Desired = true,
                Locked = false,
                SiteMasterBackup = currentDesiredState?.SiteMasterBackup,
                LauncherBackup = currentDesiredState?.LauncherBackup
            };

            if (machine.IsLauncher)
            {
                var launcherVersion = _versionResolver.Resolve(Software.Launcher, command.LauncherVersionMode,
                    command.LauncherVersion, currentDesiredState?.Launcher);
                var reportingVersion = _versionResolver.Resolve(Software.Reporting, command.ReportingVersionMode,
                    command.ReportingVersion, currentDesiredState?.Reporting);
                var pdfExportVersion = _versionResolver.Resolve(Software.PdfExport, command.PdfExportVersionMode,
                    command.PdfExportVersion, currentDesiredState?.PdfExport);

                newDesiredState.Launcher = launcherVersion.IsNone() ? Versions.None : launcherVersion.Version;
                newDesiredState.Reporting = reportingVersion.IsNone() ? Versions.None : reportingVersion.Version;
                newDesiredState.PdfExport = pdfExportVersion.IsNone() ? Versions.None : pdfExportVersion.Version;
            }
            else
            {
                newDesiredState.Launcher = Versions.None;
                newDesiredState.Reporting = Versions.None;
                newDesiredState.PdfExport = Versions.None;
            }

            if (machine.IsSiteMaster)
            {
                var clientVersion = _versionResolver.Resolve(Software.Client, command.ClientVersionMode,
                    command.ClientVersion, currentDesiredState?.Client);
                var siteMasterVersion = _versionResolver.Resolve(Software.SiteMaster, command.SiteMasterVersionMode,
                    command.SiteMasterVersion, currentDesiredState?.SiteMaster);
                var relExportVersion = _versionResolver.Resolve(Software.SqlExport, command.SqlExportVersionMode,
                    command.SqlExportVersion, currentDesiredState?.RelExport); //TODO: To be removed
                var sqlExportVersion = _versionResolver.Resolve(Software.SqlExport, command.SqlExportVersionMode,
                    command.SqlExportVersion, currentDesiredState?.SqlExport);
                var linkwareVersion = _versionResolver.Resolve(Software.Linkware, command.LinkwareVersionMode,
                    command.LinkwareVersion, currentDesiredState?.Linkware);
                var smchkVersion = _versionResolver.Resolve(Software.Smchk, command.SmchkVersionMode,
                    command.SmchkVersion, currentDesiredState?.Smchk);
                var discoveryVersion = _versionResolver.Resolve(Software.Discovery, command.DiscoveryVersionMode,
                    command.DiscoveryVersion, currentDesiredState?.Discovery);
                var fiberSenSysVersion = _versionResolver.Resolve(Software.FiberSenSys, command.FiberSenSysVersionMode,
                    command.FiberSenSysVersion, currentDesiredState?.FiberSenSys);
                var fiberMountainVersion = _versionResolver.Resolve(Software.FiberMountain,
                    command.FiberMountainVersionMode, command.FiberMountainVersion, currentDesiredState?.FiberMountain);
                var serviceNowVersion = _versionResolver.Resolve(Software.ServiceNow,
                    command.ServiceNowVersionMode, command.ServiceNowVersion, currentDesiredState?.ServiceNow);
                var commScopeVersion = _versionResolver.Resolve(Software.CommScope,
                    command.CommScopeVersionMode, command.CommScopeVersion, currentDesiredState?.CommScope);

                newDesiredState.SiteMaster = siteMasterVersion.IsNone() ? Versions.None : siteMasterVersion.Version;
                newDesiredState.RelExport =
                    relExportVersion.IsNone() ? Versions.None : relExportVersion.Version; //TODO: To be removed
                newDesiredState.SqlExport = sqlExportVersion.IsNone() ? Versions.None : sqlExportVersion.Version;
                newDesiredState.Client = clientVersion.IsNone() ? Versions.None : clientVersion.Version;
                newDesiredState.Linkware = linkwareVersion.IsNone() ? Versions.None : linkwareVersion.Version;
                newDesiredState.Smchk = smchkVersion.IsNone() ? Versions.None : smchkVersion.Version;
                newDesiredState.Discovery = discoveryVersion.IsNone() ? Versions.None : discoveryVersion.Version;
                newDesiredState.FiberSenSys = fiberSenSysVersion.IsNone() ? Versions.None : fiberSenSysVersion.Version;
                newDesiredState.FiberMountain =
                    fiberMountainVersion.IsNone() ? Versions.None : fiberMountainVersion.Version;
                newDesiredState.ServiceNow = serviceNowVersion.IsNone() ? Versions.None : serviceNowVersion.Version;
                newDesiredState.CommScope = commScopeVersion.IsNone() ? Versions.None : commScopeVersion.Version;
            }
            else
            {
                newDesiredState.SiteMaster = Versions.None;
                newDesiredState.Smchk = Versions.None;
                newDesiredState.RelExport = Versions.None;
                newDesiredState.SqlExport = Versions.None;
                newDesiredState.Client = Versions.None;
                newDesiredState.Linkware = Versions.None;
                newDesiredState.Discovery = Versions.None;
                newDesiredState.FiberSenSys = Versions.None;
                newDesiredState.FiberMountain = Versions.None;
                newDesiredState.ServiceNow = Versions.None;
                newDesiredState.CommScope = Versions.None;
            }

            var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances)
                .FirstOrDefaultAsync(x => x.Id == machine.ClassId, cancellationToken);
            var deployerVersion = _versionResolver.Resolve(Software.Deployer, command.DeployerVersionMode,
                command.DeployerVersion, currentDesiredState?.Deployer, mmaClass?.MmaInstance?.DeployerVer);
            var populateVersion = _versionResolver.Resolve(Software.Populate, command.PopulateVersionMode,
                command.PopulateVersion, currentDesiredState?.Populate);

            newDesiredState.Populate = populateVersion.IsNone() ? Versions.None : populateVersion.Version;
            newDesiredState.Deployer = deployerVersion.IsNone() ? Versions.None : deployerVersion.Version;

            var libraryFiles = _versionResolver.GetLibraryFiles(command.MainLibraryFiles, command.MainLibraryMode,
                currentDesiredState?.LibraryFiles);
            newDesiredState.LibraryFiles = libraryFiles;
            newDesiredState.LibraryFile = libraryFiles.Any()
                ? libraryFiles.FirstOrDefault(x =>
                {
                    var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                    return file != null;
                })
                : 0;

            newDesiredState.AccountLibraryFile = _versionResolver.GetLibraryFiles(
                command.AccountLibraryFile.HasValue ? new[] { command.AccountLibraryFile.Value } : new long[0],
                command.AccountLibraryMode,
                (currentDesiredState != null && currentDesiredState.AccountLibraryFile.HasValue)
                    ? new[] { currentDesiredState.AccountLibraryFile.Value }
                    : Array.Empty<long>()).FirstOrDefault(x =>
                    {
                        var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                        return file != null;
                    });

            newDesiredState.MonitoringEnabled = command.ShowInGrafana;
            newDesiredState.Timestamp = DateTimeOffset.Now;

            machine.VmImageName = command.VmImageName;
            machine.AutoStopTime = command.AutoStopTime;
            machine.Turbo = true;

            machine.SetOperationModeToNormal();

            Context.Set<State>().Add(newDesiredState);
        }
    }
}
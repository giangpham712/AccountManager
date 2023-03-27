using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Machine;
using MediatR;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForAccounts
{
    public class UpdateSoftwareForAccountsCommandHandler : CommandHandlerBase<UpdateSoftwareForAccountsCommand, Unit>
    {
        private readonly ISoftwareVersionResolver _versionResolver;

        public UpdateSoftwareForAccountsCommandHandler(IMediator mediator, ICloudStateDbContext context,
            ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _versionResolver = versionResolver;
        }

        public override async Task<Unit> Handle(UpdateSoftwareForAccountsCommand command,
            CancellationToken cancellationToken)
        {
            var transaction = Context.GetTransaction();
            try
            {
                var accounts = await Context.Set<Account>().Include(x => x.MachineConfig)
                    .Where(x => command.Accounts.Contains(x.Id)).ToListAsync(cancellationToken);
                foreach (var account in accounts)
                {
                    var machineConfig = account.MachineConfig;

                    if (machineConfig == null)
                        continue;

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

                    var fiberSenSysVersion = _versionResolver.Resolve(Software.FiberSenSys,
                        command.FiberSenSysVersionMode,
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
                        command.CommScopeVersionMode,
                        command.CommScopeVersion, machineConfig.CommScopeVersion);
                    machineConfig.CommScopeVersion = commScopeVersion?.ToString();

                    var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances)
                        .FirstOrDefaultAsync(x => x.Id == machineConfig.Account.ClassId, cancellationToken);
                    var deployerVersion = _versionResolver.Resolve(Software.Deployer, command.DeployerVersionMode,
                        command.DeployerVersion, machineConfig.DeployerVersion, mmaClass?.MmaInstance.DeployerVer);
                    machineConfig.DeployerVersion = deployerVersion?.ToString();

                    var mainLibraryFiles = _versionResolver.GetLibraryFiles(command.MainLibraryFiles,
                        command.MainLibraryMode, machineConfig.MainLibraryFiles);
                    machineConfig.MainLibraryFiles = mainLibraryFiles;
                    machineConfig.MainLibraryFile = mainLibraryFiles.FirstOrDefault(x =>
                    {
                        var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                        return file != null;
                    });

                    machineConfig.MainLibraryMode = machineConfig.MainLibraryFiles != null && machineConfig.MainLibraryFiles.Any()
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

                    await account.SetLastUserCycle(Context);

                    var machines = Context.Set<Machine>()
                        .Include(x => x.States)
                        .Where(x => x.AccountId == account.Id).ToList();

                    foreach (var machine in machines) await UpdateMachine(machine, command, cancellationToken);
                }

                await Context.SaveChangesAsync(cancellationToken);

                transaction.Commit();

                await Mediator.Publish(new SoftwareUpdatedEvent
                {
                    Actor = command.User,
                    Accounts = accounts,
                    Command = command
                }, cancellationToken);

                return Unit.Value;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        private async Task UpdateMachine(Machine machine, UpdateSoftwareCommandBase command,
            CancellationToken cancellationToken)
        {
            var states = await Context.Set<State>().Where(x => x.MachineId == machine.Id)
                .ToListAsync(cancellationToken);
            var desiredState = states.FirstOrDefault(x => x.Desired);
            if (desiredState == null)
                return;

            if (machine.IsLauncher)
            {
                var launcherVersion = _versionResolver.Resolve(Software.Launcher, command.LauncherVersionMode,
                    command.LauncherVersion, desiredState.Launcher);
                var reportingVersion = _versionResolver.Resolve(Software.Reporting, command.ReportingVersionMode,
                    command.ReportingVersion, desiredState.Reporting);
                var pdfExportVersion = _versionResolver.Resolve(Software.PdfExport, command.PdfExportVersionMode,
                    command.PdfExportVersion, desiredState.PdfExport);

                desiredState.Launcher = launcherVersion.IsNone() ? Versions.None : launcherVersion.Version;
                desiredState.Reporting = reportingVersion.IsNone() ? Versions.None : reportingVersion.Version;
                desiredState.PdfExport = pdfExportVersion.IsNone() ? Versions.None : pdfExportVersion.Version;
            }
            else
            {
                desiredState.Launcher = Versions.None;
                desiredState.Reporting = Versions.None;
                desiredState.PdfExport = Versions.None;
            }

            if (machine.IsSiteMaster)
            {
                var clientVersion = _versionResolver.Resolve(Software.Client, command.ClientVersionMode,
                    command.ClientVersion, desiredState.Client);
                var siteMasterVersion = _versionResolver.Resolve(Software.SiteMaster, command.SiteMasterVersionMode,
                    command.SiteMasterVersion, desiredState.SiteMaster);
                var relExportVersion = _versionResolver.Resolve(Software.SqlExport, command.SqlExportVersionMode,
                    command.SqlExportVersion, desiredState.RelExport); //TODO: To be removed
                var sqlExportVersion = _versionResolver.Resolve(Software.SqlExport, command.SqlExportVersionMode,
                    command.SqlExportVersion, desiredState.SqlExport);
                var linkwareVersion = _versionResolver.Resolve(Software.Linkware, command.LinkwareVersionMode,
                    command.LinkwareVersion, desiredState.Linkware);
                var smchkVersion = _versionResolver.Resolve(Software.Smchk, command.SmchkVersionMode,
                    command.SmchkVersion, desiredState.Smchk);
                var discoveryVersion = _versionResolver.Resolve(Software.Discovery, command.DiscoveryVersionMode,
                    command.DiscoveryVersion, desiredState.Discovery);
                var fiberSenSysVersion = _versionResolver.Resolve(Software.FiberSenSys, command.FiberSenSysVersionMode,
                    command.FiberSenSysVersion, desiredState.FiberSenSys);
                var fiberMountainVersion = _versionResolver.Resolve(Software.FiberMountain,
                    command.FiberMountainVersionMode, command.FiberMountainVersion, desiredState.FiberMountain);
                var serviceNowVersion = _versionResolver.Resolve(Software.ServiceNow,
                    command.ServiceNowVersionMode, command.ServiceNowVersion, desiredState.ServiceNow);
                var commScopeVersion = _versionResolver.Resolve(Software.CommScope,
                    command.CommScopeVersionMode, command.CommScopeVersion, desiredState.CommScope);

                desiredState.SiteMaster = siteMasterVersion.IsNone() ? Versions.None : siteMasterVersion.Version;
                desiredState.Smchk = smchkVersion.IsNone() ? Versions.None : smchkVersion.Version;
                desiredState.RelExport =
                    relExportVersion.IsNone() ? Versions.None : relExportVersion.Version; //TODO: To be removed
                desiredState.SqlExport = sqlExportVersion.IsNone() ? Versions.None : sqlExportVersion.Version;
                desiredState.Client = clientVersion.IsNone() ? Versions.None : clientVersion.Version;
                desiredState.Linkware = linkwareVersion.IsNone() ? Versions.None : linkwareVersion.Version;
                desiredState.Discovery = discoveryVersion.IsNone() ? Versions.None : discoveryVersion.Version;
                desiredState.FiberSenSys = fiberSenSysVersion.IsNone() ? Versions.None : fiberSenSysVersion.Version;
                desiredState.FiberMountain =
                    fiberMountainVersion.IsNone() ? Versions.None : fiberMountainVersion.Version;
                desiredState.ServiceNow = serviceNowVersion.IsNone() ? Versions.None : serviceNowVersion.Version;
                desiredState.CommScope = commScopeVersion.IsNone() ? Versions.None : commScopeVersion.Version;
            }
            else
            {
                desiredState.SiteMaster = Versions.None;
                desiredState.Smchk = Versions.None;
                desiredState.RelExport = Versions.None;
                desiredState.SqlExport = Versions.None;
                desiredState.Client = Versions.None;
                desiredState.Linkware = Versions.None;
                desiredState.Discovery = Versions.None;
                desiredState.FiberSenSys = Versions.None;
                desiredState.FiberMountain = Versions.None;
                desiredState.ServiceNow = Versions.None;
                desiredState.CommScope = Versions.None;
            }

            var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances)
                .FirstOrDefaultAsync(x => x.Id == machine.ClassId);
            var deployerVersion = _versionResolver.Resolve(Software.Deployer, command.DeployerVersionMode,
                command.DeployerVersion, desiredState.Deployer, mmaClass?.MmaInstance?.DeployerVer);
            var populateVersion = _versionResolver.Resolve(Software.Populate, command.PopulateVersionMode,
                command.PopulateVersion, desiredState.Populate);

            desiredState.Populate = populateVersion.IsNone() ? Versions.None : populateVersion.Version;
            desiredState.Deployer = deployerVersion.IsNone() ? Versions.None : deployerVersion.Version;

            var libraryFiles = _versionResolver.GetLibraryFiles(command.MainLibraryFiles, command.MainLibraryMode, desiredState.LibraryFiles);
            desiredState.LibraryFiles = libraryFiles;
            desiredState.LibraryFile = libraryFiles.Any()
                ? libraryFiles.FirstOrDefault(x =>
                {
                    var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                    return file != null;
                })
                : 0;

            desiredState.AccountLibraryFile = _versionResolver.GetLibraryFiles(
                command.AccountLibraryFile.HasValue ? new[] { command.AccountLibraryFile.Value } : new long[0],
                command.AccountLibraryMode,
                desiredState.AccountLibraryFile.HasValue
                    ? new[] { desiredState.AccountLibraryFile.Value }
                    : new long[0]).FirstOrDefault(x =>
            {
                var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                return file != null;
            });

            machine.Turbo = true;
            machine.SetOperationModeToNormal();
        }
    }
}
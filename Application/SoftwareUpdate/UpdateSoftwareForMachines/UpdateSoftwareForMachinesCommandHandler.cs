using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Machine;
using MediatR;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForMachines
{
    public class UpdateSoftwareForMachinesCommandHandler : CommandHandlerBase<UpdateSoftwareForMachinesCommand, Unit>
    {
        private readonly ISoftwareVersionResolver _versionResolver;


        public UpdateSoftwareForMachinesCommandHandler(IMediator mediator, ICloudStateDbContext context,
            ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _versionResolver = versionResolver;
        }

        public override async Task<Unit> Handle(UpdateSoftwareForMachinesCommand command,
            CancellationToken cancellationToken)
        {
            var machines = await Context.Set<Machine>()
                .Include(x => x.Account)
                .Include(x => x.States).Where(x => command.Machines.Contains(x.Id)).ToListAsync(cancellationToken);

            var accounts = new HashSet<Account>();

            foreach (var machine in machines)
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
                    var fiberSenSysVersion = _versionResolver.Resolve(Software.FiberSenSys,
                        command.FiberSenSysVersionMode, command.FiberSenSysVersion, currentDesiredState?.FiberSenSys);
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
                    newDesiredState.RelExport = Versions.None; //TODO: To be removed
                    newDesiredState.SqlExport = Versions.None;
                    newDesiredState.Client = Versions.None;
                    newDesiredState.Linkware = Versions.None;
                    newDesiredState.Smchk = Versions.None;
                    newDesiredState.Discovery = Versions.None;
                    newDesiredState.FiberSenSys = Versions.None;
                    newDesiredState.FiberMountain = Versions.None;
                    newDesiredState.ServiceNow = Versions.None;
                    newDesiredState.CommScope = Versions.None;
                }

                var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances)
                    .FirstOrDefaultAsync(x => x.Id == machine.ClassId);
                var deployerVersion = _versionResolver.Resolve(Software.Deployer, command.DeployerVersionMode,
                    command.DeployerVersion, newDesiredState.Deployer, mmaClass?.MmaInstance?.DeployerVer);
                var populateVersion = _versionResolver.Resolve(Software.Populate, command.PopulateVersionMode,
                    command.PopulateVersion, newDesiredState.Populate);

                newDesiredState.Populate = populateVersion.IsNone() ? Versions.None : populateVersion.Version;
                newDesiredState.Deployer = deployerVersion.IsNone() ? Versions.None : deployerVersion.Version;

                var libraryFiles = _versionResolver.GetLibraryFiles(command.MainLibraryFiles, command.MainLibraryMode,
                    newDesiredState.LibraryFiles);
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
                    newDesiredState.AccountLibraryFile.HasValue
                        ? new[] { newDesiredState.AccountLibraryFile.Value }
                        : new long[0]).FirstOrDefault(x =>
                        {
                            var file = Context.Set<File>().FirstOrDefault(y => y.Id == x);
                            return file != null;
                        });

                machine.Turbo = true;
                machine.SetOperationModeToNormal();

                Context.Set<State>().Add(newDesiredState);
                accounts.Add(machine.Account);
            }

            foreach (var account in accounts) await account.SetLastUserCycle(Context);

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new SoftwareUpdatedEvent
            {
                Actor = command.User,
                Machines = machines,
                Command = command
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
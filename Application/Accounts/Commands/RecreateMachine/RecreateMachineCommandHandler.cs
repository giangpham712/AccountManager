using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Accounts.Commands.RecreateMachine
{
    public class RecreateMachineCommandHandler : CommandHandlerBase<RecreateMachineCommand, Unit>
    {
        private readonly ISoftwareVersionResolver _versionResolver;

        public RecreateMachineCommandHandler(IMediator mediator, ICloudStateDbContext context,
            ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _versionResolver = versionResolver;
        }

        public override async Task<Unit> Handle(RecreateMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = Context.Set<Machine>()
                .Find(command.MachineId);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), command.MachineId);

            var account = await Context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.MachineConfig)
                .Include(x => x.LicenseConfig)
                .FirstOrDefaultAsync(x => x.Id == machine.AccountId && !x.IsDeleted, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), machine.AccountId);

            machine.Terminate = true;

            var machineConfig = account.MachineConfig;

            var launcherVersion = new VersionInfo(machineConfig.LauncherVersion);
            var reportingVersion = new VersionInfo(machineConfig.ReportingVersion);
            var pdfExportVersion = new VersionInfo(machineConfig.PdfExportVersion);
            var clientVersion = new VersionInfo(machineConfig.ClientVersion);
            var siteMasterVersion = new VersionInfo(machineConfig.SiteMasterVersion);
            var sqlExportVersion = new VersionInfo(machineConfig.SqlExportVersion);
            var linkwareVersion = new VersionInfo(machineConfig.LinkwareVersion);
            var smchkVersion = new VersionInfo(machineConfig.SmchkVersion);
            var discoveryVersion = new VersionInfo(machineConfig.DiscoveryVersion);
            var fiberSenSysVersion = new VersionInfo(machineConfig.FiberSenSysVersion);
            var fiberMountainVersion = new VersionInfo(machineConfig.FiberMountainVersion);
            var serviceNowVersion = new VersionInfo(machineConfig.ServiceNowVersion);
            var commScopeVersion = new VersionInfo(machineConfig.CommScopeVersion);

            var deployerVersion = new VersionInfo(machineConfig.DeployerVersion);
            var populateVersion = new VersionInfo(machineConfig.PopulateVersion);

            var newState = new State
            {
                Launcher = launcherVersion.IsNone() ? Versions.None : launcherVersion.Version,
                Reporting = reportingVersion.IsNone() ? Versions.None : reportingVersion.Version,
                PdfExport = pdfExportVersion.IsNone() ? Versions.None : pdfExportVersion.Version,
                Client = clientVersion.IsNone() ? Versions.None : clientVersion.Version,
                SiteMaster = siteMasterVersion.IsNone() ? Versions.None : siteMasterVersion.Version,
                Deployer = deployerVersion.IsNone() ? Versions.None : deployerVersion.Version,

                SqlExport = sqlExportVersion.IsNone() ? Versions.None : sqlExportVersion.Version,
                Populate = populateVersion.IsNone() ? Versions.None : populateVersion.Version,

                Linkware = linkwareVersion.IsNone() ? Versions.None : linkwareVersion.Version,
                Smchk = smchkVersion.IsNone() ? Versions.None : smchkVersion.Version,

                Discovery = discoveryVersion.IsNone() ? Versions.None : discoveryVersion.Version,
                FiberSenSys = fiberSenSysVersion.IsNone() ? Versions.None : fiberSenSysVersion.Version,
                FiberMountain = fiberMountainVersion.IsNone() ? Versions.None : fiberMountainVersion.Version,
                ServiceNow = serviceNowVersion.IsNone() ? Versions.None : serviceNowVersion.Version,
                CommScope = commScopeVersion.IsNone() ? Versions.None : commScopeVersion.Version,

                LibraryFile = machineConfig.MainLibraryFile ?? 0,
                LibraryFiles = machineConfig.MainLibraryFiles,
                AccountLibraryFile = machineConfig.AccountLibraryFile ?? 0,

                Locked = false,
                Desired = true,
                SslEnabled = machineConfig.EnableSsl,
                MonitoringEnabled = machineConfig.ShowInGrafana,

                Timestamp = DateTimeOffset.Now
            };

            var newMachine = new Machine
            {
                Name = machine.Name,
                Account = account,
                CloudInstanceTypeId = account.LicenseConfig.CloudInstanceType,
                IsLauncher = machine.IsLauncher,
                CreationMailSent = false,
                IsSiteMaster = machine.IsSiteMaster,
                MailTo = account.Contact.Email,
                RdpUsers = Array.Empty<int>(),
                ClassId = machine.ClassId,
                Managed = account.Managed,
                Region = machineConfig.Region,
                VmImageName = machine.VmImageName,
                SiteName = machine.SiteName,
                SiteMasterUrl = machine.SiteMasterUrl,
                States = new List<State> { newState },
                SampleData = machineConfig.IncludeSampleData,
                AutoStopTime = machineConfig.AutoStopTime,
                Turbo = true
            };

            var site = await Context.Set<Site>()
                .Include(x => x.Machine)
                .FirstOrDefaultAsync(x => x.Machine != null && x.Machine.Id == machine.Id, cancellationToken);

            account.Machines.Add(newMachine);

            await account.SetLastUserCycle(Context);

            if (site != null) site.Machine = newMachine;

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineRecreatedEvent
            {
                User = command.User,
                Machine = machine,
                Command = command
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
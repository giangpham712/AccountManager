using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Utils;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Accounts.Commands.CreateMachine
{
    public class CreateMachineCommandHandler : CommandHandlerBase<CreateMachineCommand, Unit>
    {
        public CreateMachineCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator,
            context)
        {
        }

        public override async Task<Unit> Handle(CreateMachineCommand command, CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Include(x => x.Sites)
                .FirstOrDefaultAsync(x => !x.IsDeleted && !x.IsTemplate && x.Id == command.AccountId,
                    cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountId);

            var machines = await Context.Set<Machine>().Where(x => x.AccountId == account.Id)
                .ToListAsync(cancellationToken);

            if (command.IsLauncher)
            {
                var launcherMachine = machines.FirstOrDefault(x => x.IsLauncher);
                if (launcherMachine != null)
                    throw new CommandException();
            }

            Site site = null;
            if (command.SiteId.HasValue)
            {
                site = account.Sites.FirstOrDefault(x => x.Id == command.SiteId);
                if (site == null)
                    throw new CommandException();

                var siteMachine = machines.FirstOrDefault(x => x.SiteName == site.UrlFriendlyName);
                if (siteMachine != null)
                    throw new CommandException();
            }

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

            var machine = new Machine
            {
                Name = AccountHelper.GenerateMachineName(account.UrlFriendlyName, account.LicenseConfig.InstancePolicy),
                Account = account,
                CloudInstanceTypeId = account.LicenseConfig.CloudInstanceType,
                IsLauncher = command.IsLauncher,
                IsSiteMaster = command.SiteId.HasValue,
                CreationMailSent = false,
                MailTo = account.Contact.Email,
                RdpUsers = Array.Empty<int>(),
                ClassId = account.ClassId,
                Managed = account.Managed,
                Region = machineConfig.Region,
                VmImageName = machineConfig.VmImageName,
                SiteName = site?.UrlFriendlyName,
                SiteMasterUrl = site == null
                    ? null
                    : AccountHelper.GenerateSiteMasterUrl(account.UrlFriendlyName, site.UrlFriendlyName,
                        account.MachineConfig.EnableSsl),
                States = new List<State> { newState },
                SampleData = machineConfig.IncludeSampleData,
                SampleDataFile = machineConfig.SampleDataFile,
                Turbo = true
            };

            if (site != null)
                site.Machine = machine;

            Context.Set<Machine>().Add(machine);

            await Context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
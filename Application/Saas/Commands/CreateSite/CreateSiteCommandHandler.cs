using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Entities.Public;
using MediatR;
using Newtonsoft.Json;

namespace AccountManager.Application.Saas.Commands.CreateSite
{
    public class CreateSiteCommandHandler : CommandHandlerBase<CreateSiteCommand, Unit>
    {
        public CreateSiteCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator, context)
        {
        }

        public override async Task<Unit> Handle(CreateSiteCommand command, CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.UrlFriendlyName == command.AccountUrlFriendlyName,
                    cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountUrlFriendlyName);

            var machineConfig = account.MachineConfig;

            var site = new Site
            {
                Name = command.Name,
                UrlFriendlyName = command.UrlFriendlyName,
                CloudInstanceType = account.LicenseConfig.CloudInstanceType,
                Account = account,
                AccountId = account.Id
            };

            Context.Set<Site>().Add(site);

            var siteMasterVersion = new VersionInfo(machineConfig.SiteMasterVersion);
            var clientVersion = new VersionInfo(machineConfig.ClientVersion);
            var sqlExportVersion = new VersionInfo(machineConfig.SqlExportVersion);
            var deployerVersion = new VersionInfo(machineConfig.DeployerVersion);
            var populateVersion = new VersionInfo(machineConfig.PopulateVersion);
            var linkwareVersion = new VersionInfo(machineConfig.LinkwareVersion);
            var smchkVersion = new VersionInfo(machineConfig.SmchkVersion);
            var discoveryVersion = new VersionInfo(machineConfig.DiscoveryVersion);
            var fiberSenSysVersion = new VersionInfo(machineConfig.FiberSenSysVersion);

            var state = new State
            {
                Launcher = Versions.None,
                SiteMaster = siteMasterVersion.IsNone() ? Versions.None : siteMasterVersion.Version,
                Client = clientVersion.IsNone() ? Versions.None : clientVersion.Version,
                RelExport = sqlExportVersion.IsNone() ? Versions.None : sqlExportVersion.Version, //TODO: To be removed
                SqlExport = sqlExportVersion.IsNone() ? Versions.None : sqlExportVersion.Version,
                Deployer = deployerVersion.IsNone() ? Versions.None : deployerVersion.Version,
                PdfExport = Versions.None,
                Linkware = linkwareVersion.IsNone() ? Versions.None : linkwareVersion.Version,
                Smchk = smchkVersion.IsNone() ? Versions.None : smchkVersion.Version,
                Discovery = discoveryVersion.IsNone() ? Versions.None : discoveryVersion.Version,
                FiberSenSys = fiberSenSysVersion.IsNone() ? Versions.None : fiberSenSysVersion.Version,
                Populate = populateVersion.IsNone() ? Versions.None : populateVersion.Version,
                Reporting = Versions.None,
                LibraryFile = machineConfig.MainLibraryFile ?? 0,
                LibraryFiles = machineConfig.MainLibraryFiles,
                AccountLibraryFile = machineConfig.AccountLibraryFile ?? 0,

                Locked = false,
                Desired = true,
                SslEnabled = machineConfig.EnableSsl
            };

            var cloudInstanceType = await Context.Set<CloudInstanceType>()
                .FirstOrDefaultAsync(x => x.CloudCode == command.CloudInstanceType, cancellationToken);

            var machine = new Machine
            {
                IsLauncher = false,
                IsSiteMaster = true,
                CreationMailSent = false,
                CloudInstanceTypeId = cloudInstanceType?.Id ?? account.LicenseConfig.CloudInstanceType,
                MailTo = account.Contact.Email,
                RdpUsers = Array.Empty<int>(),
                States = new List<State> { state },
                ClassId = account.ClassId,
                Managed = account.Managed,
                Turbo = true,
                Region = machineConfig.Region,
                VmImageName = machineConfig.VmImageName,
                Account = account,
                AccountId = account.Id,
                SampleData = machineConfig.IncludeSampleData,
                SampleDataFile = machineConfig.SampleDataFile,
                SiteName = site.UrlFriendlyName,
                SiteMasterUrl =
                    $"{(machineConfig.EnableSsl ? "https" : "http")}://{site.UrlFriendlyName}.{account.UrlFriendlyName}.planetassociates.net",
                Name = $"MMA {account.UrlFriendlyName} IPSM SiteMaster {command.UrlFriendlyName}",
                AutoStopTime = machineConfig.AutoStopTime
            };

            Context.Set<Machine>().Add(machine);

            site.Machine = machine;

            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new SiteCreatedEvent
            {
                Actor = command.User,
                Site = site,
                Command = command
            }, cancellationToken);

            return await Task.FromResult(Unit.Value);
        }
    }
}
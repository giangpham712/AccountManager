using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Configs;
using AccountManager.Application.Key;
using AccountManager.Application.License;
using AccountManager.Application.Utils;
using AccountManager.Common.Extensions;
using AccountManager.Common.Utils;
using AccountManager.Domain;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;

namespace AccountManager.Application.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommandHandler : CommandHandlerBase<CreateAccountCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly IKeysManager _keysManager;
        private readonly ISoftwareVersionResolver _versionResolver;
        private readonly ILicenseGenerator _licenseGenerator;
        private readonly IConfigGenerator _configGenerator;

        public CreateAccountCommandHandler(
            IMediator mediator, 
            ICloudStateDbContext context,
            IKeysManager keysManager, 
            ISoftwareVersionResolver versionResolver, 
            IMapper mapper, 
            ILicenseGenerator licenseGenerator, 
            IConfigGenerator configGenerator) : base(mediator, context)
        {
            _keysManager = keysManager;
            _versionResolver = versionResolver;
            _mapper = mapper;
            _licenseGenerator = licenseGenerator;
            _configGenerator = configGenerator;
        }

        public override async Task<long> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
        {
            var account = _mapper.Map<Account>(command);

            account.IsActive = true;
            account.CreationTime = DateTimeOffset.Now;
            account.Creator = command.User;

            if (command.ClassId == 200) // Add-on account
            {
                account.Managed = false;
                account.Keys = null;
            }
            else
            {
                var userTokenKeyPair = _keysManager.GenerateUserTokenKeyPair(command.KeyType == KeyType.Default);
                var interServerKeyPair = _keysManager.GenerateInterServerKeyPair(command.KeyType == KeyType.Default);
                var licenseKeyPair = _keysManager.GenerateLicenseKeyPair(command.KeyType == KeyType.Default);

                var apiPassword = StringUtils.GeneratePassword(16);
                account.Keys = new Keys
                {
                    UserTokenPrivate = userTokenKeyPair.PrivateKey,
                    UserTokenPublic = userTokenKeyPair.PublicKey,
                    InterServerPrivate = interServerKeyPair.PrivateKey,
                    InterServerPublic = interServerKeyPair.PublicKey,
                    LicensePrivate = licenseKeyPair.PrivateKey,
                    LicensePublic = licenseKeyPair.PublicKey,
                    ApiPassword = apiPassword,
                    AccountFile = $"{account.UrlFriendlyName}:{apiPassword}".ToBase64(),
                    SqlExportPass = StringUtils.GeneratePassword(24)
                };

                var licenseBytes =
                    _licenseGenerator.GenerateLicense(account.LicenseConfig, null, account.Keys.LicensePrivate);
                account.License = Convert.ToBase64String(licenseBytes);

                var machineConfig = account.MachineConfig;
                machineConfig.UseUniqueKey = command.KeyType != KeyType.Default;
                var launcherVersion = _versionResolver.Resolve(Software.Launcher, command.LauncherVersionMode,
                    command.LauncherVersion);
                machineConfig.LauncherVersion = launcherVersion?.ToString();

                var reportingVersion = _versionResolver.Resolve(Software.Reporting, command.ReportingVersionMode,
                    command.ReportingVersion);
                machineConfig.ReportingVersion = reportingVersion?.ToString();

                var clientVersion =
                    _versionResolver.Resolve(Software.Client, command.ClientVersionMode, command.ClientVersion);
                machineConfig.ClientVersion = clientVersion?.ToString();

                var siteMasterVersion = _versionResolver.Resolve(Software.SiteMaster, command.SiteMasterVersionMode,
                    command.SiteMasterVersion);
                machineConfig.SiteMasterVersion = siteMasterVersion?.ToString();

                var pdfExportVersion = _versionResolver.Resolve(Software.PdfExport, command.PdfExportVersionMode,
                    command.PdfExportVersion);
                machineConfig.PdfExportVersion = pdfExportVersion?.ToString();

                var sqlExportVersion = _versionResolver.Resolve(Software.SqlExport, command.SqlExportVersionMode,
                    command.SqlExportVersion);
                machineConfig.SqlExportVersion = sqlExportVersion?.ToString();

                var populateVersion = _versionResolver.Resolve(Software.Populate, command.PopulateVersionMode,
                    command.PopulateVersion);
                machineConfig.PopulateVersion = populateVersion?.ToString();

                var linkwareVersion = _versionResolver.Resolve(Software.Linkware, command.LinkwareVersionMode,
                    command.LinkwareVersion);
                machineConfig.LinkwareVersion = linkwareVersion?.ToString();

                var smchkVersion =
                    _versionResolver.Resolve(Software.Smchk, command.SmchkVersionMode, command.SmchkVersion);
                machineConfig.SmchkVersion = smchkVersion?.ToString();

                var discoveryVersion = _versionResolver.Resolve(Software.Discovery, command.DiscoveryVersionMode,
                    command.DiscoveryVersion);
                machineConfig.DiscoveryVersion = discoveryVersion?.ToString();

                var fiberSenSysVersion = _versionResolver.Resolve(Software.FiberSenSys, command.FiberSenSysVersionMode,
                    command.FiberSenSysVersion);
                machineConfig.FiberSenSysVersion = fiberSenSysVersion?.ToString();

                var fiberMountainVersion = _versionResolver.Resolve(Software.FiberMountain,
                    command.FiberMountainVersionMode, command.FiberMountainVersion);
                machineConfig.FiberMountainVersion = fiberMountainVersion?.ToString();

                var serviceNowVersion = _versionResolver.Resolve(Software.ServiceNow,
                    command.ServiceNowVersionMode, command.ServiceNowVersion);
                machineConfig.ServiceNowVersion = serviceNowVersion?.ToString();

                var commScopeVersion = _versionResolver.Resolve(Software.CommScope,
                    command.CommScopeVersionMode, command.CommScopeVersion);
                machineConfig.CommScopeVersion = commScopeVersion?.ToString();

                var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances)
                    .FirstOrDefaultAsync(x => x.Id == command.ClassId, cancellationToken);

                var deployerVersion = _versionResolver.Resolve(Software.Deployer, command.DeployerVersionMode,
                    command.DeployerVersion, null, mmaClass?.MmaInstance?.DeployerVer);
                machineConfig.DeployerVersion = deployerVersion?.ToString();

                var mainLibraryFiles = _versionResolver.GetLibraryFiles(command.MainLibraryFiles,
                    command.MainLibraryMode, machineConfig.MainLibraryFiles);
                machineConfig.MainLibraryFiles = mainLibraryFiles;
                machineConfig.MainLibraryFile = mainLibraryFiles.FirstOrDefault();
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
                    machineConfig.AccountLibraryFile = accountLibraryFiles.First();
                    machineConfig.AccountLibraryMode = LibraryFileModes.Select;
                }
                else
                {
                    machineConfig.AccountLibraryFile = null;
                    machineConfig.AccountLibraryMode = LibraryFileModes.None;
                }

                var state = new State
                {
                    Launcher = launcherVersion.IsNone() ? Versions.None : launcherVersion.Version,
                    Reporting = reportingVersion.IsNone() ? Versions.None : reportingVersion.Version,
                    PdfExport = pdfExportVersion.IsNone() ? Versions.None : pdfExportVersion.Version,
                    Client = clientVersion.IsNone() ? Versions.None : clientVersion.Version,
                    SiteMaster = siteMasterVersion.IsNone() ? Versions.None : siteMasterVersion.Version,
                    Deployer = deployerVersion.IsNone() ? Versions.None : deployerVersion.Version,

                    //TODO: To be removed
                    RelExport = sqlExportVersion.IsNone() ? Versions.None : sqlExportVersion.Version,
                    SqlExport = sqlExportVersion.IsNone() ? Versions.None : sqlExportVersion.Version,
                    Populate = populateVersion.IsNone() ? Versions.None : populateVersion.Version,

                    Linkware = linkwareVersion.IsNone() ? Versions.None : linkwareVersion.Version,
                    Smchk = smchkVersion.IsNone() ? Versions.None : smchkVersion.Version,

                    Discovery = discoveryVersion.IsNone() ? Versions.None : discoveryVersion.Version,
                    FiberSenSys = fiberSenSysVersion.IsNone() ? Versions.None : fiberSenSysVersion.Version,
                    FiberMountain = fiberMountainVersion.IsNone() ? Versions.None : fiberMountainVersion.Version,
                    ServiceNow = serviceNowVersion.IsNone() ? Versions.None : serviceNowVersion.Version,
                    CommScope = serviceNowVersion.IsNone() ? Versions.None : commScopeVersion.Version,

                    LibraryFile = machineConfig.MainLibraryFile ?? 0,
                    LibraryFiles = machineConfig.MainLibraryFiles,
                    AccountLibraryFile = machineConfig.AccountLibraryFile ?? 0,

                    Locked = false,
                    Desired = true,
                    SslEnabled = command.EnableSsl,
                    MonitoringEnabled = command.ShowInGrafana
                };

                var machine = new Machine
                {
                    CloudInstanceTypeId = command.CloudInstanceType,
                    IsLauncher = true,
                    CreationMailSent = false,
                    LauncherUrl = AccountHelper.GenerateLauncherUrl(account.UrlFriendlyName, command.EnableSsl),
                    MailTo = command.ContactEmail,
                    RdpUsers = Array.Empty<int>(),
                    States = new List<State> { state },
                    ClassId = command.ClassId,
                    Managed = command.Managed,
                    OverseeTermination = true,
                    Region = command.Region,
                    VmImageName = command.VmImageName,
                    SampleData = command.IncludeSampleData,
                    SampleDataFile = command.SampleDataFile,
                    RunSmokeTest = command.RunSmokeTest,
                    Turbo = true,
                    NextStopTime = machineConfig.AutoStopTime
                };
                account.Machines.Add(machine);

                var serverInstancePolicy = command.InstancePolicy;
                Site site = null;
                if (serverInstancePolicy == ServerInstancePolicy.AllInOne)
                {
                    site = new Site
                    {
                        Name = command.SiteMasterName,
                        UrlFriendlyName = command.SiteMasterUrlFriendlyName,
                        Port = command.SiteMasterPort,
                        CloudInstanceType = command.CloudInstanceType
                    };
                    account.Sites.Add(site);

                    machine.IsSiteMaster = true;
                    machine.SiteName = site.UrlFriendlyName;
                    machine.SiteMasterUrl = AccountHelper.GenerateSiteMasterUrl(account.UrlFriendlyName,
                        site.UrlFriendlyName, command.EnableSsl);
                    machine.Name = AccountHelper.GenerateMachineName(command.UrlFriendlyName, command.InstancePolicy);

                    site.Machine = machine;
                }
                else
                {
                    machine.Name =
                        AccountHelper.GenerateMachineName(command.UrlFriendlyName, command.InstancePolicy, "launcher");

                    state.SiteMaster = Versions.None;
                    state.Smchk = Versions.None;
                    state.Discovery = Versions.None;
                    state.FiberSenSys = Versions.None;
                    state.FiberMountain = Versions.None;
                    state.ServiceNow = Versions.None;
                    state.CommScope = Versions.None;
                    state.Linkware = Versions.None;
                    state.Client = Versions.None;
                    state.RelExport = Versions.None;
                    state.SqlExport = Versions.None;
                }

                #region Populate config for config.json

                machine.Config = new Config()
                {
                    ComponentConfigJson = JsonConvert.SerializeObject(await _configGenerator.GenerateComponentConfig(machine, site))
                };

                #endregion

            }

            Context.Set<Account>().Add(account);

            await Context.SaveChangesAsync(cancellationToken);

            if (command.ClassId != 200)
                await Mediator.Publish(new AccountCreatedEvent
                {
                    User = command.User,
                    Account = account,
                    Command = command
                }, cancellationToken);

            return account.Id;
        }
    }
}
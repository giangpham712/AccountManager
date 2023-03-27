using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Common.Extensions;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Machine;
using MediatR;

namespace AccountManager.Application.SoftwareUpdate.ValidateSoftwareUpdate
{
    public class
        ValidateSoftwareUpdateCommandHandler : CommandHandlerBase<ValidateSoftwareUpdateCommand,
            ValidateSoftwareUpdateResult>
    {
        private readonly ISoftwareVersionResolver _versionResolver;

        public ValidateSoftwareUpdateCommandHandler(IMediator mediator, ICloudStateDbContext context,
            ISoftwareVersionResolver versionResolver) : base(mediator, context)
        {
            _versionResolver = versionResolver;
        }

        public override async Task<ValidateSoftwareUpdateResult> Handle(ValidateSoftwareUpdateCommand command,
            CancellationToken cancellationToken)
        {
            var issues = new List<ValidationIssue>();
            var account = await Context.Set<Account>().Include(a => a.MachineConfig).AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == command.AccountId, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountId);

            var machineConfig = account.MachineConfig;

            var launcherVersion = _versionResolver.Resolve(Software.Launcher, command.LauncherVersionMode,
                command.LauncherVersion, machineConfig.LauncherVersion);
            var launcherValidationIssue = await ValidateUpdate(Software.Launcher, machineConfig.LauncherVersion,
                launcherVersion.Version);
            if (launcherValidationIssue != null) issues.Add(launcherValidationIssue);

            var reportingVersion = _versionResolver.Resolve(Software.Reporting, command.ReportingVersionMode,
                command.ReportingVersion, machineConfig.ReportingVersion);
            var reportingValidationIssue = await ValidateUpdate(Software.Reporting, machineConfig.ReportingVersion,
                reportingVersion.Version);
            if (reportingValidationIssue != null) issues.Add(reportingValidationIssue);

            var clientVersion = _versionResolver.Resolve(Software.Client, command.ClientVersionMode,
                command.ClientVersion, machineConfig.ClientVersion);
            var clientValidationIssue =
                await ValidateUpdate(Software.Client, machineConfig.ClientVersion, clientVersion.Version);
            if (clientValidationIssue != null) issues.Add(clientValidationIssue);

            var siteMasterVersion = _versionResolver.Resolve(Software.SiteMaster, command.SiteMasterVersionMode,
                command.SiteMasterVersion, machineConfig.SiteMasterVersion);
            var siteMasterValidationIssue = await ValidateUpdate(Software.SiteMaster, machineConfig.SiteMasterVersion,
                siteMasterVersion.Version);
            if (siteMasterValidationIssue != null) issues.Add(siteMasterValidationIssue);

            var mmaClass = await Context.Set<Class>().Include(x => x.MmaInstances)
                .FirstOrDefaultAsync(x => x.Id == machineConfig.Account.ClassId, cancellationToken);
            var deployerVersion = _versionResolver.Resolve(Software.Deployer, command.DeployerVersionMode,
                command.DeployerVersion, machineConfig.DeployerVersion, mmaClass?.MmaInstance.DeployerVer);
            var deployerValidationIssue = await ValidateUpdate(Software.Deployer, machineConfig.DeployerVersion,
                deployerVersion.Version);
            if (deployerValidationIssue != null) issues.Add(deployerValidationIssue);

            var pdfExportVersion = _versionResolver.Resolve(Software.PdfExport, command.PdfExportVersionMode,
                command.PdfExportVersion, machineConfig.PdfExportVersion);
            var pdfExportValidationIssue = await ValidateUpdate(Software.PdfExport, machineConfig.PdfExportVersion,
                pdfExportVersion.Version);
            if (pdfExportValidationIssue != null) issues.Add(pdfExportValidationIssue);

            var sqlExportVersion = _versionResolver.Resolve(Software.SqlExport, command.SqlExportVersionMode,
                command.SqlExportVersion, machineConfig.SqlExportVersion);
            var sqlExportValidationIssue = await ValidateUpdate(Software.SqlExport, machineConfig.SqlExportVersion,
                sqlExportVersion.Version);
            if (sqlExportValidationIssue != null) issues.Add(sqlExportValidationIssue);

            var populateVersion = _versionResolver.Resolve(Software.Populate, command.PopulateVersionMode,
                command.PopulateVersion, machineConfig.PopulateVersion);
            var populateValidationIssue = await ValidateUpdate(Software.Populate, machineConfig.PopulateVersion,
                populateVersion.Version);
            if (populateValidationIssue != null) issues.Add(populateValidationIssue);

            var linkwareVersion = _versionResolver.Resolve(Software.Linkware, command.LinkwareVersionMode,
                command.LinkwareVersion, machineConfig.LinkwareVersion);
            var linkwareValidationIssue = await ValidateUpdate(Software.Linkware, machineConfig.LinkwareVersion,
                linkwareVersion.Version);
            if (linkwareValidationIssue != null) issues.Add(linkwareValidationIssue);

            var smchkVersion = _versionResolver.Resolve(Software.Smchk, command.SmchkVersionMode,
                command.SmchkVersion, machineConfig.SmchkVersion);
            var smchkValidationIssue =
                await ValidateUpdate(Software.Smchk, machineConfig.SmchkVersion, smchkVersion.Version);
            if (smchkValidationIssue != null) issues.Add(smchkValidationIssue);

            var discoveryVersion = _versionResolver.Resolve(Software.Discovery, command.DiscoveryVersionMode,
                command.DiscoveryVersion, machineConfig.DiscoveryVersion);
            var discoveryValidationIssue = await ValidateUpdate(Software.Discovery, machineConfig.DiscoveryVersion,
                discoveryVersion.Version);
            if (discoveryValidationIssue != null) issues.Add(discoveryValidationIssue);

            return new ValidateSoftwareUpdateResult
            {
                Issues = issues
            };
        }

        public async Task<ValidationIssue> ValidateUpdate(Software software, string currentVersion, string newVersion)
        {
            var currentVersionInfo = new VersionInfo(currentVersion);
            var newVersionInfo = new VersionInfo(newVersion);

            var noCurrentBranch = currentVersionInfo.Branch == null;
            var currentCommits = await Context.Set<Commit>()
                .Include(c => c.Branch).AsNoTracking()
                .Where(c => c.ShortHash == currentVersionInfo.Version &&
                            (noCurrentBranch || c.Branch.Name == currentVersionInfo.Branch))
                .ToListAsync();

            var noNewBranch = newVersionInfo.Branch == null;
            var newCommits = await Context.Set<Commit>().Include(c => c.Branch).AsNoTracking()
                .Where(c => c.ShortHash == newVersionInfo.Version &&
                            (noNewBranch || c.Branch.Name == newVersionInfo.Branch))
                .ToListAsync();

            var commonBranches = currentCommits.Select(c => c.BranchId).Intersect(newCommits.Select(c => c.BranchId))
                .ToList();

            if (commonBranches.Any())
            {
                var commonBranchId = commonBranches.First();
                var currentCommit = currentCommits.First(c => c.BranchId == commonBranchId);
                var newCommit = newCommits.First(c => c.BranchId == commonBranchId);

                if (newCommit.Timestamp > currentCommit.Timestamp) return null;

                var commitsInBetween = await Context.Set<Commit>().AsNoTracking()
                    .Where(c => c.Timestamp < newCommit.Timestamp && c.Timestamp > currentCommit.Timestamp ||
                                c.Timestamp > newCommit.Timestamp && c.Timestamp < currentCommit.Timestamp)
                    .ToListAsync();

                var breakingChangesCommits =
                    commitsInBetween.Where(c => c.IsBreaking.HasValue && c.IsBreaking.Value).ToList();
                if (breakingChangesCommits.Any())
                    return new ValidationIssue
                    {
                        Description =
                            $"{software.ToDisplayName()} is being downgraded across one or more breaking changes ({string.Join(", ", breakingChangesCommits.Select(c => c.ShortHash))})"
                    };
            }
            else
            {
                return null;
            }

            return null;
        }
    }
}
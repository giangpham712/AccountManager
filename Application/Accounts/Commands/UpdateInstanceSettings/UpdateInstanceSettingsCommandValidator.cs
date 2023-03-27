using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Services;
using AccountManager.Application.SoftwareUpdate;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Machine;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.UpdateInstanceSettings
{
    public class
        UpdateInstanceSettingsCommandValidator : UpdateSoftwareCommandValidatorBase<UpdateInstanceSettingsCommand>
    {
        private static string AwsBucketBaseUrl = "https://planet-nexgen-builds.s3.us-east-2.amazonaws.com/";

        public UpdateInstanceSettingsCommandValidator(
            ICloudStateDbContext context,
            ILibraryFileService libraryFileService,
            IBuildFileService buildFileService) : base(context, libraryFileService, buildFileService)
        {
            RuleFor(x => x).CustomAsync(LibraryFileValid);
        }

        private async Task LibraryFileValid(UpdateInstanceSettingsCommand command,
            ValidationContext<UpdateInstanceSettingsCommand> context,
            CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Include(x => x.Class)
                .FirstOrDefaultAsync(x => x.Id == command.AccountId, cancellationToken);

            if (account == null)
            {
                context.AddFailure("Invalid account");
                return;
            }

            var mmaClass = account.Class;
            if (mmaClass == null) context.AddFailure("Invalid MMA class");

            var launcherMachine = await Context.Set<Machine>()
                .FirstOrDefaultAsync(x => x.AccountId == command.AccountId && x.IsLauncher);

            if (launcherMachine == null)
            {
                context.AddFailure("There is no Launcher machine");
                return;
            }

            var launcherMachineDesiredState = await Context.Set<State>()
                .FirstOrDefaultAsync(x => x.MachineId == launcherMachine.Id && x.Desired);

            if (launcherMachineDesiredState == null)
            {
                context.AddFailure("Launcher machine has no desired state");
                return;
            }

            DateTimeOffset? launcherVersionTimestamp = null;
            var launcherCommit = await Context.Set<Commit>()
                .FirstOrDefaultAsync(x => x.ShortHash == launcherMachineDesiredState.Launcher);
            if (launcherCommit != null)
            {
                launcherVersionTimestamp = launcherCommit.Timestamp;
            }
            else
            {
                if (!launcherMachineDesiredState.Launcher.IsNullOrWhiteSpace())
                    launcherVersionTimestamp = DateTimeOffset.Now;
            }

            var libraryFiles = await Context.Set<File>().Where(x => command.MainLibraryFiles.Contains(x.Id))
                .ToListAsync(cancellationToken);
            foreach (var libraryFile in libraryFiles)
            {
                try
                {
                    if (!await LibraryFileService.FileExists(libraryFile.Url))
                        context.AddFailure($"Library file {libraryFile.Name} doesn't exist on S3");
                }
                catch (Exception e)
                {
                    // Ignore
                }

                if (mmaClass != null && mmaClass.IsProduction && libraryFile.ReleaseStage != ReleaseStage.Released)
                    context.AddFailure($"Library file {libraryFile.Name} must not be used for production account(s)");

                // Validate manifest
                if (launcherVersionTimestamp != null && !libraryFile.Manifest.IsNullOrWhiteSpace())
                    try
                    {
                        var fileManifest = FileManifest.FromString(libraryFile.Manifest);
                        var date = fileManifest.Packages.First().LCTVersion.ShortHash;
                        var lctVersionHash = fileManifest.Packages.First().LCTVersion.ShortHash;
                        var commit = await Context.Set<Commit>()
                            .FirstOrDefaultAsync(x => x.ShortHash == lctVersionHash);
                        if (commit != null)
                        {
                            if (commit.Timestamp > launcherVersionTimestamp)
                                // context.AddFailure($"Library file {libraryFile.Name} is newer than version of Launcher");
                                return;
                            if (launcherVersionTimestamp - commit.Timestamp > new TimeSpan(180, 0, 0, 0))
                                // context.AddFailure($"Library file {libraryFile.Name} is too older");
                                return;
                        }
                    }
                    catch (Exception e)
                    {
                        context.AddFailure($"Library file {libraryFile.Name} has invalid manifest");
                    }
            }
        }
    }
}
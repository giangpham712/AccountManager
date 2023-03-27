using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Services;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Machine;
using FluentValidation;
using FluentValidation.Validators;

namespace AccountManager.Application.SoftwareUpdate.UpdateSoftwareForAccounts
{
    public class
        UpdateSoftwareForAccountsCommandValidator : UpdateSoftwareCommandValidatorBase<UpdateSoftwareForAccountsCommand>
    {
        public UpdateSoftwareForAccountsCommandValidator(ICloudStateDbContext context,
            ILibraryFileService libraryFileService, IBuildFileService buildFileService) : base(context,
            libraryFileService, buildFileService)
        {
            RuleFor(x => x).CustomAsync(LibraryFileValid);
        }

        private async Task LibraryFileValid(UpdateSoftwareForAccountsCommand command,
            ValidationContext<UpdateSoftwareForAccountsCommand> context,
            CancellationToken cancellationToken)
        {
            var libraryFiles = await Context.Set<File>().Where(x => command.MainLibraryFiles.Contains(x.Id))
                .ToListAsync(cancellationToken);
            foreach (var libraryFile in libraryFiles)
                try
                {
                    if (!await LibraryFileService.FileExists(libraryFile.Url))
                        context.AddFailure($"Library file {libraryFile.Name} doesn't exist on S3");
                }
                catch (Exception e)
                {
                    // Ignore
                }

            var accounts = await Context.Set<Account>().Include(x => x.Class)
                .Where(x => command.Accounts.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var account in accounts)
            {
                var mmaClass = account.Class;
                if (mmaClass == null) context.AddFailure($"Account {account.Name} - Invalid MMA class");

                var launcherMachine = await Context.Set<Machine>()
                    .FirstOrDefaultAsync(x => x.AccountId == account.Id && x.IsLauncher);
                var launcherMachineDesiredState = await Context.Set<State>()
                    .FirstOrDefaultAsync(x => x.MachineId == launcherMachine.Id && x.Desired);

                if (launcherMachineDesiredState == null)
                {
                    context.AddFailure("Launcher machine has no desired state");
                    return;
                }

                var launcherCommit = await Context.Set<Commit>()
                    .FirstOrDefaultAsync(x => x.ShortHash == launcherMachineDesiredState.Launcher);

                foreach (var libraryFile in libraryFiles)
                {
                    if (mmaClass != null && mmaClass.IsProduction && libraryFile.ReleaseStage != ReleaseStage.Released)
                        context.AddFailure(
                            $"Account {account.Name} - Library file {libraryFile.Name} must not be used for production account(s)");

                    // Validate manifest
                    if (launcherCommit != null && !libraryFile.Manifest.IsNullOrWhiteSpace())
                        try
                        {
                            var fileManifest = FileManifest.FromString(libraryFile.Manifest);
                            var date = fileManifest.Packages.First().LCTVersion.ShortHash;

                            var commit = await Context.Set<Commit>().FirstOrDefaultAsync(x =>
                                x.ShortHash == fileManifest.Packages.First().LCTVersion.ShortHash);
                            if (commit != null)
                            {
                                if (commit.Timestamp > launcherCommit.Timestamp)
                                {
                                    context.AddFailure(
                                        $"Account {account.Name} - Library file {libraryFile.Name} is newer than version of Launcher");
                                    return;
                                }

                                if (launcherCommit.Timestamp - commit.Timestamp > new TimeSpan(180, 0, 0, 0))
                                {
                                    context.AddFailure(
                                        $"Account {account.Name} - Library file {libraryFile.Name} is too old");
                                    return;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            context.AddFailure(
                                $"Account {account.Name} - Library file {libraryFile.Name} has invalid manifest");
                        }
                }
            }
        }
    }
}
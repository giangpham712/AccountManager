using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Services;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Machine;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.CreateDraftAccount
{
    public class CreateDraftAccountCommandValidator : AbstractValidator<CreateDraftAccountCommand>
    {
        private readonly ICloudStateDbContext _context;
        private readonly ILibraryFileService _libraryFileService;

        public CreateDraftAccountCommandValidator(ICloudStateDbContext context, ILibraryFileService libraryFileService)
        {
            _context = context;
            _libraryFileService = libraryFileService;

            RuleFor(x => x.UrlFriendlyName)
                .NotEmpty().WithMessage("Account URL name is required")
                .MinimumLength(2).WithMessage("Account URL name must have at least 2 characters")
                .Matches(@"^[a-zA-Z][a-zA-Z0-9-][a-zA-Z0-9]*$").WithMessage(
                    "Account URL name must contain only alphanumeric characters and hyphen and start with a letter")
                .Must(UrlFriendlyNameUnique).WithMessage("Account URL name must be unique");


            RuleFor(x => x).CustomAsync(LibraryFileValid);
            RuleFor(x => x).CustomAsync(AccountNameUnique);
        }

        private async Task LibraryFileValid(
            CreateDraftAccountCommand command,
            ValidationContext<CreateDraftAccountCommand> context,
            CancellationToken cancellationToken)
        {
            var mmaClass = await _context.Set<Class>()
                .FirstOrDefaultAsync(x => x.Id == command.ClassId, cancellationToken);
            if (mmaClass == null) context.AddFailure("Invalid MMA class");


            var libraryFiles = await _context.Set<File>().Where(x => command.MainLibraryFiles.Contains(x.Id))
                .ToListAsync(cancellationToken);
            foreach (var libraryFile in libraryFiles)
            {
                try
                {
                    if (!await _libraryFileService.FileExists(libraryFile.Url))
                        context.AddFailure($"Library file {libraryFile.Name} doesn't exist on S3");
                }
                catch (Exception e)
                {
                    // Ignore
                }

                if (mmaClass != null && mmaClass.IsProduction && libraryFile.ReleaseStage != ReleaseStage.Released)
                    context.AddFailure($"Library file {libraryFile.Name} must not be used for production account(s)");
            }
        }

        private async Task AccountNameUnique(CreateDraftAccountCommand command,
            ValidationContext<CreateDraftAccountCommand> context,
            CancellationToken cancellationToken)
        {
            if (await _context.Set<Account>().AnyAsync(x => x.Name == command.Name && !x.IsDeleted, cancellationToken))
                context.AddFailure($"Account or template {command.Name} already exists");
        }

        private bool UrlFriendlyNameUnique(string urlFriendlyName)
        {
            return !_context.Set<Account>().Any(x => x.UrlFriendlyName == urlFriendlyName);
        }
    }
}
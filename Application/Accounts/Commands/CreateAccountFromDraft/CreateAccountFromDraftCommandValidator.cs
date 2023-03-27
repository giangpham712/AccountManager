using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Services;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Machine;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.CreateAccountFromDraft
{
    public class CreateAccountFromDraftCommandValidator : AbstractValidator<CreateAccountFromDraftCommand>
    {
        private readonly ICloudStateDbContext _context;
        private readonly ILibraryFileService _libraryFileService;

        public CreateAccountFromDraftCommandValidator(ICloudStateDbContext context, ILibraryFileService libraryFileService)
        {
            _context = context;
            _libraryFileService = libraryFileService;

            RuleFor(x => x).CustomAsync(AccountNameUnique);
        }

        private async Task AccountNameUnique(CreateAccountFromDraftCommand command,
            ValidationContext<CreateAccountFromDraftCommand> context,
            CancellationToken cancellationToken)
        {
            
        }
    }
}
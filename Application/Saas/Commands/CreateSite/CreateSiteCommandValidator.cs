using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Saas.Commands.CreateSite
{
    public class CreateSiteCommandValidator : AbstractValidator<CreateSiteCommand>
    {
        private readonly ICloudStateDbContext _context;

        public CreateSiteCommandValidator(ICloudStateDbContext context)
        {
            _context = context;


            RuleFor(x => x).CustomAsync(CanCreateNewSite);

            RuleFor(x => x.UrlFriendlyName)
                .NotEmpty().WithMessage("Site URL name is required")
                .MinimumLength(2).WithMessage("Site URL name must have at least 2 characters")
                .Matches(@"^[a-zA-Z][a-zA-Z0-9-][a-zA-Z0-9]*$").WithMessage(
                    "Site URL name must contain only alphanumeric characters and hyphen and start with a letter")
                .MustAsync(UrlFriendlyNameUnique)
                .WithMessage("Site URL name must be unique");
        }

        private async Task CanCreateNewSite(CreateSiteCommand command, ValidationContext<CreateSiteCommand> context,
            CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.LicenseConfig)
                .Include(x => x.Sites)
                .FirstOrDefaultAsync(x => x.UrlFriendlyName == command.AccountUrlFriendlyName, cancellationToken);

            if (account == null || account.IsDeleted)
            {
                context.AddFailure("Accounts not found");
                return;
            }

            if (!account.IsActive)
            {
                context.AddFailure("Accounts has been deactivated");
                return;
            }

            if (account.LicenseConfig.MaxSites <= account.Sites.Count)
                context.AddFailure("Maximum site limit has been reached");
        }


        private async Task<bool> UrlFriendlyNameUnique(CreateSiteCommand command, string urlFriendlyName, CancellationToken token)
        {
            return !await _context.Set<Site>()
                .Include(x => x.Account)
                .AnyAsync(x => x.Account.UrlFriendlyName == command.AccountUrlFriendlyName &&
                          x.UrlFriendlyName == urlFriendlyName, cancellationToken: token);
        }
    }
}
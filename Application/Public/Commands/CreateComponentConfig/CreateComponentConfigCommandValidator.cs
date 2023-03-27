using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Public;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Commands.CreateComponentConfig
{
    public class CreateComponentConfigCommandValidator : AbstractValidator<CreateComponentConfigCommand>
    {
        private readonly ICloudStateDbContext _context;

        public CreateComponentConfigCommandValidator(ICloudStateDbContext context)
        {
            _context = context;

            RuleFor(x => x).CustomAsync(ComponentConfigKeyUnique);
        }

        private async Task ComponentConfigKeyUnique(CreateComponentConfigCommand command,
            ValidationContext<CreateComponentConfigCommand> context,
            CancellationToken cancellationToken)
        {
            if (await _context.Set<ComponentConfig>().AnyAsync(x => x.SubKey == command.SubKey && x.RootKey == command.RootKey, cancellationToken))
                context.AddFailure($"Component config {command.RootKey}.{command.SubKey} already exists");
        }
    }
}

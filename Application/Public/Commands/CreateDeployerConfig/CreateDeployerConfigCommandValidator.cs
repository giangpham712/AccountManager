using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities.Public;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Commands.CreateDeployerConfig
{
    public class CreateDeployerConfigCommandValidator : AbstractValidator<CreateDeployerConfigCommand>
    {
        private readonly ICloudStateDbContext _context;

        public CreateDeployerConfigCommandValidator(ICloudStateDbContext context)
        {
            _context = context;

            RuleFor(x => x).CustomAsync(DeployerConfigKeyUnique);
        }

        private async Task DeployerConfigKeyUnique(CreateDeployerConfigCommand command,
            ValidationContext<CreateDeployerConfigCommand> context,
            CancellationToken cancellationToken)
        {
            if (await _context.Set<DeployerConfig>().AnyAsync(x => x.SubKey == command.SubKey && x.RootKey == command.RootKey, cancellationToken))
                context.AddFailure($"Deployer config {command.RootKey}.{command.SubKey} already exists");
        }
    }
}

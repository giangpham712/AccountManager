using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateGeneralTemplate
{
    public class CreateOrUpdateGeneralTemplateCommandValidator : AbstractValidator<CreateOrUpdateGeneralTemplateCommand>
    {
        private readonly ICloudStateDbContext _context;

        public CreateOrUpdateGeneralTemplateCommandValidator(ICloudStateDbContext context)
        {
            _context = context;

            RuleFor(x => x).CustomAsync(TemplatesNamesUnique);
        }

        private async Task TemplatesNamesUnique(CreateOrUpdateGeneralTemplateCommand command, ValidationContext<CreateOrUpdateGeneralTemplateCommand> context,
            CancellationToken cancellationToken)
        {
            if (await _context.Set<Account>().AnyAsync(x => x.Id != command.Id && x.Name == command.Name, cancellationToken: cancellationToken))
                context.AddFailure($"General template or account {command.Name} already exists");

            if (await _context.Set<LicenseConfig>()
                .AnyAsync(x => (x.Account == null || x.Account.Id != command.Id) && x.Name == command.Name, cancellationToken: cancellationToken))
                context.AddFailure($"License template {command.Name} already exists");

            if (await _context.Set<MachineConfig>()
                .AnyAsync(x => (x.Account == null || x.Account.Id != command.Id) && x.Name == command.Name, cancellationToken: cancellationToken))
                context.AddFailure($"Instance settings template {command.Name} already exists");

            if (await _context.Set<BackupConfig>()
                .AnyAsync(x => (x.Account == null || x.Account.Id != command.Id) && x.Name == command.Name, cancellationToken: cancellationToken))
                context.AddFailure($"Backup settings template {command.Name} already exists");
        }
    }
}
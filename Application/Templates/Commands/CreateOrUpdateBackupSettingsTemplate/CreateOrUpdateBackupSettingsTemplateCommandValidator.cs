using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateBackupSettingsTemplate
{
    public class
        CreateOrUpdateBackupSettingsTemplateCommandValidator : AbstractValidator<
            CreateOrUpdateBackupSettingsTemplateCommand>
    {
        private readonly ICloudStateDbContext _context;

        public CreateOrUpdateBackupSettingsTemplateCommandValidator(ICloudStateDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Template name is required")
                .MustAsync(NameUnique).WithMessage("Template name must be unique");
        }

        private async Task<bool> NameUnique(CreateOrUpdateBackupSettingsTemplateCommand command, string name, CancellationToken token)
        {
            return ! await _context.Set<BackupConfig>().AnyAsync(x => x.Id != command.Id && x.Name == name, token);
        }
    }
}
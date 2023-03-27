using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Templates.Commands.CreateOrUpdateInstanceSettingsTemplate
{
    public class
        CreateOrUpdateInstanceSettingsTemplateCommandValidator : AbstractValidator<
            CreateOrUpdateInstanceSettingsTemplateCommand>
    {
        private readonly ICloudStateDbContext _context;

        public CreateOrUpdateInstanceSettingsTemplateCommandValidator(ICloudStateDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Template name is required")
                .MustAsync(NameUnique).WithMessage("Template name must be unique");
        }

        private async Task<bool> NameUnique(CreateOrUpdateInstanceSettingsTemplateCommand command, string name, CancellationToken token)
        {
            return ! await _context.Set<MachineConfig>().AnyAsync(x => x.Id != command.Id && x.Name == name, token);
        }
    }
}
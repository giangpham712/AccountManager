using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.UpdateBackupSettings
{
    public class UpdateBackupSettingsCommandValidator : AbstractValidator<UpdateBackupSettingsCommand>
    {
        private readonly ICloudStateDbContext _context;

        public UpdateBackupSettingsCommandValidator(ICloudStateDbContext context)
        {
            _context = context;

            RuleFor(x => x).CustomAsync(BackupSettingsNotConflictWithIdleSchedule);
        }

        private async Task BackupSettingsNotConflictWithIdleSchedule(UpdateBackupSettingsCommand command,
            ValidationContext<UpdateBackupSettingsCommand> context, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.IdleSchedules)
                .FirstOrDefaultAsync(x => x.Id == command.AccountId, cancellationToken);

            if (account == null || account.IsDeleted)
            {
                context.AddFailure("Accounts not found");
                return;
            }

            var idleSchedules = account.IdleSchedules;
            var backupTimes = command.Times;

            foreach (var idleSchedule in idleSchedules)
            {
                var from = idleSchedule.StopAt;
                var to = from.AddHours(idleSchedule.ResumeAfter);

                foreach (var backupTime in backupTimes)
                    if (backupTime <= to && backupTime >= @from)
                        context.AddFailure(new ValidationFailure("backupTimes",
                            "Backup time has conflict with idle schedule", backupTime));
            }
        }
    }
}
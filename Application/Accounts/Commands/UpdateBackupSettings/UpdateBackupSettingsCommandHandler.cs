using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.UpdateBackupSettings
{
    public class UpdateBackupSettingsCommandHandler : CommandHandlerBase<UpdateBackupSettingsCommand, Unit>
    {
        private readonly IMapper _mapper;
        public UpdateBackupSettingsCommandHandler(IMediator mediator, ICloudStateDbContext context, IMapper mapper) : base(mediator,
            context)
        {
            _mapper = mapper;
        }

        public override async Task<Unit> Handle(UpdateBackupSettingsCommand command,
            CancellationToken cancellationToken)
        {
            var account = Context.Set<Account>()
                .Include(x => x.BackupConfig)
                .Include(x => x.Machines)
                .FirstOrDefault(x => !x.IsDeleted && x.Id == command.AccountId);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountId);

            var backupConfig = account.BackupConfig;

            if (backupConfig != null)
                _mapper.Map(command, backupConfig);
            else
                account.BackupConfig = _mapper.Map<BackupConfig>(command);

            await account.SetLastUserCycle(Context);

            foreach (var machine in account.Machines)
            {
                machine.Turbo = true;
                machine.SetOperationModeToNormal();
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            await Mediator.Publish(new BackupSettingsPushedEvent
            {
                User = command.User,
                Account = account,
                Command = command,
                Changes = changes
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
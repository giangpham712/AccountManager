using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.UpdateIdleSchedule
{
    public class UpdateIdleScheduleCommandHandler : CommandHandlerBase<UpdateIdleScheduleCommand, Unit>
    {
        private readonly IMapper _mapper;
        public UpdateIdleScheduleCommandHandler(IMediator mediator, ICloudStateDbContext context, IMapper mapper) : base(mediator,
            context)
        {
            _mapper = mapper;
        }

        public override async Task<Unit> Handle(UpdateIdleScheduleCommand command, CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Include(x => x.IdleSchedules)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == command.AccountId, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.AccountId);

            await account.SetLastUserCycle(Context);

            foreach (var idleSchedule in account.IdleSchedules.ToArray())
            {
                Context.Set<IdleSchedule>().Remove(idleSchedule);
                account.IdleSchedules.Remove(idleSchedule);
            }

            foreach (var idleScheduleDto in command.IdleSchedules)
            {
                var newIdleSchedule = _mapper.Map<IdleSchedule>(idleScheduleDto);
                account.IdleSchedules.Add(newIdleSchedule);
            }

            account.Machines = await Context.Set<Machine>()
                .Where(x => x.AccountId.HasValue && x.AccountId.Value == account.Id)
                .ToListAsync(cancellationToken);

            foreach (var machine in account.Machines)
            {
                machine.SetOperationModeToNormal();
                machine.Turbo = true;
                machine.NextStartTime = null;
                machine.NextStopTime = null;
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            await Mediator.Publish(new IdleSchedulePushedEvent
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
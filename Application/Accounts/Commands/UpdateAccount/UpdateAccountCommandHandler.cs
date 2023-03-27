using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.UpdateAccount
{
    public class UpdateAccountCommandHandler : CommandHandlerBase<UpdateAccountCommand, Unit>
    {
        private readonly IMapper _mapper;
        public UpdateAccountCommandHandler(IMediator mediator, ICloudStateDbContext context, IMapper mapper) : base(mediator,
            context)
        {
            _mapper = mapper;
        }

        public override async Task<Unit> Handle(UpdateAccountCommand command, CancellationToken cancellationToken)
        {
            var account = await Context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.Machines)
                .Include(x => x.Billing)
                .SingleOrDefaultAsync(x => !x.IsDeleted && !x.IsTemplate && x.Id == command.Id, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.Id);

            _mapper.Map(command, account);

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            await Mediator.Publish(new AccountPropertiesPushedEvent
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
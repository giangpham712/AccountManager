using System;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccountManager.Application.Machines.Commands.PublishAsSampleData
{
    public class PublishAsSampleDataCommandHandler : CommandHandlerBase<PublishAsSampleDataCommand, Unit>
    {
        public PublishAsSampleDataCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator,
            context)
        {
        }

        public override async Task<Unit> Handle(PublishAsSampleDataCommand command, CancellationToken cancellationToken)
        {
            var machine = await Context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            var operationType = await Context.Set<OperationType>()
                .FirstOrDefaultAsync(x => x.Name == "BSAM", cancellationToken);

            var operation = new Operation
            {
                Type = operationType,
                Active = true,
                Status = "FORCED",
                Timestamp = DateTimeOffset.Now,
                Params = command.FileName,
                MachineId = command.Id,
                TypeName = operationType.Name
            };

            if (machine != null)
            {
                machine.Turbo = true;
                machine.SetOperationModeToNormal();
                await machine.Account.SetLastUserCycle(Context);
            }

            Context.Set<Operation>().Add(operation);
            await Context.SaveChangesAsync(cancellationToken);

            await Mediator.Publish(new MachineActionTriggeredEvent
            {
                User = command.User,
                Machine = machine,
                Action = UserOperationTypes.PublishAsSampleData,
                Params = JsonConvert.SerializeObject(new
                {
                    command.FileName
                })
            }, cancellationToken);

            return Unit.Value;
        }
    }
}
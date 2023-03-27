using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Common;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Commands.UpdateOperation
{
    public class UpdateOperationCommandHandler : CommandHandlerBase<UpdateOperationCommand, Unit>
    {
        public UpdateOperationCommandHandler(IMediator mediator, ICloudStateDbContext context) : base(mediator,
            context)
        {
        }

        public override async Task<Unit> Handle(UpdateOperationCommand command, CancellationToken cancellationToken)
        {
            var operation = await Context.Set<Operation>()
                .FirstOrDefaultAsync(x => x.MachineId == command.MachineId && x.Id == command.OperationId,
                    cancellationToken);

            if (operation == null)
                throw new EntityNotFoundException(nameof(Operation), command.MachineId);


            var patches = new List<Patch>();
            foreach (var patchable in UpdateOperationCommand.Patchables)
            {
                if (!(patchable.GetValue(command) is Patch patch) || !patch.Patchable)
                    continue;

                var targetProperty = typeof(Operation).GetProperty(patchable.Name);
                if (targetProperty == null)
                    continue;

                patch.Name = patchable.Name;

                patches.Add(patch);

                targetProperty.SetValue(operation, patch.Value);
            }

            await Context.SaveChangesAsync(cancellationToken, out var changes);

            return Unit.Value;
        }
    }
}
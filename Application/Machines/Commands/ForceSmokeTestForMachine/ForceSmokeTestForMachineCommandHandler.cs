using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Accounts.Extensions;
using AccountManager.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Commands.ForceSmokeTestForMachine
{
    public class ForceSmokeTestForMachineCommandHandler : IRequestHandler<ForceSmokeTestForMachineCommand>
    {
        private readonly ICloudStateDbContext _context;

        public ForceSmokeTestForMachineCommandHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ForceSmokeTestForMachineCommand command, CancellationToken cancellationToken)
        {
            var machine = await _context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.Account)
                .Include(x => x.CloudInstances)
                .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), command.Id);

            machine.Turbo = true;
            machine.RunSmokeTest = true;
            machine.SetOperationModeToNormal();

            await machine.Account.SetLastUserCycle(_context);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
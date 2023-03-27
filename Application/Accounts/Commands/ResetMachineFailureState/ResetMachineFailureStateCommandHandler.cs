using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;

namespace AccountManager.Application.Accounts.Commands.ResetMachineFailureState
{
    public class ResetMachineFailureStateCommandHandler : IRequestHandler<ResetMachineFailureStateCommand>
    {
        private readonly ICloudStateDbContext _context;

        public ResetMachineFailureStateCommandHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ResetMachineFailureStateCommand request, CancellationToken cancellationToken)
        {
            var machine = _context.Set<Machine>().Find(request.Id);
            if (machine == null)
                throw new EntityNotFoundException(nameof(Machine), request.Id);

            machine.NeedsAdmin = false;
            machine.FailCounter = 0;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
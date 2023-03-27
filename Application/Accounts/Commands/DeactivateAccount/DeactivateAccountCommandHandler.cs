using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Domain;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Commands.DeactivateAccount
{
    public class DeactivateAccountCommandHandler : IRequestHandler<DeactivateAccountCommand>
    {
        private readonly ICloudStateDbContext _context;

        public DeactivateAccountCommandHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeactivateAccountCommand command, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.Machines)
                .FirstOrDefaultAsync(x => !x.IsDeleted && !x.IsTemplate && x.Id == command.Id, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), command.Id);

            account.IsActive = false;
            foreach (var machine in account.Machines)
            {
                machine.Stop = true;
                machine.Turbo = true;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
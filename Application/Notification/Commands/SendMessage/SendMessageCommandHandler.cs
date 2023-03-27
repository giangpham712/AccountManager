using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Machine;
using MediatR;

namespace AccountManager.Application.Notification.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand>
    {
        private readonly ICloudStateDbContext _context;

        public SendMessageCommandHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        {
            var machineIds = command.Machines ?? new List<long>();
            if (command.Accounts != null && command.Accounts.Any())
            {
                var machines = await _context.Set<Machine>()
                    .Where(x => x.AccountId.HasValue && command.Accounts.Contains(x.AccountId.Value))
                    .ToListAsync();

                machineIds = machineIds.Union(machines.Select(x => x.Id)).Distinct();
            }

            if (command.AccountUrlNames != null && command.AccountUrlNames.Any())
            {
                var machines = await _context.Set<Machine>()
                    .Include(x => x.Account)
                    .Where(x => x.AccountId.HasValue && command.AccountUrlNames.Contains(x.Account.UrlFriendlyName))
                    .ToListAsync();

                machineIds = machineIds.Union(machines.Select(x => x.Id)).Distinct();
            }

            var messages = machineIds.Select(x => new Message
            {
                MachineId = x,
                Title = command.Title,
                Body = command.Body,
                ExpiresAfter = 10,
                Timestamp = DateTimeOffset.Now
            });

            foreach (var message in messages) _context.Set<Message>().Add(message);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
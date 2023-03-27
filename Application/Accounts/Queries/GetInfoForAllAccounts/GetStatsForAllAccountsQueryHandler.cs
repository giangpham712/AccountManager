using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetInfoForAllAccounts
{
    public class
        GetStatsForAllAccountsQueryHandler : IRequestHandler<GetStatsForAllAccountsQuery, IEnumerable<AccountStatsDto>>
    {
        private readonly ICloudStateDbContext _context;

        public GetStatsForAllAccountsQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccountStatsDto>> Handle(GetStatsForAllAccountsQuery query,
            CancellationToken cancellationToken)
        {
            var accounts = await _context.Set<Account>()
                .Include(x => x.Machines)
                .Where(x => !x.IsDeleted && !x.IsTemplate)
                .ToListAsync(cancellationToken);

            return accounts.Select(x =>
            {
                var managed = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value);

                var manualMaintenance = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value
                    && y.ManualMaintenance.HasValue && y.ManualMaintenance.Value);

                var idle = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value
                                                                    && y.Idle.HasValue && y.Idle.Value);

                var stop = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value && y.Stop);

                var needsAdmin = x.Machines.Count(y => y.Managed.HasValue && y.Managed.Value && y.NeedsAdmin);
                var turbo = x.Machines.Count(y =>
                    y.Managed.HasValue && y.Managed.Value && y.Turbo.HasValue && y.Turbo.Value);


                return new AccountStatsDto
                {
                    Id = x.Id,
                    MachineCount = x.Machines.Count,
                    NeedsAdmin = needsAdmin,
                    ManualMaintenance = manualMaintenance,
                    Managed = managed,
                    Unmanaged = x.Machines.Count - managed,
                    Turbo = turbo,
                    Idle = idle,
                    Stopped = stop
                };
            });
        }
    }
}
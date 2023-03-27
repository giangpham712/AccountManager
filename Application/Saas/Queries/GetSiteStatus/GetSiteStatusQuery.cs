using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Entities.Public;
using MediatR;

namespace AccountManager.Application.Saas.Queries.GetSiteStatus
{
    public class GetSiteStatusQuery : IRequest<SiteStatusDto>
    {
        public string AccountUrlFriendlyName { get; set; }
        public string UrlFriendlyName { get; set; }
    }

    public class GetSiteStatusQueryHandler : IRequestHandler<GetSiteStatusQuery, SiteStatusDto>
    {
        private readonly ICloudStateDbContext _context;

        public GetSiteStatusQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<SiteStatusDto> Handle(GetSiteStatusQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.UrlFriendlyName == request.AccountUrlFriendlyName,
                    cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.AccountUrlFriendlyName);

            var site = await _context.Set<Site>()
                .Include(x => x.Machine)
                .FirstOrDefaultAsync(x => x.AccountId == account.Id && x.UrlFriendlyName == request.UrlFriendlyName,
                    cancellationToken);

            if (site == null)
                throw new EntityNotFoundException(nameof(Site), request.UrlFriendlyName);

            var machine = site.Machine;

            if (machine == null)
                return null;

            var desiredState = await _context.Set<State>()
                .FirstOrDefaultAsync(x => x.MachineId == machine.Id && x.Desired, cancellationToken);

            var instance = await _context.Set<CloudInstance>()
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync(x => x.MachineId == machine.Id && x.Active, cancellationToken);

            var activeOperation = await _context.Set<Operation>()
                .Include(x => x.Type)
                .FirstOrDefaultAsync(x => x.MachineId == machine.Id && x.Active, cancellationToken);

            var siteStatus = new SiteStatusDto
            {
                InstanceStatus = instance?.Status,
                SiteStatus = GetSiteStatus(machine, instance, activeOperation)
            };

            return await Task.FromResult(siteStatus);
        }

        private static string GetSiteStatus(Machine machine, CloudInstance instance, Operation activeOperation)
        {
            if (instance?.Status == "running") return "ready";

            // if (machine.NeedsAdmin) return "error";

            if (activeOperation != null) return $"{activeOperation.Type.Description} in progress";

            return "unknown";
        }
    }
}
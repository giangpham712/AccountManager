using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Utils;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetCreatableMachinesForAccount
{
    public class GetCreatableMachinesForAccountQuery : IRequest<IEnumerable<CreatableMachineDto>>
    {
        public long Id { get; set; }
    }

    public class GetCreatableMachinesForAccountQueryHandler : IRequestHandler<GetCreatableMachinesForAccountQuery,
        IEnumerable<CreatableMachineDto>>
    {
        private readonly ICloudStateDbContext _context;

        public GetCreatableMachinesForAccountQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CreatableMachineDto>> Handle(GetCreatableMachinesForAccountQuery request,
            CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.LicenseConfig)
                .Include(x => x.Sites)
                .FirstOrDefaultAsync(x => !x.IsDeleted && !x.IsTemplate && x.Id == request.Id, cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.Id);

            var machines = await _context.Set<Machine>().Where(x => x.AccountId == account.Id)
                .ToListAsync(cancellationToken);

            var licenseConfig = account.LicenseConfig;

            var sites = account.Sites;

            var creatableMachines = new List<CreatableMachineDto>();
            var launcherMachine = machines.FirstOrDefault(x => x.IsLauncher);

            if (launcherMachine == null)
            {
                var site = licenseConfig.InstancePolicy == ServerInstancePolicy.AllInOne
                    ? sites.FirstOrDefault()
                    : null;

                creatableMachines.Add(new CreatableMachineDto
                {
                    IsLauncher = true,
                    IsSiteMaster = licenseConfig.InstancePolicy == ServerInstancePolicy.AllInOne,
                    Name = AccountHelper.GenerateMachineName(account.UrlFriendlyName, licenseConfig.InstancePolicy,
                        "Launcher"),
                    SiteId = site?.Id,
                    SiteName = site?.UrlFriendlyName
                });
            }

            if (licenseConfig.InstancePolicy == ServerInstancePolicy.InstancePerSiteMaster)
                foreach (var site in sites)
                {
                    var machine = machines.FirstOrDefault(x => x.SiteName == site.UrlFriendlyName);
                    if (machine == null)
                        creatableMachines.Add(new CreatableMachineDto
                        {
                            IsLauncher = false,
                            IsSiteMaster = true,
                            Name = AccountHelper.GenerateMachineName(account.UrlFriendlyName,
                                licenseConfig.InstancePolicy, site.UrlFriendlyName),
                            SiteId = site.Id,
                            SiteName = site.UrlFriendlyName
                        });
                }

            return creatableMachines;
        }
    }
}
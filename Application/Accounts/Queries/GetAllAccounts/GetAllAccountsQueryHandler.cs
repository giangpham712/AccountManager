using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetAllAccounts
{
    public class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, List<AccountListingDto>>
    {
        private readonly ICloudStateDbContext _context;
        private readonly IMapper _mapper;

        public GetAllAccountsQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AccountListingDto>> Handle(GetAllAccountsQuery request,
            CancellationToken cancellationToken)
        {
            var accounts = await _context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.Class)
                .Include(x => x.Billing)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Where(x => !x.IsDeleted && !x.IsTemplate)
                .ToListAsync(cancellationToken);

            var instanceSettingsTemplates = await _context.Set<MachineConfig>().Where(x => x.IsTemplate)
                .OrderByDescending(x => x.Id).ToListAsync(cancellationToken);

            var machines = await _context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.CloudInstances)
                .Include(x => x.CloudInstanceType)
                .Include(x => x.Class)
                .Where(x => x.AccountId.HasValue)
                .ToListAsync(cancellationToken);

            var machinesByAccount = machines
                .Where(x => x.AccountId.HasValue)
                .GroupBy(x => x.AccountId.Value)
                .ToDictionary(x => x.Key, x => x.ToList());

            var accountDtos = new List<AccountListingDto>();
            foreach (var account in accounts)
            {
                var matched = instanceSettingsTemplates.FirstOrDefault(x => x.MatchVersions(account.MachineConfig));
                var accountDto = _mapper.Map<AccountListingDto>(account);
                accountDto.BundleVersion = matched == null ? "Custom" : matched.Name;

                if (machinesByAccount.TryGetValue(account.Id, out var accountMachines))
                {
                    accountDto.MachineStats = GetMachineStats(accountMachines);
                }

                accountDtos.Add(accountDto);
            }

            return await Task.FromResult(accountDtos);
        }

        private MachineStatsDto GetMachineStats(List<Machine> accountMachines)
        {
            var machineStatsDto = new MachineStatsDto();
            var machinesByClass = new Dictionary<string, int>();

            foreach (var machine in accountMachines)
            {
                var instance = machine.CloudInstances.FirstOrDefault();

                machineStatsDto.Total++;

                if (machine.Class != null)
                {
                    if (!machinesByClass.ContainsKey(machine.Class.Name)) machinesByClass.Add(machine.Class.Name, 0);

                    machinesByClass[machine.Class.Name]++;
                }

                if (machine.Managed.HasValue && machine.Managed.Value && machine.NeedsAdmin)
                    machineStatsDto.NeedsAdmin++;

                if (!machine.Managed.HasValue || !machine.Managed.Value)
                    machineStatsDto.Unmanaged++;

                if (machine.Managed.HasValue && machine.Managed.Value)
                    machineStatsDto.Managed++;

                if (machine.Managed.HasValue && machine.Managed.Value && machine.Turbo.HasValue && machine.Turbo.Value)
                    machineStatsDto.Turbo++;

                var creditCost = machine.CloudInstanceType?.CloudCreditCost ?? 0;
                if (instance == null) continue;

                if (instance.Status == "running")
                {
                    machineStatsDto.RunningCost += creditCost;
                    machineStatsDto.TotalCost += creditCost;
                }
                else if (instance.Status == "stopped")
                {
                    machineStatsDto.TotalCost += creditCost;
                }
            }

            machineStatsDto.ClassStats = machinesByClass.Select(x => new
            {
                Class = x.Key,
                Total = x.Value
            });

            return machineStatsDto;
        }
    }
}
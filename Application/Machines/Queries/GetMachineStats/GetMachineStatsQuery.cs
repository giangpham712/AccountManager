using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Machine;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetMachineStats
{
    public class GetMachineStatsQuery : IRequest<MachineStatsDto>
    {
        public List<int> AccountIds { get; set; }
    }

    public class GetMachineStatsQueryHandler : IRequestHandler<GetMachineStatsQuery, MachineStatsDto>
    {
        private readonly ICloudStateDbContext _context;

        public GetMachineStatsQueryHandler(ICloudStateDbContext context)
        {
            _context = context;
        }

        public async Task<MachineStatsDto> Handle(GetMachineStatsQuery request, CancellationToken cancellationToken)
        {
            List<Machine> machines;

            machines = await _context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.CloudInstances)
                .Include(x => x.CloudInstanceType)
                .Include(x => x.Class)
                .ToListAsync(cancellationToken);

            var machineStatsDto = new MachineStatsDto();

            var machinesByClass = new Dictionary<string, int>();

            foreach (var machine in machines.Where(m => m.Class != null && m.Class.Name != "On Premises"))
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
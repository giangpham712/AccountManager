using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Tasks;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetAllMachines
{
    public class GetAllMachinesQueryHandler : IRequestHandler<GetAllMachinesQuery, IEnumerable<MachineDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;
        private readonly ITaskManager _taskManager;

        public GetAllMachinesQueryHandler(
            ICloudStateDbContext context,
            ITaskManager taskManager, IMapper mapper)
        {
            _context = context;
            _taskManager = taskManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MachineDto>> Handle(GetAllMachinesQuery request,
            CancellationToken cancellationToken)
        {
            var machines = await _context.Set<Domain.Entities.Machine.Machine>()
                .AsNoTracking()
                .Include(x => x.Account)
                .Include(x => x.Class)
                .Include(x => x.CloudInstances.Where(y => y.Active))
                .Include(x => x.Operations.Where(y => y.Active))
                .Where(x => !x.Account.IsDeleted && !(x.Terminate && !x.CloudInstances.Any(y => y.Active)))
                .ToListAsync(cancellationToken);

            var machineIds = machines.Select(x => x.Id).ToArray();
            var states = await _context.Set<State>()
                .Where(x => x.Desired && x.MachineId.HasValue && machineIds.Contains(x.MachineId.Value))
                .ToListAsync(cancellationToken);

            foreach (var machine in machines) machine.States = states.Where(x => x.MachineId == machine.Id).ToList();

            var instanceSettingsTemplates = await _context.Set<MachineConfig>().Where(x => x.IsTemplate)
                .OrderByDescending(x => x.Id).ToListAsync(cancellationToken);

            var machineDtos = new List<MachineDto>();
            foreach (var machine in machines)
            {
                var machineDto = _mapper.Map<MachineDto>(machine);
                var matched = instanceSettingsTemplates.FirstOrDefault(x => x.MatchVersions(machine));
                machineDto.BundleVersion = matched == null ? "Custom" : matched.Name;
                machineDtos.Add(machineDto);
                machineDto.Tasks = _mapper.Map<List<AMTaskDto>>(_taskManager.ListTasksByMachine(machine.Id));
            }

            return machineDtos;
        }
    }
}
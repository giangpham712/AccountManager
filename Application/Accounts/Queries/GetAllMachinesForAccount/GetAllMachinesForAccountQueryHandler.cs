using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetAllMachinesForAccount
{
    public class
        GetAllMachinesForAccountQueryHandler : IRequestHandler<GetAllMachinesForAccountQuery, IEnumerable<MachineDto>>
    {
        private readonly ICloudStateDbContext _context;
        private readonly ITaskManager _taskManager;
        private readonly IMapper _mapper;

        public GetAllMachinesForAccountQueryHandler(ICloudStateDbContext context, ITaskManager taskManager, IMapper mapper)
        {
            _context = context;
            _taskManager = taskManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MachineDto>> Handle(GetAllMachinesForAccountQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Set<Machine>()
                .Include(x => x.Class)
                .Include(x => x.CloudInstanceType)
                .Include(x => x.Config)
                .Where(x => request.Id.HasValue && x.AccountId.HasValue && x.AccountId == request.Id ||
                            x.Account.UrlFriendlyName == request.UrlFriendlyName)
                .Select(x => new
                {
                    x,
                    x.Class,
                    CloudInstances = x.CloudInstances.Where(y => y.Active),
                    x.CloudInstanceType,
                    Operations = x.Operations.Where(y => y.Active),
                    OperationTypes = x.Operations.Select(y => y.Type)
                });


            var machines = query
                .Where(x => !(x.x.Terminate && !x.CloudInstances.Any(y => y.Active)))
                .AsEnumerable()
                .Select(x => x.x)
                .ToList();

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
                machineDto.Tasks = _mapper.Map<List<AMTaskDto>>(_taskManager.ListTasksByMachine(machine.Id));
                machineDto.PendingOperations = machineDto.Operations.Where(IsOperationPending).ToList();
                machineDtos.Add(machineDto);
            }

            return machineDtos;
        }

        private bool IsOperationPending(OperationDto operationDto)
        {
            if (operationDto.Status == "RUNNING" &&
                operationDto.Params == "PENDING")
                return true;

            return false;
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Tasks;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Machines.Queries.GetAllTasksForMachine
{
    public class GetAllTasksForMachineQueryHandler : IRequestHandler<GetAllTasksForMachineQuery, List<AMTaskDto>>
    {
        private readonly ITaskManager _taskManager;
        private readonly IMapper _mapper;

        public GetAllTasksForMachineQueryHandler(ITaskManager taskManager, IMapper mapper)
        {
            _taskManager = taskManager;
            _mapper = mapper;
        }

        public async Task<List<AMTaskDto>> Handle(GetAllTasksForMachineQuery query, CancellationToken cancellationToken)
        {
            var machineTasks = _taskManager.ListTasksByMachine(query.Id);
            return _mapper.Map<List<AMTaskDto>>(machineTasks);
        }
    }
}
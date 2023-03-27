using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Machines.Queries.GetAllTasksForMachine
{
    public class GetAllTasksForMachineQuery : IRequest<List<AMTaskDto>>
    {
        public long Id { get; set; }
    }
}
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Machines.Queries.GetDesiredStateForMachine
{
    
    public class GetDesiredStateForMachineQuery : IRequest<StateDto>
    {
        public long Id { get; set; }
    }
}
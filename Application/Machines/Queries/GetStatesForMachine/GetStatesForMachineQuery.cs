using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Machines.Queries.GetStatesForMachine
{
    public class GetStatesForMachineQuery : IRequest<PagedResult<StateDto>>
    {
        public GetStatesForMachineQuery()
        {
            StartIndex = 0;
            Limit = 20;
        }

        public long? Id { get; set; }

        public bool Desired { get; set; }
        public bool Current { get; set; }

        public int StartIndex { get; set; }
        public int Limit { get; set; }
    }
}
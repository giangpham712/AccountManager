using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Machines.Queries.GetAllOperationsForMachine
{
    public class GetAllOperationsForMachineQuery : IRequest<PagedResult<OperationDto>>
    {
        public GetAllOperationsForMachineQuery()
        {
            StartIndex = 0;
            Limit = 20;
        }

        public long? Id { get; set; }
        public int StartIndex { get; set; }
        public int Limit { get; set; }
    }
}
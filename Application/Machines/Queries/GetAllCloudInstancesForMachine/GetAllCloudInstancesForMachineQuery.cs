using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Machines.Queries.GetAllCloudInstancesForMachine
{
    public class GetAllCloudInstancesForMachineQuery : IRequest<PagedResult<CloudInstanceDto>>
    {
        public GetAllCloudInstancesForMachineQuery()
        {
            StartIndex = 0;
            Limit = 20;
        }

        public long? Id { get; set; }
        public int StartIndex { get; set; }
        public int Limit { get; set; }
    }
}
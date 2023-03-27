using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Machines.Queries.GetRequestStatisticsForMachine
{
    public class GetRequestStatisticsForMachineQuery : IRequest<PagedResult<RequestStatisticsDto>>
    {
        public GetRequestStatisticsForMachineQuery()
        {
            StartIndex = 0;
            Limit = 20;
        }

        public long? Id { get; set; }

        public int StartIndex { get; set; }
        public int Limit { get; set; }
    }
}
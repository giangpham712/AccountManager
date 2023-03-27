using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetRequestStatisticsForMachine
{
    public class GetRequestStatisticsForMachineQueryHandler : IRequestHandler<GetRequestStatisticsForMachineQuery,
            PagedResult<RequestStatisticsDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetRequestStatisticsForMachineQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<RequestStatisticsDto>> Handle(GetRequestStatisticsForMachineQuery request,
            CancellationToken cancellationToken)
        {
            var requestStatisticsQuery = _context.Set<RequestStatistics>()
                .Include(x => x.Machine)
                .ThenInclude(x => x.Class)
                .AsNoTracking()
                .AsQueryable();

            if (request.Id.HasValue && request.Id.Value != 0)
            {
                requestStatisticsQuery = requestStatisticsQuery.Where(x => x.MachineId == request.Id);
            }

            var total = await requestStatisticsQuery.CountAsync(cancellationToken);

            var requestStatistics = await requestStatisticsQuery.OrderByDescending(x => x.LastUpdate)
                .Skip(request.StartIndex)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            return new PagedResult<RequestStatisticsDto>
            {
                Items = _mapper.Map<IEnumerable<RequestStatisticsDto>>(requestStatistics),
                TotalItems = total,
                StartIndex = request.StartIndex,
                Limit = request.Limit,
                HasMore = false
            };
        }
    }
}
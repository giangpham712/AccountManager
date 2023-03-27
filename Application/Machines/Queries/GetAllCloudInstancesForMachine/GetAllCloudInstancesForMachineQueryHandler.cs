using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Machine;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetAllCloudInstancesForMachine
{
    public class GetAllCloudInstancesForMachineQueryHandler : IRequestHandler<GetAllCloudInstancesForMachineQuery,
            PagedResult<CloudInstanceDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllCloudInstancesForMachineQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<CloudInstanceDto>> Handle(GetAllCloudInstancesForMachineQuery request,
            CancellationToken cancellationToken)
        {
            var cloudInstancesQuery = _context.Set<CloudInstance>()
                .Include(x => x.Machine).ThenInclude(x => x.Class)
                .AsNoTracking()
                .AsQueryable();

            if (request.Id.HasValue)
            {
                cloudInstancesQuery = cloudInstancesQuery.Where(x => x.MachineId == request.Id);
            }

            var total = await cloudInstancesQuery.CountAsync(cancellationToken);

            var cloudInstances = await cloudInstancesQuery.OrderByDescending(x => x.Timestamp)
                .Skip(request.StartIndex)
                .Take(request.Limit)
                .Select(x => _mapper.Map<CloudInstanceDto>(x))
                .ToListAsync(cancellationToken);

            return new PagedResult<CloudInstanceDto>()
            {
                Items = cloudInstances,
                TotalItems = total,
                StartIndex = request.StartIndex,
                Limit = request.Limit,
                HasMore = request.StartIndex + cloudInstances.Count < total
            };
        }
    }
}
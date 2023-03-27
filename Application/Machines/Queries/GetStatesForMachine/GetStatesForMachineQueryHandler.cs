using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetStatesForMachine
{
    public class
        GetStatesForMachineQueryHandler : IRequestHandler<GetStatesForMachineQuery,
            PagedResult<StateDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetStatesForMachineQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<StateDto>> Handle(GetStatesForMachineQuery request,
            CancellationToken cancellationToken)
        {
            List<State> states;
            int total;

            if (request.Current)
            {
                states = new List<State>();

                var currentDesiredState = await _context.Set<State>()
                    .Include(x => x.Machine)
                    .ThenInclude(x => x.Class)
                    .Where(x => x.MachineId == request.Id && x.Desired)
                    .OrderByDescending(x => x.Timestamp)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentDesiredState != null)
                {
                    states.Add(currentDesiredState);
                }

                var currentState = await _context.Set<State>()
                    .Include(x => x.Machine)
                    .ThenInclude(x => x.Class)
                    .Where(x => x.MachineId == request.Id && !x.Desired)
                    .OrderByDescending(x => x.Timestamp)
                    .FirstOrDefaultAsync(cancellationToken);

                if (currentState != null)
                {
                    states.Add(currentState);
                }

                total = states.Count;
            }
            else
            {
                var statesQuery = _context.Set<State>()
                        .Include(x => x.Machine)
                        .ThenInclude(x => x.Class)
                        .AsNoTracking()
                        .AsQueryable();

                if (request.Id.HasValue && request.Id.Value != 0)
                {
                    statesQuery = statesQuery.Where(x => x.MachineId == request.Id);
                }

                if (request.Desired)
                {
                    statesQuery = statesQuery.Where(x => x.Desired);
                }

                total = await statesQuery.CountAsync(cancellationToken);

                states = await statesQuery.OrderByDescending(x => x.Timestamp)
                    .Skip(request.StartIndex)
                    .Take(request.Limit)
                    .ToListAsync(cancellationToken);

            }

            return new PagedResult<StateDto>
            {
                Items = _mapper.Map<IEnumerable<StateDto>>(states),
                TotalItems = total,
                StartIndex = request.StartIndex,
                Limit = request.Limit,
                HasMore = false
            };
        }
    }
}
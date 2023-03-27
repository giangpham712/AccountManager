using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetDesiredStateForMachine
{
    public class GetDesiredStateForMachineQueryHandler : IRequestHandler<GetDesiredStateForMachineQuery, StateDto>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetDesiredStateForMachineQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<StateDto> Handle(GetDesiredStateForMachineQuery query, CancellationToken cancellationToken)
        {
            var machine = await _context.Set<Domain.Entities.Machine.Machine>()
                .Include(x => x.States)
                .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken);

            if (machine == null)
                throw new EntityNotFoundException(nameof(Domain.Entities.Machine.Machine), query.Id);

            var desiredState = machine.States.FirstOrDefault(x => x.Desired);
            return _mapper.Map<StateDto>(desiredState);
        }
    }
}
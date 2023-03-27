using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetAllMachineLookups
{
    public class GetAllMachineLookupsQueryHandler : IRequestHandler<GetAllMachineLookupsQuery, IEnumerable<MachineLookupDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllMachineLookupsQueryHandler(IMapper mapper, ICloudStateDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<MachineLookupDto>> Handle(GetAllMachineLookupsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Set<Machine>()
                .Include(x => x.Class)
                .Include(x => x.CloudInstances.Where(y => y.Active))
                .Where(x => (!x.Dummy.HasValue || !x.Dummy.Value) && !x.Account.IsDeleted && !(x.Terminate && !x.CloudInstances.Any(y => y.Active)))
                .OrderBy(x => x.Name)
                .Select(x => new MachineLookupDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ClassName = x.Class == null ? null : x.Class.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
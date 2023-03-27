using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Mma.Queries.GetAllInstances
{
    public class GetAllMmaInstancesQueryHandler : IRequestHandler<GetAllMmaInstancesQuery, IEnumerable<MmaInstanceDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllMmaInstancesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MmaInstanceDto>> Handle(GetAllMmaInstancesQuery request,
            CancellationToken cancellationToken)
        {
            var instances = await _context.Set<MmaInstance>().ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<MmaInstanceDto>>(instances);
        }
    }
}
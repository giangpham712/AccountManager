using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Queries.GetAllComponentConfigs
{
    public class GetAllComponentConfigsQueryHandler : IRequestHandler<GetAllComponentConfigsQuery, List<ComponentConfigDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllComponentConfigsQueryHandler(IMapper mapper, ICloudStateDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<ComponentConfigDto>> Handle(GetAllComponentConfigsQuery request, CancellationToken cancellationToken)
        {
            var componentConfigs = await _context.Set<ComponentConfig>().ToListAsync(cancellationToken);
            return _mapper.Map<List<ComponentConfigDto>>(componentConfigs);
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Public.Queries.GetAllDeployerConfigs
{
    public class GetAllDeployerConfigsQueryHandler : IRequestHandler<GetAllDeployerConfigsQuery, List<DeployerConfigDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllDeployerConfigsQueryHandler(IMapper mapper, ICloudStateDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<DeployerConfigDto>> Handle(GetAllDeployerConfigsQuery request, CancellationToken cancellationToken)
        {
            var componentConfigs = await _context.Set<DeployerConfig>().ToListAsync(cancellationToken);
            return _mapper.Map<List<DeployerConfigDto>>(componentConfigs);
        }
    }
}
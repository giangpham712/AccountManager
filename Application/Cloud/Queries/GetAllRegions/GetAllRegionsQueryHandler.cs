using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Cloud.Queries.GetAllRegions
{
    public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, List<RegionDto>>
    {
        private readonly ICloudStateDbContext _context;
        private readonly IMapper _mapper;

        public GetAllRegionsQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RegionDto>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
        {
            var regions = await _context.Set<CloudRegion>().ToListAsync(cancellationToken);
            return await Task.FromResult(_mapper.Map<List<RegionDto>>(regions));
        }
    }
}
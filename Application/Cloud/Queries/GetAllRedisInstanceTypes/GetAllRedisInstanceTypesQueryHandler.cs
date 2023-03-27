using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Public;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Cloud.Queries.GetAllRedisInstanceTypes
{
    public class
        GetAllRedisInstanceTypesQueryHandler : IRequestHandler<GetAllRedisInstanceTypesQuery,
            List<RedisCloudInstanceTypeDto>>
    {
        private readonly ICloudStateDbContext _context;
        private readonly IMapper _mapper;

        public GetAllRedisInstanceTypesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RedisCloudInstanceTypeDto>> Handle(GetAllRedisInstanceTypesQuery request,
            CancellationToken cancellationToken)
        {
            var instanceTypes = await _context.Set<RedisCloudInstanceType>()
                .AsNoTracking()
                .OrderBy(x => x.MemorySize).ToListAsync(cancellationToken);
            return await Task.FromResult(_mapper.Map<List<RedisCloudInstanceTypeDto>>(instanceTypes));
        }
    }
}
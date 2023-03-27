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

namespace AccountManager.Application.Cloud.Queries.GetAllInstanceTypes
{
    public class GetAllInstanceTypesQueryHandler : IRequestHandler<GetAllInstanceTypesQuery, List<CloudInstanceTypeDto>>
    {
        private static readonly long[] ExcludedInstanceTypes = { 1, 2, 3, 4 };

        private readonly ICloudStateDbContext _context;
        private readonly IMapper _mapper;

        public GetAllInstanceTypesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CloudInstanceTypeDto>> Handle(GetAllInstanceTypesQuery request,
            CancellationToken cancellationToken)
        {
            var instanceTypes = await _context.Set<CloudInstanceType>()
                .AsNoTracking()
                .Where(x => !ExcludedInstanceTypes.Contains(x.Id))
                .OrderBy(x => x.StorageSize).ToListAsync(cancellationToken);
            return await Task.FromResult(_mapper.Map<List<CloudInstanceTypeDto>>(instanceTypes));
        }
    }
}
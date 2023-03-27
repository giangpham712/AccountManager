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

namespace AccountManager.Application.Cloud.Queries.GetAllImages
{
    public class GetAllImagesQueryHandler : IRequestHandler<GetAllImagesQuery, List<CloudBaseImageDto>>
    {
        private readonly ICloudStateDbContext _context;
        private readonly IMapper _mapper;

        public GetAllImagesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CloudBaseImageDto>> Handle(GetAllImagesQuery request,
            CancellationToken cancellationToken)
        {
            var instanceTypes =
                await _context.Set<CloudBaseImage>().OrderBy(x => x.Name).ToListAsync(cancellationToken);
            return await Task.FromResult(_mapper.Map<List<CloudBaseImageDto>>(instanceTypes));
        }
    }
}
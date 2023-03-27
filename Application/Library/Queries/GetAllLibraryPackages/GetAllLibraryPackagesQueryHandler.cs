using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities.Library;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Library.Queries.GetAllLibraryPackages
{
    public class GetAllLibraryPackagesQueryHandler : IRequestHandler<GetAllLibraryPackagesQuery, List<PackageDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllLibraryPackagesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PackageDto>> Handle(GetAllLibraryPackagesQuery request,
            CancellationToken cancellationToken)
        {
            var packages = await _context.Set<Package>().ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<PackageDto>>(packages);

            return dtos;
        }
    }
}
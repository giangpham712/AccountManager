using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Queries.GetAllLicenseTemplates
{
    public class
        GetAllLicenseTemplatesQueryHandler : IRequestHandler<GetAllLicenseTemplatesQuery, IEnumerable<LicenseConfigDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllLicenseTemplatesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LicenseConfigDto>> Handle(GetAllLicenseTemplatesQuery request,
            CancellationToken cancellationToken)
        {
            var licenseTemplates = await _context.Set<LicenseConfig>()
                .Include(x => x.Account)
                .Where(x => x.Account == null || !x.Account.IsDeleted)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<LicenseConfigDto>>(licenseTemplates);
        }
    }
}
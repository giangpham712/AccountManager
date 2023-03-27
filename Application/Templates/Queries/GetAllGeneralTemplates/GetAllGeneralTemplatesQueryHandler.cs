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

namespace AccountManager.Application.Templates.Queries.GetAllGeneralTemplates
{
    public class
        GetAllGeneralTemplatesQueryHandler : IRequestHandler<GetAllGeneralTemplatesQuery, IEnumerable<AccountDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllGeneralTemplatesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountDto>> Handle(GetAllGeneralTemplatesQuery request,
            CancellationToken cancellationToken)
        {
            var generalTemplates = await _context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.Billing)
                .Include(x => x.BackupConfig)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Where(x => !x.IsDeleted)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<AccountDto>>(generalTemplates);
        }
    }
}
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

namespace AccountManager.Application.Templates.Queries.GetAllBackupSettingsTemplates
{
    public class
        GetAllBackupSettingsTemplatesQueryHandler : IRequestHandler<GetAllBackupSettingsTemplatesQuery,
            IEnumerable<BackupConfigDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllBackupSettingsTemplatesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BackupConfigDto>> Handle(GetAllBackupSettingsTemplatesQuery request,
            CancellationToken cancellationToken)
        {
            var backupSettingsTemplates = await _context.Set<BackupConfig>()
                .Include(x => x.Account)
                .Where(x => x.Account == null || !x.Account.IsDeleted)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<BackupConfigDto>>(backupSettingsTemplates);
        }
    }
}
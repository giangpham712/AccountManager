using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Exceptions;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetAccount
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAccountQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Set<Account>()
                .Include(x => x.Contact)
                .Include(x => x.Class)
                .Include(x => x.Billing)
                .Include(x => x.LicenseConfig)
                .Include(x => x.MachineConfig)
                .Include(x => x.BackupConfig)
                .Include(x => x.IdleSchedules)
                .Include(x => x.Sites)
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
                throw new EntityNotFoundException(nameof(Account), request.Id);

            var accountDto = _mapper.Map<AccountDto>(account);

            // License settings
            var licenseTemplates = await _context.Set<LicenseConfig>().Where(x => x.IsTemplate)
                .OrderByDescending(x => x.Id).ToListAsync(cancellationToken);
            var matchedLicenseTemplate = licenseTemplates.FirstOrDefault(x => x.Match(account.LicenseConfig));
            accountDto.LicenseConfig.MatchedTemplate =
                matchedLicenseTemplate == null ? "Custom" : matchedLicenseTemplate.Name;

            // Instance settings
            var machineConfigDto = accountDto.MachineConfig;
            var libraryFileIds = machineConfigDto.MainLibraryFileIds;

            var libraryFiles = await _context.Set<File>().Where(x => x.Id != 0 && libraryFileIds.Contains(x.Id))
                .ToListAsync(cancellationToken);
            machineConfigDto.MainLibraryFiles = _mapper.Map<FileDto[]>(libraryFiles.ToArray());

            if (machineConfigDto.MainLibraryFileId.HasValue && machineConfigDto.MainLibraryFileId != 0)
            {
                var mainLibraryFile = await _context.Set<File>()
                    .FirstOrDefaultAsync(x => x.Id == machineConfigDto.MainLibraryFileId, cancellationToken);
                machineConfigDto.MainLibraryFile = _mapper.Map<FileDto>(mainLibraryFile);
            }

            if (machineConfigDto.AccountLibraryFileId.HasValue && machineConfigDto.AccountLibraryFileId != 0)
            {
                var accountLibraryFile = await _context.Set<File>()
                    .FirstOrDefaultAsync(x => x.Id == machineConfigDto.AccountLibraryFileId, cancellationToken);
                machineConfigDto.AccountLibraryFile = _mapper.Map<FileDto>(accountLibraryFile);
            }

            var instanceSettingsTemplates = await _context.Set<MachineConfig>().Where(x => x.IsTemplate)
                .OrderByDescending(x => x.Id).ToListAsync(cancellationToken);
            var matched = instanceSettingsTemplates.FirstOrDefault(x => x.MatchVersions(account.MachineConfig));
            machineConfigDto.BundleVersion = matched == null ? "Custom" : matched.Name;

            return await Task.FromResult(accountDto);
        }
    }
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Extensions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Accounts.Queries.GetInstanceSettingsForAccount
{
    public class GetInstanceSettingsForAccountQuery : IRequest<MachineConfigDto>
    {
        public long Id { get; set; }
    }

    public class
        GetInstanceSettingsForAccountQueryHandler : IRequestHandler<GetInstanceSettingsForAccountQuery,
            MachineConfigDto>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetInstanceSettingsForAccountQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MachineConfigDto> Handle(GetInstanceSettingsForAccountQuery query,
            CancellationToken cancellationToken)
        {
            var machineConfig = await _context.Set<MachineConfig>().Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Account != null && x.Account.Id == query.Id, cancellationToken);

            var machineConfigDto = _mapper.Map<MachineConfigDto>(machineConfig);

            var libraryFileIds = machineConfigDto.MainLibraryFileIds;

            var libraryFiles = await _context.Set<File>().Where(x => x.Id != 0 && libraryFileIds.Contains(x.Id))
                .ToListAsync(cancellationToken);
            machineConfigDto.MainLibraryFiles = _mapper.Map<FileDto[]>(libraryFiles.ToArray());

            if (machineConfigDto.MainLibraryFileId.HasValue)
            {
                var mainLibraryFile = await _context.Set<File>()
                    .FirstOrDefaultAsync(x => x.Id == machineConfigDto.MainLibraryFileId, cancellationToken);
                machineConfigDto.MainLibraryFile = _mapper.Map<FileDto>(mainLibraryFile);
            }

            if (machineConfigDto.AccountLibraryFileId.HasValue)
            {
                var accountLibraryFile = await _context.Set<File>()
                    .FirstOrDefaultAsync(x => x.Id == machineConfigDto.AccountLibraryFileId, cancellationToken);
                machineConfigDto.AccountLibraryFile = _mapper.Map<FileDto>(accountLibraryFile);
            }

            var instanceSettingsTemplates = await _context.Set<MachineConfig>().Where(x => x.IsTemplate)
                .OrderByDescending(x => x.Id).ToListAsync(cancellationToken);
            var matched = instanceSettingsTemplates.FirstOrDefault(x => x.MatchVersions(machineConfig));
            machineConfigDto.BundleVersion = matched == null ? "Custom" : matched.Name;

            return machineConfigDto;
        }
    }
}
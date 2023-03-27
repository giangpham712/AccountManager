using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Entities;
using AccountManager.Domain.Entities.Account;
using AccountManager.Domain.Entities.Library;
using AutoMapper;
using MediatR;

namespace AccountManager.Application.Templates.Queries.GetAllInstanceSettingsTemplates
{
    public class GetAllInstanceSettingsTemplatesQueryHandler : IRequestHandler<GetAllInstanceSettingsTemplatesQuery,
        IEnumerable<MachineConfigDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetAllInstanceSettingsTemplatesQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MachineConfigDto>> Handle(GetAllInstanceSettingsTemplatesQuery request,
            CancellationToken cancellationToken)
        {
            var licenseTemplates = await _context.Set<MachineConfig>()
                .Include(x => x.Account)
                .Where(x => x.Account == null || x.Account != null && !x.Account.IsDeleted)
                .ToListAsync(cancellationToken);

            var allLibraryFileIds = licenseTemplates.SelectMany(x => x.MainLibraryFiles)
                .Union(licenseTemplates.Where(x => x.AccountLibraryFile.HasValue)
                    .Select(x => x.AccountLibraryFile.Value)).Distinct();

            var libraryFiles = await _context.Set<File>()
                .Where(x => x.Id != 0 && allLibraryFileIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var libraryFileMap = _mapper.Map<IEnumerable<FileDto>>(libraryFiles).ToDictionary(x => x.Id, x => x);

            var licenseTemplateDtos = _mapper.Map<List<MachineConfigDto>>(licenseTemplates);

            foreach (var licenseTemplateDto in licenseTemplateDtos)
            {
                licenseTemplateDto.MainLibraryFiles = licenseTemplateDto.MainLibraryFileIds.Select(x => libraryFileMap.TryGetValue(x, out var mainLibraryFile) ? mainLibraryFile : null).Where(x => x != null).ToArray();

                if (licenseTemplateDto.AccountLibraryFileId.HasValue)
                {
                    licenseTemplateDto.AccountLibraryFile =
                        libraryFileMap.TryGetValue(licenseTemplateDto.AccountLibraryFileId.Value,
                            out var accountLibraryFile)
                            ? accountLibraryFile
                            : null;
                }
            }

            return licenseTemplateDtos;
        }
    }
}
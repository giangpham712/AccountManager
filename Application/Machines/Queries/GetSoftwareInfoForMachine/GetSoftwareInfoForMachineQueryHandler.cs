using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Common.Extensions;
using AccountManager.Domain.Constants;
using AccountManager.Domain.Entities.Git;
using AccountManager.Domain.Entities.Library;
using AccountManager.Domain.Entities.Machine;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.Application.Machines.Queries.GetSoftwareInfoForMachine
{
    public class
        GetSoftwareInfoForMachineQueryHandler : IRequestHandler<GetSoftwareInfoForMachineQuery, MachineStateInfoDto>
    {
        private readonly IMapper _mapper;
        private readonly ICloudStateDbContext _context;

        public GetSoftwareInfoForMachineQueryHandler(ICloudStateDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MachineStateInfoDto> Handle(GetSoftwareInfoForMachineQuery request,
            CancellationToken cancellationToken)
        {
            var states = new List<State>();
            var latestState = await _context.Set<State>().OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync(x => x.MachineId == request.Id && !x.Desired, cancellationToken);

            if (latestState != null)
                states.Add(latestState);

            var desiredState = await _context.Set<State>()
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync(x => x.MachineId == request.Id && x.Desired, cancellationToken);

            if (desiredState != null)
                states.Add(desiredState);

            var stateDtos = _mapper.Map<List<StateDto>>(states);

            var hashes = states.SelectMany(x =>
            {
                var stateHashes = new List<string>
                {
                    x.Launcher,
                    x.Reporting,
                    x.PdfExport,
                    x.SiteMaster,
                    x.Client,
                    x.RelExport, //TODO: To be removed
                    x.SqlExport,
                    x.Deployer,
                    x.Populate,
                    x.Linkware,
                    x.Smchk,
                    x.Discovery,
                    x.FiberSenSys,
                    x.FiberMountain,
                    x.ServiceNow,
                    x.CommScope,
                };
                return stateHashes;
            }).Distinct().Where(x => !x.IsNullOrWhiteSpace() && x != Versions.None);

            var commits = await _context.Set<Commit>().Include(x => x.Branch).Where(x => hashes.Contains(x.ShortHash))
                .ToListAsync(cancellationToken);
            var commitDtos = _mapper.Map<List<CommitDto>>(commits);
            var commitGroupByBranch = commitDtos.GroupBy(x => x.BranchId).OrderByDescending(x => x.Count());

            foreach (var stateDto in stateDtos)
            {
                foreach (var group in commitGroupByBranch)
                {
                    stateDto.LauncherCommit = stateDto.LauncherCommit ??
                                              group.FirstOrDefault(x => x.ShortHash == stateDto.Launcher);
                    stateDto.ReportingCommit = stateDto.ReportingCommit ??
                                               group.FirstOrDefault(x => x.ShortHash == stateDto.Reporting);
                    stateDto.PdfExportCommit = stateDto.PdfExportCommit ??
                                               group.FirstOrDefault(x => x.ShortHash == stateDto.PdfExport);
                    stateDto.SiteMasterCommit = stateDto.SiteMasterCommit ??
                                                group.FirstOrDefault(x => x.ShortHash == stateDto.SiteMaster);
                    stateDto.ClientCommit = stateDto.ClientCommit ??
                                            group.FirstOrDefault(x => x.ShortHash == stateDto.Client);
                    stateDto.RelExportCommit = stateDto.RelExportCommit ??
                                               group.FirstOrDefault(x =>
                                                   x.ShortHash == stateDto.RelExport); //TODO: To be removed
                    stateDto.SqlExportCommit = stateDto.SqlExportCommit ??
                                               group.FirstOrDefault(x => x.ShortHash == stateDto.SqlExport);
                    stateDto.DeployerCommit = stateDto.DeployerCommit ??
                                              group.FirstOrDefault(x => x.ShortHash == stateDto.Deployer);
                    stateDto.PopulateCommit = stateDto.PopulateCommit ??
                                              group.FirstOrDefault(x => x.ShortHash == stateDto.Populate);
                    stateDto.LinkwareCommit = stateDto.LinkwareCommit ??
                                              group.FirstOrDefault(x => x.ShortHash == stateDto.Linkware);
                    stateDto.DiscoveryCommit = stateDto.DiscoveryCommit ??
                                               group.FirstOrDefault(x => x.ShortHash == stateDto.Discovery);
                    stateDto.FiberSenSysCommit = stateDto.FiberSenSysCommit ??
                                                 group.FirstOrDefault(x => x.ShortHash == stateDto.FiberSenSys);
                    stateDto.FiberMountainCommit = stateDto.FiberMountainCommit ??
                                                 group.FirstOrDefault(x => x.ShortHash == stateDto.FiberMountain);
                    stateDto.ServiceNowCommit = stateDto.ServiceNowCommit ??
                                                 group.FirstOrDefault(x => x.ShortHash == stateDto.ServiceNow);
                    stateDto.CommScopeCommit = stateDto.CommScopeCommit ??
                                                 group.FirstOrDefault(x => x.ShortHash == stateDto.CommScope);
                    stateDto.SmchkCommit =
                        stateDto.SmchkCommit ?? group.FirstOrDefault(x => x.ShortHash == stateDto.Smchk);
                }

                stateDto.LibraryFiles = stateDto.LibraryFileIds.Any()
                    ? _mapper.Map<IEnumerable<FileDto>>(await _context.Set<File>()
                            .Where(x => x.Id != 0 && stateDto.LibraryFileIds.Contains(x.Id))
                            .ToListAsync(cancellationToken))
                        .ToArray()
                    : new FileDto[0];

                stateDto.AccountLibraryFile = stateDto.AccountLibraryFileId.HasValue
                    ? _mapper.Map<FileDto>(await _context.Set<File>()
                        .FirstOrDefaultAsync(x => x.Id != 0 && stateDto.AccountLibraryFileId.Value == x.Id,
                            cancellationToken))
                    : null;
            }

            return new MachineStateInfoDto
            {
                States = stateDtos
            };
        }
    }
}
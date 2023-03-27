using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Services;
using AccountManager.Domain;
using MediatR;

namespace AccountManager.Application.BinaryBuild.Queries.GetAllBuildsForSoftware
{
    public class GetAllBuildsForSoftwareQuery : IRequest<IEnumerable<BuildFile>>
    {
        public Software Software { get; set; }
        public BuildFileType Type { get; set; }
    }

    public class
        GetAllBuildsForSoftwareQueryHandler : IRequestHandler<GetAllBuildsForSoftwareQuery, IEnumerable<BuildFile>>
    {
        private readonly IBuildFileService _buildFileService;

        public GetAllBuildsForSoftwareQueryHandler(IBuildFileService buildFileService)
        {
            _buildFileService = buildFileService;
        }

        public async Task<IEnumerable<BuildFile>> Handle(GetAllBuildsForSoftwareQuery request,
            CancellationToken cancellationToken)
        {
            var buildFiles = await _buildFileService.ListBuildFiles(request.Software, request.Type);
            return buildFiles;
        }
    }
}
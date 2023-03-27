using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccountManager.Application.Services;
using MediatR;

namespace AccountManager.Application.Library.Queries.GetAllSampleDataFiles
{
    public class
        GetAllSampleDataFilesQueryHandler : IRequestHandler<GetAllSampleDataFilesQuery, IEnumerable<SampleDataFile>>
    {
        private readonly ISampleDataFileService _sampleDataFileService;

        public GetAllSampleDataFilesQueryHandler(ISampleDataFileService sampleDataFileService)
        {
            _sampleDataFileService = sampleDataFileService;
        }

        public async Task<IEnumerable<SampleDataFile>> Handle(GetAllSampleDataFilesQuery request,
            CancellationToken cancellationToken)
        {
            return await _sampleDataFileService.ListFiles();
        }
    }
}
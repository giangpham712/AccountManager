using System.Collections.Generic;
using AccountManager.Application.Services;
using MediatR;

namespace AccountManager.Application.Library.Queries.GetAllSampleDataFiles
{
    public class GetAllSampleDataFilesQuery : IRequest<IEnumerable<SampleDataFile>>
    {
    }
}
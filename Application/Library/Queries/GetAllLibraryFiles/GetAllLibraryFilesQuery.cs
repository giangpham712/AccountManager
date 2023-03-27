using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Library.Queries.GetAllLibraryFiles
{
    public class GetAllLibraryFilesQuery : IRequest<List<FileDto>>
    {
    }
}
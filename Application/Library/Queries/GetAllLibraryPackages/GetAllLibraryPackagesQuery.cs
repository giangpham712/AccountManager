using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Library.Queries.GetAllLibraryPackages
{
    public class GetAllLibraryPackagesQuery : IRequest<List<PackageDto>>
    {
    }
}
using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Cloud.Queries.GetAllRegions
{
    public class GetAllRegionsQuery : IRequest<List<RegionDto>>
    {
    }
}
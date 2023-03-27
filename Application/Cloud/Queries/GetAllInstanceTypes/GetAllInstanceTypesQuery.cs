using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Cloud.Queries.GetAllInstanceTypes
{
    public class GetAllInstanceTypesQuery : IRequest<List<CloudInstanceTypeDto>>
    {
    }
}
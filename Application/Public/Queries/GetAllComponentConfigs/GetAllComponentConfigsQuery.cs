using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Public.Queries.GetAllComponentConfigs
{
    public class GetAllComponentConfigsQuery : IRequest<List<ComponentConfigDto>>
    {
    }
}

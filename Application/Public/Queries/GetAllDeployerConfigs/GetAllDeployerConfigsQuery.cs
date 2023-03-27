using System.Collections.Generic;
using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Public.Queries.GetAllDeployerConfigs
{
    public class GetAllDeployerConfigsQuery : IRequest<List<DeployerConfigDto>>
    {
    }
}

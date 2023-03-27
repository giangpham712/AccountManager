using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Public.Queries.GetDeployerConfigByKey
{
    public class GetDeployerConfigByKeyQuery : IRequest<DeployerConfigDto>
    {
        public string Key { get; set; }
    }
}

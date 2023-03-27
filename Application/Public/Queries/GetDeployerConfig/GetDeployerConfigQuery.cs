using AccountManager.Application.Models.Dto;
using MediatR;

namespace AccountManager.Application.Public.Queries.GetDeployerConfig
{
    public class GetDeployerConfigQuery : IRequest<DeployerConfigDto>
    {
        public long Id { get; set; }
    }
}

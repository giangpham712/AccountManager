using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Public.Commands.CreateComponentConfig;
using AccountManager.Application.Public.Commands.UpdateComponentConfig;
using AccountManager.Application.Public.Queries.GetAllDeployerConfigs;
using AccountManager.Application.Public.Queries.GetDeployerConfig;
using AccountManager.Application.Public.Queries.GetDeployerConfigByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/deployer-configs")]
    [ApiController]
    public class DeployerConfigApiController : AuthorizedApiController
    {
        public DeployerConfigApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet]
        [Route("")]
        public async Task<List<DeployerConfigDto>> GetAll()
        {
            return await Mediator.Send(new GetAllDeployerConfigsQuery());
        }

        [HttpGet]
        [Route("{id:long}")]
        public async Task<DeployerConfigDto> GetBy([FromRoute] long id)
        {
            return await Mediator.Send(new GetDeployerConfigQuery()
            {
                Id = id
            });
        }

        [HttpGet]
        [Route("{key}")]
        public async Task<DeployerConfigDto> GetByKey([FromRoute] string key)
        {
            return await Mediator.Send(new GetDeployerConfigByKeyQuery()
            {
                Key = key
            });
        }


        #endregion

        #region Commands

        [HttpPost]
        [Route("")]
        public async Task<Unit> Create([FromBody] CreateComponentConfigCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut]
        [Route("{id:long}")]
        public async Task<Unit> Update([FromRoute] long id, [FromBody] UpdateComponentConfigCommand command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        #endregion
    }
}
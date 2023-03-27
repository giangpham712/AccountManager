using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Application.Models.Dto;
using AccountManager.Application.Public.Commands.CreateComponentConfig;
using AccountManager.Application.Public.Commands.DeleteComponentConfig;
using AccountManager.Application.Public.Commands.UpdateComponentConfig;
using AccountManager.Application.Public.Queries.GetAllComponentConfigs;
using AccountManager.Application.Public.Queries.GetComponentConfig;
using AccountManager.Application.Public.Queries.GetComponentConfigByKey;
using MediatR;

namespace AccountManager.WebApi.Controllers.Api
{
    [Route(ApiVersion + "/component-configs")]
    [ApiController]
    public class ComponentConfigApiController : AuthorizedApiController
    {
        public ComponentConfigApiController(IMediator mediator) : base(mediator)
        {
        }

        #region Queries

        [HttpGet]
        [Route("")]
        public async Task<List<ComponentConfigDto>> GetAll()
        {
            return await Mediator.Send(new GetAllComponentConfigsQuery());
        }

        [HttpGet]
        [Route("{id:long}")]
        public async Task<ComponentConfigDto> GetBy([FromRoute] long id)
        {
            return await Mediator.Send(new GetComponentConfigQuery()
            {
                Id = id
            });
        }

        [HttpGet]
        [Route("{key}")]
        public async Task<ComponentConfigDto> GetByKey([FromRoute] string key)
        {
            return await Mediator.Send(new GetComponentConfigByKeyQuery()
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

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<Unit> Delete([FromRoute] long id)
        {
            return await Mediator.Send(new DeleteComponentConfigCommand()
            {
                Id = id
            });
        }

        #endregion
    }
}
